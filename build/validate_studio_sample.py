"""Check a showcase sample in studio/<sample-id>/ against the layout and info file in studio/README.md.

Usage:
  python build/validate_studio_sample.py [--root <folder>] [--no-images] [<sample-id> ...]

With no ids it checks every folder in studio/ that has .studiolive/sample.json. --root checks samples outside this repo
(a staging folder with one sample per subfolder). Prints one line per problem and exits 1 if any sample has an error.
Warnings (W) do not fail the run.

Needs Pillow for the icon and screenshot sizes (pip install pillow). Without it the run fails, unless --no-images skips
the image checks on purpose.
"""
import argparse
import json
import re
import subprocess
import sys
from pathlib import Path
from urllib.parse import unquote

try:
    from PIL import Image, UnidentifiedImageError
except ImportError:
    Image = None
    UnidentifiedImageError = OSError

SCHEMA = 'https://schemas.platform.uno/sample/v1'
RAW = 'https://raw.githubusercontent.com/unoplatform/Uno.Samples/master/studio/'
PLATFORMS = {'desktop', 'wasm', 'ios', 'android', 'macos'}
TYPES = {'showcase', 'starter', 'feature'}
STATUSES = {'active', 'preview', 'legacy'}
TOP = {'$schema', 'id', 'displayName', 'description', 'platforms', 'sourcePath', 'iconUrl', 'screenshots', 'showcase'}
SHOWCASE = {'summary', 'type', 'status', 'owner', 'industry', 'goals', 'requires', 'tags', 'liveUrl'}
SCREEN_SIZES = {(1280, 800), (390, 844)}
ICON_SIZE, ICON_BYTES = (256, 256), 200 * 1024
KEBAB = re.compile(r'^[a-z0-9]+(-[a-z0-9]+)*$')
# GitHub handles: 1 to 39 letters, digits and single hyphens, not starting or ending with a hyphen.
HANDLE = re.compile(r'^(?=.{1,39}$)[A-Za-z0-9]+(?:-[A-Za-z0-9]+)*$')


def text(value):
    return isinstance(value, str) and value.strip() != ''


def strings(value):
    return isinstance(value, list) and all(text(v) for v in value)


class Report:
    def __init__(self, sample):
        self.sample, self.errors, self.warnings = sample, [], []

    def error(self, message):
        self.errors.append(message)

    def warn(self, message):
        self.warnings.append(message)


def read_json_object(path, label, report, strip_comments=False):
    """The JSON object in a file, or None after reporting why not. utf-8-sig accepts a byte order mark, as .NET does."""
    try:
        raw = path.read_text(encoding='utf-8-sig')
    except FileNotFoundError:
        return None
    except (OSError, UnicodeDecodeError) as e:
        report.error(f'{label}: cannot be read ({e})')
        return None
    try:
        data = json.loads(without_comments(raw) if strip_comments else raw)
    except ValueError as e:
        report.error(f'{label}: not valid JSON ({e})')
        return None
    if not isinstance(data, dict):
        report.error(f'{label}: must be a JSON object, got {type(data).__name__}')
        return None
    return data


def without_comments(raw):
    """JSON with // and /* */ comments removed outside strings, the way dotnet reads global.json."""
    out, i, n = [], 0, len(raw)
    while i < n:
        c = raw[i]
        if c == '"':
            j = i + 1
            while j < n and raw[j] != '"':
                j += 2 if raw[j] == '\\' else 1
            out.append(raw[i:j + 1])
            i = j + 1
        elif raw.startswith('//', i):
            while i < n and raw[i] != '\n':
                i += 1
        elif raw.startswith('/*', i):
            end = raw.find('*/', i + 2)
            i = n if end < 0 else end + 2
        else:
            out.append(c)
            i += 1
    return ''.join(out)


def local_file(url, folder, sample_id, report, field):
    """The file in the sample folder that an absolute raw.githubusercontent URL points at."""
    if not text(url) or not url.startswith('https://'):
        report.error(f'{field}: must be an absolute https URL, got {url!r}')
        return None
    prefix = RAW + sample_id + '/'
    if not url.startswith(prefix):
        report.error(f'{field}: must start with {prefix}')
        return None
    relative = unquote(url[len(prefix):])
    path = (folder / relative).resolve()
    if not path.is_relative_to(folder.resolve()):
        report.error(f'{field}: {relative} points outside the sample folder')
        return None
    if not path.is_file():
        report.error(f'{field}: {relative} is not in the sample folder')
        return None
    return path


def image_size(path, report, field):
    """The image's size and format, or None after reporting that it can't be read as an image."""
    try:
        with Image.open(path) as image:
            return image.size, image.format
    except (UnidentifiedImageError, OSError) as e:
        report.error(f'{field}: {path.name} is not a readable image ({e})')
        return None


def check_info(folder, info, report, images):
    sample_id = folder.name
    for key in sorted(set(info) - TOP):
        report.warn(f'unknown top-level field {key!r} (Studio ignores it; showcase fields go in "showcase")')
    if info.get('$schema') != SCHEMA:
        report.error(f'$schema: must be {SCHEMA}')
    if info.get('id') != sample_id:
        report.error(f'id: must equal the folder name {sample_id!r}, got {info.get("id")!r}')
    if not KEBAB.match(sample_id):
        report.error(f'folder name {sample_id!r} is not kebab-case')
    for key in ('displayName', 'description'):
        if not text(info.get(key)):
            report.error(f'{key}: required, a non-empty string')

    platforms = info.get('platforms')
    if platforms is not None:
        if not strings(platforms) or not platforms:
            report.error('platforms: a non-empty list of strings when present')
        else:
            for p in sorted(set(platforms) - PLATFORMS):
                report.error(f'platforms: {p!r} is not one of {sorted(PLATFORMS)}')
            if len(set(platforms)) != len(platforms):
                report.error('platforms: listed twice')

    source = info.get('sourcePath')
    if not text(source) or not source.endswith('.csproj'):
        report.error('sourcePath: required, the path to the .csproj')
    elif not (folder / source).is_file():
        report.error(f'sourcePath: {source} does not exist')

    if 'iconUrl' in info:
        icon = local_file(info['iconUrl'], folder, sample_id, report, 'iconUrl')
        if icon:
            if icon.stat().st_size > ICON_BYTES:
                report.error(f'iconUrl: {icon.stat().st_size // 1024} KB, over 200 KB')
            if images and (read := image_size(icon, report, 'iconUrl')):
                size, fmt = read
                if size != ICON_SIZE:
                    report.error(f'iconUrl: {size[0]}x{size[1]}, must be 256x256')
                if fmt not in ('PNG', 'WEBP'):
                    report.error(f'iconUrl: {fmt}, must be PNG or WebP')
    else:
        report.warn('iconUrl: missing; the gallery card shows no icon (README, Adding a sample, step 2)')

    screenshots = info.get('screenshots', [])
    if not isinstance(screenshots, list):
        report.error('screenshots: a list of URLs')
        screenshots = []
    if not screenshots:
        report.warn('screenshots: none; add at least one (README, Adding a sample, step 2)')
    if len(screenshots) > 10:
        report.error(f'screenshots: {len(screenshots)}, at most 10')
    for i, url in enumerate(screenshots):
        shot = local_file(url, folder, sample_id, report, f'screenshots[{i}]')
        if shot and images and (read := image_size(shot, report, f'screenshots[{i}]')):
            size, _ = read
            if size not in SCREEN_SIZES:
                report.error(f'screenshots[{i}]: {shot.name} is {size[0]}x{size[1]}, must be 1280x800 or 390x844')
    return platforms


def check_showcase(info, platforms, sdk, report):
    showcase = info.get('showcase')
    if not isinstance(showcase, dict):
        report.error('showcase: required, an object')
        return
    for key in sorted(set(showcase) - SHOWCASE):
        report.warn(f'showcase: unknown field {key!r}')
    if not text(showcase.get('summary')):
        report.error('showcase.summary: required, one sentence')
    elif len(showcase['summary']) > 160:
        report.warn(f'showcase.summary: {len(showcase["summary"])} characters; the card fits about 160')
    if showcase.get('type') not in TYPES:
        report.error(f'showcase.type: one of {sorted(TYPES)}')
    status = showcase.get('status')
    if status not in STATUSES:
        report.error(f'showcase.status: one of {sorted(STATUSES)}')
    owner = showcase.get('owner')
    if not strings(owner) or not owner:
        report.error('showcase.owner: required, a list of GitHub handles')
    else:
        for handle in owner:
            if not HANDLE.match(handle.lstrip('@')):
                report.error(f'showcase.owner: {handle!r} is not a GitHub handle')
    goals = showcase.get('goals')
    if not strings(goals) or not 2 <= len(goals) <= 6:
        report.error('showcase.goals: required, two to six lines')
    for key in ('requires', 'tags'):
        if key in showcase and not strings(showcase[key]):
            report.error(f'showcase.{key}: a list of strings')
    for tag in showcase.get('tags') or []:
        if isinstance(tag, str) and not KEBAB.match(tag):
            report.warn(f'showcase.tags: {tag!r} is not lowercase kebab-case')
    if 'industry' in showcase and not text(showcase['industry']):
        report.error('showcase.industry: a non-empty string when present')
    if 'liveUrl' in showcase and not (text(showcase['liveUrl']) and showcase['liveUrl'].startswith('https://')):
        report.error('showcase.liveUrl: an absolute https URL')
    if showcase.get('type') == 'showcase' and platforms is not None and 'wasm' not in platforms:
        report.error('platforms: a showcase sample lists wasm (the live demo is the web build)')
    if sdk and status in ('active', 'preview'):
        prerelease = '-' in sdk
        if prerelease and status == 'active':
            report.error(f'showcase.status: global.json pins the preview Uno.Sdk {sdk}; status is preview')
        if not prerelease and status == 'preview':
            report.warn(f'showcase.status: preview, but global.json pins the stable Uno.Sdk {sdk}')


def check_companions(studiolive, report):
    # Studio needs the messages list; agent-context.json is only useful with at least one message, while
    # user-conversation.json may start empty and carry only suggestedPrompts.
    for name, may_be_empty in (('agent-context.json', False), ('user-conversation.json', True)):
        path = studiolive / name
        if not path.is_file():
            report.warn(f'{name}: missing (optional; the sample opens in Studio without context)')
            continue
        data = read_json_object(path, name, report)
        if data is None:
            continue
        messages = data.get('messages')
        if not isinstance(messages, list) or (not messages and not may_be_empty):
            report.error(f'{name}: messages must be a {"list" if may_be_empty else "non-empty list"}')
            continue
        for i, m in enumerate(messages):
            if not isinstance(m, dict) or m.get('role') not in ('user', 'assistant', 'system') or not text(m.get('content')):
                report.error(f'{name}: messages[{i}] needs a role (user, assistant, system) and content')
        if name == 'user-conversation.json' and 'suggestedPrompts' in data and not strings(data['suggestedPrompts']):
            report.error(f'{name}: suggestedPrompts must be a list of strings')


def uno_sdk(folder, report):
    path = folder / 'global.json'
    if not path.is_file():
        report.error('global.json: missing (it pins Uno.Sdk)')
        return None
    data = read_json_object(path, 'global.json', report, strip_comments=True)
    if data is None:
        return None
    sdks = data.get('msbuild-sdks')
    sdk = sdks.get('Uno.Sdk') if isinstance(sdks, dict) else None
    if not text(sdk):
        report.error('global.json: no msbuild-sdks.Uno.Sdk version')
        return None
    return sdk


def check(folder, images):
    report = Report(folder.name)
    studiolive = folder / '.studiolive'
    path = studiolive / 'sample.json'
    if not path.is_file():
        report.error('.studiolive/sample.json: missing')
        return report, None
    info = read_json_object(path, '.studiolive/sample.json', report)
    if info is None:
        return report, None
    sdk = uno_sdk(folder, report)
    platforms = check_info(folder, info, report, images)
    check_showcase(info, platforms, sdk, report)
    check_companions(studiolive, report)
    if not (folder / 'README.md').is_file():
        report.error('README.md: missing')
    if not list(folder.glob('*.sln')) and not list(folder.glob('*.slnx')):
        report.error('no solution file at the sample root')
    if (folder / 'uno-sample.json').is_file():
        report.warn('uno-sample.json: the legacy manifest; .studiolive/sample.json replaces it')
    for name in ignored(studiolive):
        report.error(f'.studiolive/{name}: ignored by git, so it would never be committed')
    return report, sdk


def ignored(studiolive):
    """Files in .studiolive that git ignores (none when the folder is not in a git work tree)."""
    files = [p.relative_to(studiolive).as_posix() for p in studiolive.rglob('*') if p.is_file()]
    if not files:
        return []
    try:
        result = subprocess.run(['git', 'check-ignore', '--no-index', '--stdin'], cwd=studiolive, input=chr(10).join(files),
                                capture_output=True, text=True)
    except FileNotFoundError:
        return []
    return [line for line in result.stdout.splitlines() if line]


def main():
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[1] / 'studio')
    parser.add_argument('--no-images', action='store_true', help='skip the icon and screenshot size checks (no Pillow needed)')
    parser.add_argument('ids', nargs='*')
    args = parser.parse_args()
    if not args.root.is_dir():
        print(f'E --root: {args.root} is not a folder')
        return 1
    if Image is None and not args.no_images:
        print('E Pillow is not installed, so the icon and screenshot sizes cannot be checked.')
        print('  Install it (pip install pillow), or pass --no-images to skip those checks on purpose.')
        return 1
    images = not args.no_images
    folders = [args.root / i for i in args.ids] if args.ids else sorted(
        p for p in args.root.iterdir() if (p / '.studiolive' / 'sample.json').is_file())
    if not folders:
        print(f'No samples with .studiolive/sample.json under {args.root}')
        return 1
    if not images:
        print('W --no-images: icon and screenshot sizes not checked')
    failed = 0
    for folder in folders:
        if not folder.is_dir():
            print(f'FAIL {folder.name}  (no such folder under {args.root})')
            failed += 1
            continue
        report, sdk = check(folder, images)
        state = 'FAIL' if report.errors else 'OK'
        print(f'{state:4} {folder.name}  (Uno.Sdk {sdk or "?"}, {len(report.errors)} errors, {len(report.warnings)} warnings)')
        for e in report.errors:
            print(f'  E {e}')
        for w in report.warnings:
            print(f'  W {w}')
        failed += bool(report.errors)
    return 1 if failed else 0


if __name__ == '__main__':
    sys.exit(main())
