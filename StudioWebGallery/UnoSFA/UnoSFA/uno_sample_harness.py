#!/usr/bin/env python3
"""Uno app validation harness.

This harness builds and smoke-checks a Uno app across TFMs and runs static audits
for broadly useful checks (icons, localization, feature usage, navigation,
responsive breakpoints, and UI heuristics).
"""

from __future__ import annotations

import argparse
import datetime as dt
import html
import json
import os
import re
import shutil
import socket
import subprocess
import sys
import tempfile
import time
import hashlib
import urllib.request
from pathlib import Path
from typing import Any, Dict, List, Optional, Set, Tuple


Status = str  # PASS | FAIL | WARN | MANUAL | SKIP
VALID_STATUSES = {"PASS", "FAIL", "WARN", "MANUAL", "SKIP"}
DEFAULT_RESPONSIVE_MARKERS = [
    "ResponsiveExtension",
    "ResponsiveView",
    "AdaptiveTrigger",
    "StateTrigger",
    "VisualStateManager.VisualStateGroups",
]


def collect_project_files(root: Path, pattern: str) -> List[Path]:
    files = sorted(root.rglob(pattern))
    return [
        p
        for p in files
        if "bin" not in p.parts and "obj" not in p.parts and ".git" not in p.parts
    ]


def try_read_text(path: Path) -> str:
    if not path.exists():
        return ""
    return path.read_text(encoding="utf-8", errors="ignore")


def run_cmd(cmd: List[str], cwd: Path, timeout_sec: int = 600) -> Dict[str, object]:
    started = time.time()
    proc = subprocess.run(
        cmd,
        cwd=str(cwd),
        capture_output=True,
        text=True,
        timeout=timeout_sec,
    )
    return {
        "cmd": " ".join(cmd),
        "rc": proc.returncode,
        "stdout": proc.stdout,
        "stderr": proc.stderr,
        "duration_sec": round(time.time() - started, 2),
    }


def run_cmd_with_env(
    cmd: List[str],
    cwd: Path,
    timeout_sec: int = 600,
    env_overrides: Optional[Dict[str, str]] = None,
) -> Dict[str, object]:
    started = time.time()
    env = os.environ.copy()
    if env_overrides:
        env.update(env_overrides)

    proc = subprocess.run(
        cmd,
        cwd=str(cwd),
        capture_output=True,
        text=True,
        timeout=timeout_sec,
        env=env,
    )
    return {
        "cmd": " ".join(cmd),
        "rc": proc.returncode,
        "stdout": proc.stdout,
        "stderr": proc.stderr,
        "duration_sec": round(time.time() - started, 2),
    }


def run_probe_with_timeout(
    cmd: List[str],
    cwd: Path,
    success_pattern: str,
    timeout_sec: int,
) -> Dict[str, object]:
    started = time.time()
    pattern = re.compile(success_pattern)
    timed_out = False

    try:
        proc = subprocess.run(
            cmd,
            cwd=str(cwd),
            capture_output=True,
            text=True,
            timeout=timeout_sec,
        )
        output = (proc.stdout or "") + "\n" + (proc.stderr or "")
        rc = proc.returncode
    except subprocess.TimeoutExpired as ex:
        timed_out = True
        ex_stdout = ex.stdout or ""
        ex_stderr = ex.stderr or ""
        if isinstance(ex_stdout, bytes):
            ex_stdout = ex_stdout.decode("utf-8", errors="ignore")
        if isinstance(ex_stderr, bytes):
            ex_stderr = ex_stderr.decode("utf-8", errors="ignore")
        output = ex_stdout + "\n" + ex_stderr
        rc = None

    found = bool(pattern.search(output))
    duration = round(time.time() - started, 2)
    return {
        "cmd": " ".join(cmd),
        "rc": rc,
        "duration_sec": duration,
        "found": found,
        "timed_out": timed_out,
        "output": "\n".join(output.splitlines()[-120:]),
    }


def run_probe_with_timeout_and_env(
    cmd: List[str],
    cwd: Path,
    success_pattern: str,
    timeout_sec: int,
    env_overrides: Optional[Dict[str, str]] = None,
) -> Dict[str, object]:
    started = time.time()
    pattern = re.compile(success_pattern)
    timed_out = False
    env = os.environ.copy()
    if env_overrides:
        env.update(env_overrides)

    try:
        proc = subprocess.run(
            cmd,
            cwd=str(cwd),
            capture_output=True,
            text=True,
            timeout=timeout_sec,
            env=env,
        )
        output = (proc.stdout or "") + "\n" + (proc.stderr or "")
        rc = proc.returncode
    except subprocess.TimeoutExpired as ex:
        timed_out = True
        ex_stdout = ex.stdout or ""
        ex_stderr = ex.stderr or ""
        if isinstance(ex_stdout, bytes):
            ex_stdout = ex_stdout.decode("utf-8", errors="ignore")
        if isinstance(ex_stderr, bytes):
            ex_stderr = ex_stderr.decode("utf-8", errors="ignore")
        output = ex_stdout + "\n" + ex_stderr
        rc = None

    found = bool(pattern.search(output))
    duration = round(time.time() - started, 2)
    return {
        "cmd": " ".join(cmd),
        "rc": rc,
        "duration_sec": duration,
        "found": found,
        "timed_out": timed_out,
        "output": "\n".join(output.splitlines()[-200:]),
    }


def probe_http(url: str, timeout_sec: int = 5) -> bool:
    try:
        request = urllib.request.Request(url, method="GET")
        with urllib.request.urlopen(request, timeout=timeout_sec) as response:
            return int(getattr(response, "status", 200)) < 400
    except Exception:
        return False


def find_available_tcp_port() -> int:
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
        sock.bind(("127.0.0.1", 0))
        return int(sock.getsockname()[1])


def parse_resw_keys(text: str) -> Set[str]:
    keys = set()
    for match in re.finditer(r'<data\s+name="([^"]+)"', text, flags=re.IGNORECASE):
        keys.add(match.group(1))
    return keys


def markdown_table_cell(value: Any) -> str:
    return str(value).replace("|", "\\|").replace("\n", "<br>")


def playwright_available() -> bool:
    return shutil.which("node") is not None


def run_wasm_browser_diagnostics(
    base_url: str,
    timeout_sec: int,
    console_fail_patterns: List[str],
) -> Dict[str, object]:
    if not playwright_available():
        return {
            "status": "SKIP",
            "details": "Node.js is not available; skipped browser diagnostics.",
            "category": "environment",
        }

    script = """
const fs = require('fs');
const { chromium } = require('playwright');

async function main() {
  const baseUrl = process.argv[2];
  const timeoutMs = Number(process.argv[3]);
  const patterns = JSON.parse(process.argv[4] || '[]').map(x => new RegExp(x, 'i'));

  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage();
  const consoleErrors = [];
  const requestFailures = [];

  page.on('console', msg => {
    if (msg.type() === 'error') {
      consoleErrors.push(msg.text());
    }
  });

  page.on('requestfailed', req => {
    requestFailures.push(`${req.method()} ${req.url()} :: ${req.failure() ? req.failure().errorText : 'failed'}`);
  });

  try {
    await page.goto(baseUrl, { waitUntil: 'networkidle', timeout: timeoutMs });
  } catch (e) {
    consoleErrors.push(`Navigation failed: ${String(e)}`);
  }

  await page.waitForTimeout(1200);

  const matchedConsole = consoleErrors.filter(e => patterns.some(p => p.test(e)));
  const ok = matchedConsole.length === 0 && requestFailures.length === 0;

  const payload = {
    ok,
    baseUrl,
    consoleErrors,
    matchedConsole,
    requestFailures
  };
  process.stdout.write(JSON.stringify(payload));
  await browser.close();
}

main().catch(err => {
  process.stdout.write(JSON.stringify({ ok: false, fatal: String(err), consoleErrors: [], matchedConsole: [], requestFailures: [] }));
  process.exit(0);
});
"""

    with tempfile.NamedTemporaryFile("w", suffix=".js", delete=False, encoding="utf-8") as tf:
        tf.write(script)
        script_path = Path(tf.name)

    try:
        result = run_cmd(
            [
                "node",
                str(script_path),
                base_url,
                str(max(1000, timeout_sec * 1000)),
                json.dumps(console_fail_patterns),
            ],
            cwd=Path.cwd(),
            timeout_sec=max(30, timeout_sec + 20),
        )
        if int(result.get("rc", 1)) != 0:
            stderr = str(result.get("stderr", ""))
            if "Cannot find module 'playwright'" in stderr:
                return {
                    "status": "SKIP",
                    "details": "Playwright package is not installed; skipped browser diagnostics.",
                    "category": "environment",
                }
            return {
                "status": "WARN",
                "details": "Browser diagnostics command failed: " + format_log_snippet(stderr, max_lines=12),
                "category": "environment",
            }

        raw = str(result.get("stdout", "")).strip()
        parsed = json.loads(raw) if raw else {}
    except Exception as ex:
        return {
            "status": "WARN",
            "details": f"Browser diagnostics failed to execute: {ex}",
            "category": "environment",
        }
    finally:
        try:
            script_path.unlink(missing_ok=True)
        except Exception:
            pass

    if not isinstance(parsed, dict):
        return {
            "status": "WARN",
            "details": "Browser diagnostics produced invalid output.",
            "category": "environment",
        }

    if parsed.get("ok"):
        return {
            "status": "PASS",
            "details": "No failed requests or matching console errors detected in WASM browser diagnostics.",
            "category": "product",
        }

    errors = []
    matched_console = parsed.get("matchedConsole") or []
    request_failures = parsed.get("requestFailures") or []
    fatal = parsed.get("fatal")
    if fatal:
        errors.append(f"fatal={fatal}")
    if matched_console:
        errors.append("console=" + " | ".join(str(x) for x in matched_console[:6]))
    if request_failures:
        errors.append("requests=" + " | ".join(str(x) for x in request_failures[:6]))

    return {
        "status": "FAIL",
        "details": "WASM browser diagnostics detected issues: " + "; ".join(errors) if errors else "WASM browser diagnostics detected issues.",
        "category": "product",
    }


def run_visual_regression_snapshots(
    base_url: str,
    report_dir: Path,
    config: Dict[str, Any],
) -> Dict[str, str]:
    visual_cfg = config.get("visual_regression", {}) if isinstance(config.get("visual_regression", {}), dict) else {}
    if not bool(visual_cfg.get("enabled", False)):
        return {
            "check": "Visual regression snapshots",
            "status": "SKIP",
            "details": "Visual regression is disabled in harness config.",
            "category": "product",
        }

    if not playwright_available():
        return {
            "check": "Visual regression snapshots",
            "status": "SKIP",
            "details": "Node.js is not available; skipped visual regression snapshots.",
            "category": "environment",
        }

    routes = visual_cfg.get("routes", ["/Main/Home", "/Main/Menu", "/Main/Cart", "/Main/Orders"])
    if not isinstance(routes, list) or not all(isinstance(r, str) for r in routes):
        routes = ["/Main/Home", "/Main/Menu", "/Main/Cart", "/Main/Orders"]

    baseline_dir = Path(str(visual_cfg.get("baseline_dir", report_dir / "visual-baseline"))).resolve()
    current_dir = report_dir / "visual-current"
    current_dir.mkdir(parents=True, exist_ok=True)
    baseline_dir.mkdir(parents=True, exist_ok=True)

    script = """
const fs = require('fs');
const path = require('path');
const { chromium } = require('playwright');

async function main() {
  const baseUrl = process.argv[2];
  const outDir = process.argv[3];
  const routes = JSON.parse(process.argv[4]);

  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage({ viewport: { width: 1280, height: 800 } });

  const generated = [];
  for (const route of routes) {
    const url = new URL(route, baseUrl).toString();
    await page.goto(url, { waitUntil: 'networkidle', timeout: 20000 });
    await page.waitForTimeout(1000);
    const slug = route.replace(/[^a-zA-Z0-9]/g, '_').replace(/^_+/, '') || 'root';
    const filename = `${slug}.png`;
    const target = path.join(outDir, filename);
    await page.screenshot({ path: target, fullPage: true });
    generated.push(filename);
  }

  await browser.close();
  process.stdout.write(JSON.stringify({ ok: true, generated }));
}

main().catch(err => {
  process.stdout.write(JSON.stringify({ ok: false, error: String(err), generated: [] }));
  process.exit(0);
});
"""

    with tempfile.NamedTemporaryFile("w", suffix=".js", delete=False, encoding="utf-8") as tf:
        tf.write(script)
        script_path = Path(tf.name)

    try:
        result = run_cmd(
            ["node", str(script_path), base_url, str(current_dir), json.dumps(routes)],
            cwd=Path.cwd(),
            timeout_sec=180,
        )
        if int(result.get("rc", 1)) != 0:
            stderr = str(result.get("stderr", ""))
            if "Cannot find module 'playwright'" in stderr:
                return {
                    "check": "Visual regression snapshots",
                    "status": "SKIP",
                    "details": "Playwright package is not installed; skipped visual regression snapshots.",
                    "category": "environment",
                }
            return {
                "check": "Visual regression snapshots",
                "status": "WARN",
                "details": "Visual snapshot command failed: " + format_log_snippet(stderr, max_lines=12),
                "category": "environment",
            }

        raw = str(result.get("stdout", "")).strip()
        parsed = json.loads(raw) if raw else {}
    except Exception as ex:
        return {
            "check": "Visual regression snapshots",
            "status": "WARN",
            "details": f"Visual snapshot execution failed: {ex}",
            "category": "environment",
        }
    finally:
        try:
            script_path.unlink(missing_ok=True)
        except Exception:
            pass

    if not isinstance(parsed, dict) or not parsed.get("ok"):
        return {
            "check": "Visual regression snapshots",
            "status": "FAIL",
            "details": f"Visual snapshot capture failed: {parsed.get('error', 'unknown error') if isinstance(parsed, dict) else 'invalid output'}",
            "category": "product",
        }

    generated = [str(x) for x in parsed.get("generated", [])]
    if not generated:
        return {
            "check": "Visual regression snapshots",
            "status": "WARN",
            "details": "No visual snapshots were generated.",
            "category": "product",
        }

    missing_baselines: List[str] = []
    changed: List[str] = []
    for name in generated:
        current_file = current_dir / name
        baseline_file = baseline_dir / name
        if not baseline_file.exists():
            missing_baselines.append(name)
            continue

        current_hash = hashlib.sha256(current_file.read_bytes()).hexdigest()
        baseline_hash = hashlib.sha256(baseline_file.read_bytes()).hexdigest()
        if current_hash != baseline_hash:
            changed.append(name)

    if missing_baselines:
        return {
            "check": "Visual regression snapshots",
            "status": "WARN",
            "details": "Baseline images missing for: " + ", ".join(missing_baselines) + f". Current captures stored in {current_dir}.",
            "category": "product",
        }

    if changed:
        return {
            "check": "Visual regression snapshots",
            "status": "FAIL",
            "details": "Visual differences detected in: " + ", ".join(changed),
            "category": "product",
        }

    return {
        "check": "Visual regression snapshots",
        "status": "PASS",
        "details": "All configured snapshot routes match baseline image hashes.",
        "category": "product",
    }


def grep_count(pattern: str, text: str) -> int:
    return len(re.findall(pattern, text, flags=re.MULTILINE))


def load_harness_config(config_path: Optional[Path]) -> Tuple[Dict[str, Any], List[str], str]:
    if config_path is None:
        return {}, [], "none"

    if not config_path.exists():
        return {}, [f"Config file not found: {config_path}"], str(config_path)

    try:
        parsed = json.loads(try_read_text(config_path))
    except json.JSONDecodeError as ex:
        return {}, [f"Invalid JSON in config file {config_path}: {ex}"], str(config_path)

    if not isinstance(parsed, dict):
        return {}, [f"Config root must be a JSON object: {config_path}"], str(config_path)

    return parsed, [], str(config_path)


def parse_positive_int(value: Any, default: int) -> int:
    try:
        parsed = int(value)
        return parsed if parsed > 0 else default
    except (TypeError, ValueError):
        return default


def find_default_project_file(script_path: Path) -> Optional[Path]:
    legacy = script_path.parent.parent / "app" / "BrewHouse" / "BrewHouse.csproj"
    if legacy.exists():
        return legacy

    cwd_candidates = collect_project_files(Path.cwd(), "*.csproj")
    if len(cwd_candidates) == 1:
        return cwd_candidates[0]

    if len(cwd_candidates) > 1:
        preview = "\n".join(f"- {candidate}" for candidate in cwd_candidates[:10])
        suffix = "" if len(cwd_candidates) <= 10 else f"\n- ... ({len(cwd_candidates) - 10} more)"
        raise ValueError(
            "Found multiple csproj files under current directory. Use --project-file to select one.\n"
            f"Candidates:\n{preview}{suffix}"
        )

    return None


def resolve_project_file(args: argparse.Namespace, script_path: Path) -> Path:
    if args.project_file:
        project_file = args.project_file.resolve()
        if not project_file.exists() or project_file.suffix.lower() != ".csproj":
            raise ValueError(f"Invalid --project-file: {project_file}")
        return project_file

    if args.project_root:
        root = args.project_root.resolve()
        if root.is_file() and root.suffix.lower() == ".csproj":
            return root
        if root.is_dir():
            candidates = collect_project_files(root, "*.csproj")
            if len(candidates) == 1:
                return candidates[0]
            if len(candidates) > 1:
                raise ValueError(
                    f"Found multiple csproj files under {root}. Use --project-file to select one."
                )
        raise ValueError(f"No project file found from --project-root: {root}")

    discovered = find_default_project_file(script_path)
    if discovered:
        return discovered

    raise ValueError("Could not auto-discover a project. Use --project-file or --project-root.")


def detect_tfms(csproj_path: Path) -> List[str]:
    text = try_read_text(csproj_path)

    match = re.search(r"<TargetFrameworks>([^<]+)</TargetFrameworks>", text)
    if match:
        return [tfm.strip() for tfm in match.group(1).split(";") if tfm.strip()]

    single = re.search(r"<TargetFramework>([^<]+)</TargetFramework>", text)
    if single:
        tfm = single.group(1).strip()
        return [tfm] if tfm else []

    return []


def check_java_version() -> Tuple[Status, str]:
    try:
        result = run_cmd(["java", "-version"], cwd=Path.cwd(), timeout_sec=30)
    except FileNotFoundError:
        return (
            "FAIL",
            "java not found in PATH. Android build requires Java 17+. "
            "Remediation (macOS/Homebrew): brew install openjdk@17 && export JAVA_HOME=$(/usr/libexec/java_home -v 17) && export PATH=\"$JAVA_HOME/bin:$PATH\". "
            "Verify with: java -version.",
        )

    output = (result["stderr"] or "") + "\n" + (result["stdout"] or "")
    m = re.search(r'"(\d+)(?:\.\d+)?', output)
    if not m:
        return (
            "WARN",
            "Could not parse java version from java -version output. "
            "Run java -version manually and ensure Java 17+ is active before Android builds.",
        )

    major = int(m.group(1))
    if major < 17:
        return (
            "FAIL",
            f"Java {major} detected. Android build needs Java 17+. "
            "Remediation (macOS/Homebrew): brew install openjdk@17 && export JAVA_HOME=$(/usr/libexec/java_home -v 17) && export PATH=\"$JAVA_HOME/bin:$PATH\". "
            "Verify with: java -version.",
        )
    return "PASS", f"Java {major} detected."


def classify_build_failure(tfm: str, output: str) -> Tuple[str, str]:
    if "android" in tfm:
        if re.search(r"UnsupportedClassVersionError|class file version|java error AMM0000", output):
            return "environment", "Android toolchain runtime mismatch (Java version)."

    if "ios" in tfm:
        if "Info.plist" in output and "DirectoryNotFoundException" in output:
            return "product", "Missing expected iOS Info.plist path in project layout."

    return "product", "General build failure; inspect build logs."


def audit_icons(project_root: Path) -> List[Dict[str, str]]:
    findings: List[Dict[str, str]] = []
    svg_files = collect_project_files(project_root, "*.svg")
    icon_svgs = [p for p in svg_files if "icon" in p.name.lower()]

    if icon_svgs:
        findings.append(
            {
                "check": "Icon asset signal",
                "status": "PASS",
                "details": f"Found icon-like SVG assets ({len(icon_svgs)}).",
                "category": "product",
            }
        )

        primary_icon = next((p for p in icon_svgs if p.name.lower() == "icon.svg"), icon_svgs[0])
        icon_text = try_read_text(primary_icon)
        has_path = "<path" in icon_text
        rect_count = icon_text.count("<rect")
        if not has_path and rect_count >= 1:
            findings.append(
                {
                    "check": "Base icon quality signal",
                    "status": "WARN",
                    "details": f"{primary_icon.name} appears mostly rectangular; verify platform icon quality.",
                    "category": "product",
                }
            )
        else:
            findings.append(
                {
                    "check": "Base icon quality signal",
                    "status": "PASS",
                    "details": f"{primary_icon.name} includes shape/path content beyond a flat background.",
                    "category": "product",
                }
            )
    else:
        findings.append(
            {
                "check": "Icon asset signal",
                "status": "WARN",
                "details": "No icon-like SVG assets were detected. Verify app icon assets are configured for all targets.",
                "category": "product",
            }
        )

    return findings


def audit_localization(project_root: Path) -> List[Dict[str, str]]:
    findings: List[Dict[str, str]] = []
    xaml_files = collect_project_files(project_root, "*.xaml")

    xuid_count = 0
    for xaml in xaml_files:
        xuid_count += grep_count(r"\bx:Uid=", try_read_text(xaml))

    if xuid_count > 0:
        status = "PASS" if xuid_count >= 20 else "WARN"
        detail = f"Detected {xuid_count} x:Uid usages across XAML files."
    else:
        status = "SKIP"
        detail = "No x:Uid usages detected. This app may not use x:Uid-based localization."

    findings.append(
        {
            "check": "x:Uid coverage",
            "status": status,
            "details": detail,
            "category": "product",
        }
    )

    resw_files = collect_project_files(project_root, "*.resw")
    if not resw_files:
        findings.append(
            {
                "check": "Resource file signal",
                "status": "SKIP",
                "details": "No .resw files detected under project root.",
                "category": "product",
            }
        )
    else:
        locale_keys: Dict[str, set[str]] = {}
        per_locale_counts: Dict[str, int] = {}
        for path in resw_files:
            locale = path.parent.name
            text = try_read_text(path)
            per_locale_counts[locale] = grep_count(r"<data name=", text)
            locale_keys[locale] = parse_resw_keys(text)
        findings.append(
            {
                "check": "Resource file signal",
                "status": "PASS",
                "details": "Detected .resw files with key counts: "
                + ", ".join(f"{k}={v}" for k, v in sorted(per_locale_counts.items())),
                "category": "product",
            }
        )

        if locale_keys:
            reference_locale = sorted(locale_keys.keys())[0]
            reference_keys = locale_keys[reference_locale]
            parity_issues: List[str] = []
            for locale in sorted(locale_keys.keys()):
                if locale == reference_locale:
                    continue
                missing = sorted(reference_keys - locale_keys[locale])
                extra = sorted(locale_keys[locale] - reference_keys)
                if missing or extra:
                    issue_parts = []
                    if missing:
                        issue_parts.append(f"missing={','.join(missing[:8])}")
                    if extra:
                        issue_parts.append(f"extra={','.join(extra[:8])}")
                    parity_issues.append(f"{locale} ({'; '.join(issue_parts)})")

            if parity_issues:
                findings.append(
                    {
                        "check": "Localization key parity",
                        "status": "FAIL",
                        "details": f"Locale key mismatch against {reference_locale}: " + " | ".join(parity_issues),
                        "category": "product",
                    }
                )
            else:
                findings.append(
                    {
                        "check": "Localization key parity",
                        "status": "PASS",
                        "details": "All locale .resw files contain matching key sets.",
                        "category": "product",
                    }
                )

        # These dingbat arrows have repeatedly rendered as missing-glyph squares in WASM.
        high_risk_dingbats = {"➤", "➜", "➔", "➝", "➞", "➟", "➠", "➡"}
        glyph_issues: List[str] = []
        for path in resw_files:
            text = try_read_text(path)
            for match in re.finditer(
                r'<data\s+name="([^"]+\.Content)"[^>]*>\s*<value>(.*?)</value>\s*</data>',
                text,
                flags=re.IGNORECASE | re.DOTALL,
            ):
                key = match.group(1)
                value = html.unescape(match.group(2))
                found = sorted({ch for ch in value if ch in high_risk_dingbats})
                if found:
                    escaped = ", ".join(f"{ch} (U+{ord(ch):04X})" for ch in found)
                    glyph_issues.append(f"{path.parent.name}:{key} uses {escaped}")

        if glyph_issues:
            findings.append(
                {
                    "check": "Localized icon glyph portability",
                    "status": "FAIL",
                    "details": "High-risk dingbat glyphs detected in localized button labels: " + "; ".join(glyph_issues),
                    "category": "product",
                }
            )
        else:
            findings.append(
                {
                    "check": "Localized icon glyph portability",
                    "status": "PASS",
                    "details": "No high-risk dingbat glyphs found in localized *.Content resource values.",
                    "category": "product",
                }
            )

    return findings


def audit_feature_usage(project_root: Path) -> List[Dict[str, str]]:
    findings: List[Dict[str, str]] = []

    all_cs_files = collect_project_files(project_root, "*.cs")
    all_cs = "\n".join(try_read_text(p) for p in all_cs_files)

    all_xaml_files = collect_project_files(project_root, "*.xaml")
    all_xaml = "\n".join(try_read_text(p) for p in all_xaml_files)

    app_xaml_cs = project_root / "App.xaml.cs"
    app_cs = try_read_text(app_xaml_cs)

    has_theme_declared = "ThemeService" in app_cs
    if has_theme_declared:
        if re.search(r"\bIThemeService\b", all_cs) and re.search(r"\bSetThemeAsync\b", all_cs):
            findings.append(
                {
                    "check": "ThemeService runtime usage",
                    "status": "PASS",
                    "details": "ThemeService appears declared and used.",
                    "category": "product",
                }
            )
        else:
            findings.append(
                {
                    "check": "ThemeService runtime usage",
                    "status": "FAIL",
                    "details": "ThemeService appears declared but runtime usage was not detected.",
                    "category": "product",
                }
            )
    else:
        findings.append(
            {
                "check": "ThemeService runtime usage",
                "status": "SKIP",
                "details": "ThemeService declaration not detected in App.xaml.cs.",
                "category": "product",
            }
        )

    has_serialization_declared = "Serialization" in app_cs
    serialization_refs = re.findall(r"JsonSerializer|Serialize\(|Deserialize\(|System\.Text\.Json", all_cs)
    if has_serialization_declared:
        if serialization_refs:
            findings.append(
                {
                    "check": "Serialization runtime usage",
                    "status": "PASS",
                    "details": "Serialization feature declared and usage detected.",
                    "category": "product",
                }
            )
        else:
            findings.append(
                {
                    "check": "Serialization runtime usage",
                    "status": "FAIL",
                    "details": "Serialization appears declared but no concrete serialization flow was detected.",
                    "category": "product",
                }
            )
    else:
        findings.append(
            {
                "check": "Serialization runtime usage",
                "status": "SKIP",
                "details": "Serialization declaration not detected in App.xaml.cs.",
                "category": "product",
            }
        )

    has_config_registration = ".Section<" in app_cs or "IConfiguration" in app_cs
    has_config_consumption = bool(
        re.search(r"IOptions\s*<|IOptionsSnapshot\s*<|IConfiguration|GetSection\(", all_cs)
    )
    if has_config_registration:
        findings.append(
            {
                "check": "Configuration value consumption",
                "status": "PASS" if has_config_consumption else "WARN",
                "details": "Configuration registration and consumption detected."
                if has_config_consumption
                else "Configuration registration signal detected, but concrete runtime consumption was not found.",
                "category": "product",
            }
        )
    else:
        findings.append(
            {
                "check": "Configuration value consumption",
                "status": "SKIP",
                "details": "No explicit configuration registration signal detected in App.xaml.cs.",
                "category": "product",
            }
        )

    if re.search(r"FeedView|ProgressRing|VisualState", all_xaml):
        findings.append(
            {
                "check": "Async UX signal",
                "status": "PASS",
                "details": "Detected FeedView/ProgressRing/VisualState markers in XAML.",
                "category": "product",
            }
        )
    else:
        findings.append(
            {
                "check": "Async UX signal",
                "status": "WARN",
                "details": "No FeedView/ProgressRing/VisualState markers detected in XAML.",
                "category": "product",
            }
        )

    return findings


def audit_layout_and_navigation(project_root: Path, config: Dict[str, Any]) -> List[Dict[str, str]]:
    findings: List[Dict[str, str]] = []

    xaml_files = collect_project_files(project_root, "*.xaml")
    xaml_text = "\n".join(try_read_text(p) for p in xaml_files)

    has_nav_signal = any(marker in xaml_text for marker in ["Region.Name", "NavigationView", "TabBar", "Navigation.Request"])
    findings.append(
        {
            "check": "Navigation wiring signal",
            "status": "PASS" if has_nav_signal else "SKIP",
            "details": "Detected navigation wiring markers in XAML."
            if has_nav_signal
            else "No common navigation wiring markers were found in XAML.",
            "category": "product",
        }
    )

    app_xaml_cs = project_root / "App.xaml.cs"
    main_page_xaml = project_root / "Presentation" / "MainPage.xaml"
    app_xaml_cs_text = try_read_text(app_xaml_cs)
    main_page_xaml_text = try_read_text(main_page_xaml)

    if app_xaml_cs_text and main_page_xaml_text:
        route_names = {
            match
            for match in re.findall(r'new\s+RouteMap\("([^"]+)"', app_xaml_cs_text)
            if match
        }

        tab_items = re.findall(r"<utu:TabBarItem\b[\s\S]*?</utu:TabBarItem>", main_page_xaml_text, flags=re.IGNORECASE)
        tab_names: List[str] = []
        tabs_missing_back_navigation: List[str] = []

        for tab_item in tab_items:
            name_match = re.search(r'uen:Region\.Name="([^"]+)"', tab_item)
            tab_name = name_match.group(1) if name_match else "(unknown)"
            tab_names.append(tab_name)

            has_back_behavior = bool(
                re.search(
                    r'OnClickBehaviors="[^"]*BackNavigation[^"]*"',
                    tab_item,
                    flags=re.IGNORECASE,
                )
            )
            if not has_back_behavior:
                tabs_missing_back_navigation.append(tab_name)

        tab_name_set = {name for name in tab_names if name and name != "(unknown)"}
        unresolved_tabs = sorted(tab_name_set - route_names)

        has_home_tab = "Home" in tab_name_set
        has_home_route = "Home" in route_names

        if tabs_missing_back_navigation or unresolved_tabs:
            details = []
            if unresolved_tabs:
                details.append("Tab names missing matching routes: " + ", ".join(unresolved_tabs))
            if tabs_missing_back_navigation:
                details.append("Tabs missing BackNavigation behavior: " + ", ".join(tabs_missing_back_navigation))
            findings.append(
                {
                    "check": "Tab-route consistency signal",
                    "status": "FAIL",
                    "details": "; ".join(details),
                    "category": "product",
                }
            )
        elif has_home_tab and has_home_route and tab_name_set:
            findings.append(
                {
                    "check": "Tab-route consistency signal",
                    "status": "PASS",
                    "details": "All TabBar region names map to registered routes, BackNavigation is present on each tab, and Home tab/route are defined.",
                    "category": "product",
                }
            )
        else:
            findings.append(
                {
                    "check": "Tab-route consistency signal",
                    "status": "WARN",
                    "details": "Unable to confirm complete Home tab-route/back-navigation wiring.",
                    "category": "product",
                }
            )
    else:
        findings.append(
            {
                "check": "Tab-route consistency signal",
                "status": "SKIP",
                "details": "App.xaml.cs or Presentation/MainPage.xaml not found; skipped tab-route consistency check.",
                "category": "product",
            }
        )

    responsive_markers = config.get("responsive_markers", DEFAULT_RESPONSIVE_MARKERS)
    if not isinstance(responsive_markers, list) or not all(isinstance(marker, str) for marker in responsive_markers):
        responsive_markers = DEFAULT_RESPONSIVE_MARKERS
    has_breakpoint_signal = any(marker in xaml_text for marker in responsive_markers)
    findings.append(
        {
            "check": "Breakpoint/layout adaptation signals",
            "status": "PASS" if has_breakpoint_signal else "WARN",
            "details": "Detected responsive/adaptive XAML markers."
            if has_breakpoint_signal
            else "No explicit responsive/adaptive breakpoint markers found.",
            "category": "product",
        }
    )

    touch_target_config = config.get("touch_target", {})
    if not isinstance(touch_target_config, dict):
        touch_target_config = {}

    button_uid_regex = str(touch_target_config.get("button_uid_regex", r"Filter"))
    min_height = parse_positive_int(touch_target_config.get("min_height", 40), 40)
    try:
        filter_button_pattern = re.compile(
            rf"<Button\\b[^>]*x:Uid=\"[^\"]*(?:{button_uid_regex})[^\"]*\"[^>]*>",
            flags=re.IGNORECASE,
        )
    except re.error:
        filter_button_pattern = re.compile(
            r"<Button\\b[^>]*x:Uid=\"[^\"]*Filter[^\"]*\"[^>]*>",
            flags=re.IGNORECASE,
        )

    filter_button_snippets: List[str] = []
    for xaml in xaml_files:
        content = try_read_text(xaml)
        filter_button_snippets.extend(filter_button_pattern.findall(content))

    if not filter_button_snippets:
        findings.append(
            {
                "check": "Touch target heuristic",
                "status": "SKIP",
                "details": "No filter-like buttons detected for touch-target heuristic.",
                "category": "product",
            }
        )
    else:
        missing_or_small_touch_target: List[str] = []
        for snippet in filter_button_snippets:
            uid_match = re.search(r'x:Uid="([^"]+)"', snippet)
            uid = uid_match.group(1) if uid_match else "(unknown)"

            min_height_match = re.search(r'MinHeight="([0-9]+(?:\.[0-9]+)?)"', snippet)
            if not min_height_match:
                missing_or_small_touch_target.append(uid)
                continue

            try:
                value = float(min_height_match.group(1))
                if value < float(min_height):
                    missing_or_small_touch_target.append(f"{uid} (<{min_height})")
            except ValueError:
                missing_or_small_touch_target.append(uid)

        if missing_or_small_touch_target:
            findings.append(
                {
                    "check": "Touch target heuristic",
                    "status": "WARN",
                    "details": f"Filter-like buttons missing or below MinHeight {min_height}: " + ", ".join(missing_or_small_touch_target),
                    "category": "product",
                }
            )
        else:
            findings.append(
                {
                    "check": "Touch target heuristic",
                    "status": "PASS",
                    "details": "Filter-like buttons define explicit MinHeight values.",
                    "category": "product",
                }
            )

    cta_button_config = config.get("cta_buttons", {})
    if not isinstance(cta_button_config, dict):
        cta_button_config = {}

    cta_uid_regex = str(
        cta_button_config.get(
            "uid_regex",
            r"OrderNowButton|AddToCartButton|MenuAddToCartButton|AddButton",
        )
    )
    cta_min_height = parse_positive_int(cta_button_config.get("min_height", 34), 34)

    try:
        cta_pattern = re.compile(
            rf'<Button\b[^>]*x:Uid="([^"]*(?:{cta_uid_regex})[^"]*)"[^>]*>',
            flags=re.IGNORECASE,
        )
    except re.error:
        cta_pattern = re.compile(
            r'<Button\b[^>]*x:Uid="([^"]*(?:OrderNowButton|AddToCartButton|MenuAddToCartButton|AddButton)[^"]*)"[^>]*>',
            flags=re.IGNORECASE,
        )

    cta_snippets: List[Tuple[str, str]] = []
    for xaml in xaml_files:
        content = try_read_text(xaml)
        for match in cta_pattern.finditer(content):
            cta_snippets.append((match.group(1), match.group(0)))

    if not cta_snippets:
        findings.append(
            {
                "check": "CTA button alignment contract",
                "status": "SKIP",
                "details": "No configured CTA buttons were detected for alignment checks.",
                "category": "product",
            }
        )
    else:
        cta_issues: List[str] = []
        for uid, snippet in cta_snippets:
            missing: List[str] = []

            if not re.search(r'HorizontalContentAlignment="Center"', snippet, flags=re.IGNORECASE):
                missing.append("HorizontalContentAlignment=Center")
            if not re.search(r'VerticalContentAlignment="Center"', snippet, flags=re.IGNORECASE):
                missing.append("VerticalContentAlignment=Center")

            min_height_match = re.search(r'MinHeight="([0-9]+(?:\.[0-9]+)?)"', snippet, flags=re.IGNORECASE)
            if not min_height_match:
                missing.append(f"MinHeight>={cta_min_height}")
            else:
                try:
                    if float(min_height_match.group(1)) < float(cta_min_height):
                        missing.append(f"MinHeight>={cta_min_height}")
                except ValueError:
                    missing.append(f"MinHeight>={cta_min_height}")

            if missing:
                cta_issues.append(f"{uid} missing " + ", ".join(missing))

        if cta_issues:
            findings.append(
                {
                    "check": "CTA button alignment contract",
                    "status": "FAIL",
                    "details": "; ".join(cta_issues),
                    "category": "product",
                }
            )
        else:
            findings.append(
                {
                    "check": "CTA button alignment contract",
                    "status": "PASS",
                    "details": "CTA buttons define centered content alignment and explicit minimum heights.",
                    "category": "product",
                }
            )

    return findings


def audit_custom_checks(project_root: Path, config: Dict[str, Any]) -> List[Dict[str, str]]:
    findings: List[Dict[str, str]] = []

    custom_checks = config.get("custom_checks", [])
    if not isinstance(custom_checks, list):
        return findings

    for index, item in enumerate(custom_checks):
        if not isinstance(item, dict):
            continue

        name = str(item.get("name", f"custom-check-{index + 1}"))
        glob_pattern = str(item.get("glob", "*.xaml"))
        pattern = item.get("pattern")
        if not isinstance(pattern, str) or not pattern:
            findings.append(
                {
                    "check": name,
                    "status": "WARN",
                    "details": "Custom check skipped because 'pattern' is missing.",
                    "category": str(item.get("category", "product")),
                }
            )
            continue

        mode = str(item.get("mode", "any")).lower()
        if mode not in ("any", "all"):
            mode = "any"

        status_on_missing = str(item.get("status_on_missing", "WARN")).upper()
        if status_on_missing not in VALID_STATUSES:
            status_on_missing = "WARN"

        category = str(item.get("category", "product"))
        details_on_missing = str(item.get("details_on_missing", "Pattern not found."))
        details_on_pass = str(item.get("details_on_pass", "Pattern requirement satisfied."))

        files = collect_project_files(project_root, glob_pattern)
        if not files:
            findings.append(
                {
                    "check": name,
                    "status": status_on_missing,
                    "details": f"No files matched glob '{glob_pattern}'.",
                    "category": category,
                }
            )
            continue

        matched_files = 0
        for path in files:
            if re.search(pattern, try_read_text(path), flags=re.MULTILINE):
                matched_files += 1

        if mode == "all":
            passed = matched_files == len(files)
            missing = len(files) - matched_files
            miss_details = f"{details_on_missing} Missing matches in {missing} files."
        else:
            passed = matched_files > 0
            miss_details = details_on_missing

        findings.append(
            {
                "check": name,
                "status": "PASS" if passed else status_on_missing,
                "details": details_on_pass if passed else miss_details,
                "category": category,
            }
        )

    return findings


def apply_finding_config(findings: List[Dict[str, str]], config: Dict[str, Any]) -> List[Dict[str, str]]:
    disabled_checks_value = config.get("disabled_checks", [])
    disabled_checks: set[str] = set()
    if isinstance(disabled_checks_value, list):
        disabled_checks = {str(item) for item in disabled_checks_value if isinstance(item, str)}

    check_overrides = config.get("check_overrides", {})
    if not isinstance(check_overrides, dict):
        check_overrides = {}

    configured: List[Dict[str, str]] = []
    for finding in findings:
        current = dict(finding)
        check_name = current.get("check", "")

        if check_name in disabled_checks:
            current["status"] = "SKIP"
            current["details"] = "Disabled by harness configuration."

        override = check_overrides.get(check_name)
        if isinstance(override, dict):
            status_value = override.get("status")
            if isinstance(status_value, str):
                candidate = status_value.upper()
                if candidate in VALID_STATUSES:
                    current["status"] = candidate

            details_value = override.get("details")
            if isinstance(details_value, str) and details_value:
                current["details"] = details_value

            category_value = override.get("category")
            if isinstance(category_value, str) and category_value:
                current["category"] = category_value

        configured.append(current)

    return configured


def format_log_snippet(text: str, max_lines: int = 40) -> str:
    lines = text.splitlines()
    if len(lines) <= max_lines:
        return "\n".join(lines)
    return "\n".join(lines[-max_lines:])


def write_report(report_path: Path, content: str) -> None:
    report_path.parent.mkdir(parents=True, exist_ok=True)
    report_path.write_text(content, encoding="utf-8")


def write_json(report_path: Path, data: Dict[str, Any]) -> None:
    report_path.parent.mkdir(parents=True, exist_ok=True)
    report_path.write_text(json.dumps(data, indent=2), encoding="utf-8")


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate a Uno app across TFMs and static quality checks.")
    parser.add_argument("--project-file", type=Path, default=None, help="Path to target .csproj file.")
    parser.add_argument(
        "--project-root",
        type=Path,
        default=None,
        help="Path to app root or directory containing a single .csproj file.",
    )
    parser.add_argument("--output-dir", type=Path, default=None, help="Directory for generated reports.")
    parser.add_argument("--checklist", type=Path, default=None, help="Optional checklist markdown file.")
    parser.add_argument("--config", type=Path, default=None, help="Optional harness config JSON file.")
    parser.add_argument("--strict", action="store_true", help="Return non-zero if FAIL statuses are detected.")
    parser.add_argument("--skip-run", action="store_true", help="Skip runtime smoke checks.")
    args = parser.parse_args()

    script_path = Path(__file__).resolve()

    try:
        project_file = resolve_project_file(args, script_path)
    except ValueError as ex:
        print(f"ERROR: {ex}", file=sys.stderr)
        return 2

    project_root = project_file.parent

    now = dt.datetime.now()
    report_ts = now.strftime("%Y%m%d-%H%M%S")

    default_report_dir = project_root / "reports"
    report_dir = args.output_dir.resolve() if args.output_dir else default_report_dir

    report_path = report_dir / f"harness-report-{report_ts}.md"
    latest_path = report_dir / "harness-report-latest.md"
    report_json_path = report_dir / f"harness-report-{report_ts}.json"
    latest_json_path = report_dir / "harness-report-latest.json"

    checklist_path = args.checklist.resolve() if args.checklist else None
    if checklist_path is None:
        sibling_checklist = project_root.parent / "CHECKLIST.md"
        if sibling_checklist.exists():
            checklist_path = sibling_checklist

    config_path = args.config.resolve() if args.config else None
    if config_path is None:
        default_config = project_root / "harness.config.json"
        if default_config.exists():
            config_path = default_config

    config, config_warnings, config_source = load_harness_config(config_path)

    runtime_config = config.get("runtime", {}) if isinstance(config.get("runtime", {}), dict) else {}
    wasm_probe_url = str(runtime_config.get("wasm_probe_url", "http://localhost:5000/"))
    wasm_probe_timeout_sec = parse_positive_int(runtime_config.get("wasm_probe_timeout_sec", 5), 5)
    wasm_console_fail_patterns_value = runtime_config.get(
        "wasm_console_fail_patterns",
        [
            "Failed to load resource",
            "Unhandled",
            "TypeError",
            "ERR_ABORTED",
            "has not been loaded yet",
        ],
    )
    if isinstance(wasm_console_fail_patterns_value, list):
        wasm_console_fail_patterns = [str(x) for x in wasm_console_fail_patterns_value if str(x).strip()]
    else:
        wasm_console_fail_patterns = [
            "Failed to load resource",
            "Unhandled",
            "TypeError",
            "ERR_ABORTED",
            "has not been loaded yet",
        ]

    tfms = detect_tfms(project_file)
    if not tfms:
        print(f"ERROR: Could not detect TargetFramework(s) in {project_file}", file=sys.stderr)
        return 2

    preflight_results: List[Dict[str, object]] = []
    blocked_tfms: Dict[str, str] = {}

    for tfm in tfms:
        if "android" in tfm:
            status, detail = check_java_version()
            preflight_results.append(
                {
                    "tfm": tfm,
                    "status": status,
                    "category": "environment",
                    "details": detail,
                }
            )
            if status == "FAIL":
                blocked_tfms[tfm] = detail
        else:
            preflight_results.append(
                {
                    "tfm": tfm,
                    "status": "SKIP",
                    "category": "environment",
                    "details": "No preflight prerequisites configured for this TFM.",
                }
            )

    build_results: List[Dict[str, object]] = []
    for tfm in tfms:
        if tfm in blocked_tfms:
            build_results.append(
                {
                    "tfm": tfm,
                    "status": "SKIP",
                    "category": "environment",
                    "issue_reason": blocked_tfms[tfm],
                    "skip_reason": "Blocked by preflight environment failure.",
                    "cmd": f"dotnet build {project_file.name} -f {tfm} /clp:Summary",
                    "rc": None,
                    "stdout": "",
                    "stderr": "",
                    "duration_sec": 0.0,
                }
            )
            continue

        result = run_cmd(
            ["dotnet", "build", project_file.name, "-f", tfm, "/clp:Summary"],
            cwd=project_root,
            timeout_sec=1200,
        )
        output = str(result.get("stdout", "")) + "\n" + str(result.get("stderr", ""))
        status = "PASS" if int(result["rc"]) == 0 else "FAIL"
        category = "product"
        issue_reason = ""
        if status == "FAIL":
            category, issue_reason = classify_build_failure(tfm, output)

        build_results.append(
            {
                "tfm": tfm,
                "status": status,
                "category": category,
                "issue_reason": issue_reason,
                **result,
            }
        )

    build_by_tfm = {str(b["tfm"]): b for b in build_results}

    run_results: List[Dict[str, object]] = []
    findings_visual_runtime: List[Dict[str, str]] = []
    if not args.skip_run:
        for tfm in tfms:
            build_result = build_by_tfm.get(tfm, {})
            build_status = str(build_result.get("status", "SKIP"))

            if tfm in blocked_tfms:
                run_results.append(
                    {
                        "tfm": tfm,
                        "mode": "smoke",
                        "status": "SKIP",
                        "category": "environment",
                        "details": "Skipped runtime checks because preflight failed.",
                    }
                )
                continue

            if build_status != "PASS":
                run_results.append(
                    {
                        "tfm": tfm,
                        "mode": "smoke",
                        "status": "SKIP",
                        "category": str(build_result.get("category", "product")),
                        "details": "Skipped runtime checks because build did not pass.",
                    }
                )
                continue

            if "browserwasm" in tfm:
                dynamic_wasm_port = find_available_tcp_port()
                dynamic_wasm_url = f"http://127.0.0.1:{dynamic_wasm_port}/"
                probe = run_probe_with_timeout_and_env(
                    ["dotnet", "run", "--no-launch-profile", "-f", tfm, "--no-build"],
                    cwd=project_root,
                    success_pattern=rf"App url:\s+http://127\.0\.0\.1:{dynamic_wasm_port}/",
                    timeout_sec=90,
                    env_overrides={"ASPNETCORE_URLS": dynamic_wasm_url},
                )
                if not probe.get("found"):
                    output_text = str(probe.get("output", ""))
                    if "Address already in use" in output_text and probe_http(wasm_probe_url, timeout_sec=wasm_probe_timeout_sec):
                        probe["found"] = True
                        probe["details"] = f"App host already running on {wasm_probe_url}."
                        probe["resolved_wasm_url"] = wasm_probe_url
                    elif probe_http(dynamic_wasm_url, timeout_sec=wasm_probe_timeout_sec):
                        probe["found"] = True
                        probe["details"] = f"HTTP probe succeeded on dynamic URL {dynamic_wasm_url}."
                        probe["resolved_wasm_url"] = dynamic_wasm_url
                    elif probe_http(wasm_probe_url, timeout_sec=wasm_probe_timeout_sec):
                        probe["found"] = True
                        probe["details"] = f"HTTP probe succeeded on {wasm_probe_url}."
                        probe["resolved_wasm_url"] = wasm_probe_url
                else:
                    probe["resolved_wasm_url"] = dynamic_wasm_url

                status = "PASS" if probe.get("found") else ("FAIL" if probe.get("rc") not in (None, 0) else "WARN")
                details = str(probe.get("details", ""))
                if not details:
                    details = (
                        "Smoke probe observed expected startup signal."
                        if status == "PASS"
                        else "Startup signal not observed within timeout."
                    )

                if status == "PASS":
                    resolved_wasm_url = str(probe.get("resolved_wasm_url", dynamic_wasm_url))
                    browser_diag = run_wasm_browser_diagnostics(
                        base_url=resolved_wasm_url,
                        timeout_sec=20,
                        console_fail_patterns=wasm_console_fail_patterns,
                    )
                    diag_status = str(browser_diag.get("status", "SKIP"))
                    if diag_status in ("FAIL", "WARN"):
                        status = diag_status
                        details = str(browser_diag.get("details", details))
                    elif diag_status == "SKIP" and not details:
                        details = str(browser_diag.get("details", details))

                    visual_finding = run_visual_regression_snapshots(
                        base_url=resolved_wasm_url,
                        report_dir=report_dir,
                        config=config,
                    )
                else:
                    visual_finding = {
                        "check": "Visual regression snapshots",
                        "status": "SKIP",
                        "details": "Skipped because WASM runtime probe did not pass.",
                        "category": "product",
                    }

                run_results.append(
                    {
                        "tfm": tfm,
                        "mode": "smoke",
                        "status": status,
                        "category": "product",
                        "details": details,
                        **probe,
                    }
                )
                findings_visual_runtime.append(visual_finding)
            elif "desktop" in tfm:
                probe = run_probe_with_timeout(
                    ["dotnet", "run", "-f", tfm, "--no-build"],
                    cwd=project_root,
                    success_pattern=r"(Unhandled exception|Exception)",
                    timeout_sec=30,
                )

                probe["found"] = not bool(
                    re.search(r"Unhandled exception|Exception", str(probe.get("output", "")))
                )
                if probe.get("timed_out") and probe.get("found"):
                    probe["details"] = "Process remained alive during startup timeout and no early exception was detected."

                status = "PASS" if probe.get("found") else ("FAIL" if probe.get("rc") not in (None, 0) else "WARN")
                details = str(probe.get("details", ""))
                if not details:
                    details = (
                        "Smoke probe observed expected startup signal."
                        if status == "PASS"
                        else "Startup signal not observed within timeout."
                    )

                run_results.append(
                    {
                        "tfm": tfm,
                        "mode": "smoke",
                        "status": status,
                        "category": "product",
                        "details": details,
                        **probe,
                    }
                )
            else:
                run_results.append(
                    {
                        "tfm": tfm,
                        "mode": "readiness",
                        "status": "SKIP",
                        "category": "product",
                        "details": "No runtime probe configured for this TFM.",
                    }
                )

    checklist_item_count = 0
    checklist_source = "none"
    if checklist_path and checklist_path.exists():
        checklist_source = str(checklist_path)
        checklist_item_count = grep_count(r"^- \[ \]", try_read_text(checklist_path))

    findings: List[Dict[str, str]] = []
    findings.extend(audit_icons(project_root))
    findings.extend(audit_localization(project_root))
    findings.extend(audit_feature_usage(project_root))
    findings.extend(audit_layout_and_navigation(project_root, config))
    findings.extend(audit_custom_checks(project_root, config))
    findings.extend(findings_visual_runtime)
    findings = apply_finding_config(findings, config)

    status_counts = {"PASS": 0, "FAIL": 0, "WARN": 0, "MANUAL": 0, "SKIP": 0}
    for f in findings:
        status_counts[f["status"]] = status_counts.get(f["status"], 0) + 1

    build_fail_count = sum(1 for b in build_results if str(b.get("status")) == "FAIL")
    blocked_tfm_count = len(blocked_tfms)
    preflight_fail_count = sum(1 for p in preflight_results if str(p.get("status")) == "FAIL")
    runtime_fail_count = sum(1 for r in run_results if str(r.get("status")) == "FAIL")

    lines: List[str] = []
    lines.append("# Uno App Harness Report")
    lines.append("")
    lines.append(f"Generated: {now.isoformat(timespec='seconds')}")
    lines.append(f"Project: {project_root}")
    lines.append(f"Project file: {project_file}")
    lines.append("")
    lines.append("## Summary")
    lines.append("")
    lines.append(f"- TFMs detected: {', '.join(tfms)}")
    lines.append(f"- Build failures: {build_fail_count} / {len(build_results)}")
    lines.append(f"- Runtime failures: {runtime_fail_count} / {len(run_results)}")
    lines.append(f"- Preflight blocked TFMs: {blocked_tfm_count}")
    lines.append(f"- Findings: PASS={status_counts['PASS']} FAIL={status_counts['FAIL']} WARN={status_counts['WARN']} SKIP={status_counts['SKIP']}")
    lines.append(f"- Checklist source: {checklist_source}")
    lines.append(f"- Checklist unchecked items (raw count): {checklist_item_count}")
    lines.append(f"- Harness config source: {config_source}")
    lines.append("")

    if config_warnings:
        lines.append("## Harness Configuration Notes")
        lines.append("")
        for warning in config_warnings:
            lines.append(f"- {warning}")
        lines.append("")

    lines.append("## Preflight / Environment Matrix")
    lines.append("")
    lines.append("| TFM | Status | Category | Details |")
    lines.append("|---|---|---|---|")
    for p in preflight_results:
        lines.append(
            f"| {markdown_table_cell(p.get('tfm'))} | {markdown_table_cell(p.get('status'))} | {markdown_table_cell(p.get('category'))} | {markdown_table_cell(p.get('details'))} |"
        )
    lines.append("")

    lines.append("## Build Matrix")
    lines.append("")
    lines.append("| TFM | Status | Category | Duration (s) |")
    lines.append("|---|---|---|---:|")
    for b in build_results:
        lines.append(
            f"| {markdown_table_cell(b['tfm'])} | {markdown_table_cell(b.get('status'))} | {markdown_table_cell(b.get('category', 'product'))} | {markdown_table_cell(b.get('duration_sec'))} |"
        )
    lines.append("")

    for b in build_results:
        if str(b.get("status")) == "FAIL":
            lines.append(f"### Build Failure Details: {b['tfm']}")
            lines.append("")
            if b.get("issue_reason"):
                lines.append(f"Category: {b.get('category', 'product')} ({b.get('issue_reason')})")
                lines.append("")
            lines.append("```text")
            lines.append(format_log_snippet(str(b.get("stdout", "")) + "\n" + str(b.get("stderr", ""))))
            lines.append("```")
            lines.append("")
        elif str(b.get("status")) == "SKIP":
            lines.append(f"### Build Skipped: {b['tfm']}")
            lines.append("")
            lines.append(f"Reason: {b.get('skip_reason', 'No reason provided.')} ({b.get('category', 'environment')})")
            lines.append("")

    lines.append("## Runtime / Readiness Matrix")
    lines.append("")
    lines.append("| TFM | Mode | Status | Category | Details |")
    lines.append("|---|---|---|---|---|")
    for r in run_results:
        lines.append(
            f"| {markdown_table_cell(r.get('tfm'))} | {markdown_table_cell(r.get('mode'))} | {markdown_table_cell(r.get('status', 'SKIP'))} | {markdown_table_cell(r.get('category', 'product'))} | {markdown_table_cell(r.get('details', ''))} |"
        )
    lines.append("")

    lines.append("## Static Findings")
    lines.append("")
    lines.append("| Check | Status | Category | Details |")
    lines.append("|---|---|---|---|")
    for f in findings:
        lines.append(
            f"| {markdown_table_cell(f['check'])} | {markdown_table_cell(f['status'])} | {markdown_table_cell(f.get('category', 'product'))} | {markdown_table_cell(f['details'])} |"
        )
    lines.append("")

    lines.append("## Actionable Missing / High-Risk Items")
    lines.append("")
    actionable = [f for f in findings if f["status"] in ("FAIL", "WARN")]
    preflight_actionable = [p for p in preflight_results if str(p.get("status")) in ("FAIL", "WARN")]
    runtime_actionable = [r for r in run_results if str(r.get("status")) in ("FAIL", "WARN")]

    if not actionable and build_fail_count == 0 and not preflight_actionable and not runtime_actionable:
        lines.append("- None detected by automated checks.")
    else:
        for p in preflight_actionable:
            lines.append(f"- [{p['status']}][ENV] Preflight {p['tfm']}: {p['details']}")
        for r in runtime_actionable:
            category_tag = "ENV" if r.get("category") == "environment" else "PRODUCT"
            lines.append(f"- [{r.get('status')}][{category_tag}] Runtime {r.get('tfm')}: {r.get('details')}")
        for f in actionable:
            category_tag = "ENV" if f.get("category") == "environment" else "PRODUCT"
            lines.append(f"- [{f['status']}][{category_tag}] {f['check']}: {f['details']}")
        for b in build_results:
            if str(b.get("status")) == "FAIL":
                category_tag = "ENV" if b.get("category") == "environment" else "PRODUCT"
                lines.append(f"- [FAIL][{category_tag}] Build {b['tfm']}: check Build Matrix failure details above.")
    lines.append("")

    lines.append("## Notes")
    lines.append("")
    lines.append("- Static checks are heuristic and intentionally conservative.")
    lines.append("- Add app-specific checks as needed for product-specific UX and behavior expectations.")
    lines.append("")

    report_content = "\n".join(lines)
    write_report(report_path, report_content)
    write_report(latest_path, report_content)

    report_json: Dict[str, Any] = {
        "generated": now.isoformat(timespec="seconds"),
        "project": str(project_root),
        "project_file": str(project_file),
        "tfms": tfms,
        "summary": {
            "build_failures": build_fail_count,
            "build_total": len(build_results),
            "runtime_failures": runtime_fail_count,
            "runtime_total": len(run_results),
            "preflight_failures": preflight_fail_count,
            "preflight_blocked_tfms": blocked_tfm_count,
            "findings": status_counts,
            "checklist_source": checklist_source,
            "checklist_unchecked_items": checklist_item_count,
            "config_source": config_source,
        },
        "config_warnings": config_warnings,
        "preflight": preflight_results,
        "build": [
            {
                "tfm": b.get("tfm"),
                "status": b.get("status"),
                "category": b.get("category", "product"),
                "issue_reason": b.get("issue_reason", ""),
                "duration_sec": b.get("duration_sec"),
                "rc": b.get("rc"),
                "log_tail": format_log_snippet(str(b.get("stdout", "")) + "\n" + str(b.get("stderr", ""))),
                "skip_reason": b.get("skip_reason", ""),
            }
            for b in build_results
        ],
        "runtime": [
            {
                "tfm": r.get("tfm"),
                "mode": r.get("mode"),
                "status": r.get("status"),
                "category": r.get("category", "product"),
                "details": r.get("details", ""),
                "duration_sec": r.get("duration_sec"),
            }
            for r in run_results
        ],
        "findings": findings,
    }

    write_json(report_json_path, report_json)
    write_json(latest_json_path, report_json)

    print(f"Report written: {report_path}")
    print(f"Latest report: {latest_path}")
    print(f"JSON report written: {report_json_path}")
    print(f"Latest JSON report: {latest_json_path}")

    should_fail = args.strict and (
        preflight_fail_count > 0
        or build_fail_count > 0
        or runtime_fail_count > 0
        or status_counts.get("FAIL", 0) > 0
    )
    return 1 if should_fail else 0


if __name__ == "__main__":
    sys.exit(main())
