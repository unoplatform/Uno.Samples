"""Check a showcase sample in studio/<sample-id>/ against the layout and info file in studio/README.md.

Usage:
  python -I build/validate_studio_sample.py [--root <folder>] [<sample-id> ...]

With no ids it checks every folder in studio/ that has .studiolive/sample.json. --root checks samples outside this repo
(a staging folder with one sample per subfolder). Prints one line per problem and exits 1 if any sample has an error.
Warnings (W) do not fail the run.

Needs Pillow for the icon and screenshot sizes (pip install pillow); without it the image checks are skipped.
"""
import argparse
import json
import re
import subprocess
import sys
from pathlib import Path

try:
    from PIL import Image
except ImportError:
    Image = None

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
HANDLE = re.compile(r'^[A-Za-z0-9](?:[A-Za-z0-9-]{0,38})$')


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


def local_file(url, folder, sample_id, report, field):
    """The file in the sample folder that an absolute raw.githubusercontent URL points at."""
    if not text(url) or not url.startswith('https://'):
        report.error(f'{field}: must be an absolute https URL, got {url!r}')
        return None
    prefix = RAW + sample_id + '/'
    if not url.startswith(prefix):
        report.error(f'{field}: must start with {prefix}')
        return None
    path = folder / url[len(prefix):]
    if not path.is_file():
        report.error(f'{field}: {url[len(prefix):]} is not in the sample folder')
        return None
    return path


def image_size(path):
    with Image.open(path) as image:
        return image.size, image.format


def check_info(folder, info, report):
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
            if Image:
                size, fmt = image_size(icon)
                if size != ICON_SIZE:
                    report.error(f'iconUrl: {size[0]}x{size[1]}, must be 256x256')
                if fmt not in ('PNG', 'WEBP'):
                    report.error(f'iconUrl: {fmt}, must be PNG or WebP')

    screenshots = info.get('screenshots', [])
    if not isinstance(screenshots, list):
        report.error('screenshots: a list of URLs')
        screenshots = []
    if len(screenshots) > 10:
        report.error(f'screenshots: {len(screenshots)}, at most 10')
    for i, url in enumerate(screenshots):
        shot = local_file(url, folder, sample_id, report, f'screenshots[{i}]')
        if shot and Image:
            size, _ = image_size(shot)
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
    for name, keys in (('agent-context.json', ('messages',)), ('user-conversation.json', ('messages', 'suggestedPrompts'))):
        path = studiolive / name
        if not path.is_file():
            report.warn(f'{name}: missing (optional; the sample opens in Studio without context)')
            continue
        try:
            data = json.loads(path.read_text(encoding='utf-8'))
        except ValueError as e:
            report.error(f'{name}: not valid JSON ({e})')
            continue
        messages = data.get('messages')
        if not isinstance(messages, list) or not messages:
            report.error(f'{name}: messages must be a non-empty list')
            continue
        for i, m in enumerate(messages):
            if not isinstance(m, dict) or m.get('role') not in ('user', 'assistant', 'system') or not text(m.get('content')):
                report.error(f'{name}: messages[{i}] needs a role (user, assistant, system) and content')
        if 'suggestedPrompts' in keys and 'suggestedPrompts' in data and not strings(data['suggestedPrompts']):
            report.error(f'{name}: suggestedPrompts must be a list of strings')


def uno_sdk(folder, report):
    path = folder / 'global.json'
    if not path.is_file():
        report.error('global.json: missing (it pins Uno.Sdk)')
        return None
    try:
        # dotnet reads global.json with // and /* */ comments. Strip whole comment lines and blocks only, so a URL in a
        # value stays intact.
        raw = re.sub(r'/\*[\s\S]*?\*/|^[ \t]*//[^\n]*', '', path.read_text(encoding='utf-8'), flags=re.M)
        sdk = json.loads(raw).get('msbuild-sdks', {}).get('Uno.Sdk')
    except ValueError as e:
        report.error(f'global.json: not valid JSON ({e})')
        return None
    if not text(sdk):
        report.error('global.json: no msbuild-sdks.Uno.Sdk version')
        return None
    return sdk


def check(folder):
    report = Report(folder.name)
    studiolive = folder / '.studiolive'
    path = studiolive / 'sample.json'
    try:
        info = json.loads(path.read_text(encoding='utf-8'))
    except FileNotFoundError:
        report.error('.studiolive/sample.json: missing')
        return report, None
    except ValueError as e:
        report.error(f'.studiolive/sample.json: not valid JSON ({e})')
        return report, None
    sdk = uno_sdk(folder, report)
    platforms = check_info(folder, info, report)
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
    parser.add_argument('ids', nargs='*')
    args = parser.parse_args()
    folders = [args.root / i for i in args.ids] if args.ids else sorted(
        p for p in args.root.iterdir() if (p / '.studiolive' / 'sample.json').is_file())
    if not folders:
        print(f'No samples with .studiolive/sample.json under {args.root}')
        return 1
    if not Image:
        print('W Pillow is not installed: icon and screenshot sizes not checked')
    failed = 0
    for folder in folders:
        report, sdk = check(folder)
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
