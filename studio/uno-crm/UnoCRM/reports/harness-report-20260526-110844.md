# Uno App Harness Report

Generated: 2026-05-26T11:08:44
Project: /Users/SamMBP/Code/UnoCRM/UnoCRM
Project file: /Users/SamMBP/Code/UnoCRM/UnoCRM/UnoCRM.csproj

## Summary

- TFMs detected: net10.0-android, net10.0-ios, net10.0-browserwasm, net10.0-desktop
- Build failures: 0 / 4
- Runtime failures: 0 / 4
- Preflight blocked TFMs: 0
- Findings: PASS=6 FAIL=0 WARN=2 SKIP=8
- Checklist source: none
- Checklist unchecked items (raw count): 0
- Harness config source: none

## Preflight / Environment Matrix

| TFM | Status | Category | Details |
|---|---|---|---|
| net10.0-android | PASS | environment | Java 21 detected. |
| net10.0-ios | SKIP | environment | No preflight prerequisites configured for this TFM. |
| net10.0-browserwasm | SKIP | environment | No preflight prerequisites configured for this TFM. |
| net10.0-desktop | SKIP | environment | No preflight prerequisites configured for this TFM. |

## Build Matrix

| TFM | Status | Category | Duration (s) |
|---|---|---|---:|
| net10.0-android | PASS | product | 37.21 |
| net10.0-ios | PASS | product | 21.51 |
| net10.0-browserwasm | PASS | product | 7.33 |
| net10.0-desktop | PASS | product | 2.01 |

## Runtime / Readiness Matrix

| TFM | Mode | Status | Category | Details |
|---|---|---|---|---|
| net10.0-android | readiness | SKIP | product | No runtime probe configured for this TFM. |
| net10.0-ios | readiness | SKIP | product | No runtime probe configured for this TFM. |
| net10.0-browserwasm | smoke | PASS | product | Smoke probe observed expected startup signal. |
| net10.0-desktop | smoke | PASS | product | Process remained alive during startup timeout and no early exception was detected. |

## Static Findings

| Check | Status | Category | Details |
|---|---|---|---|
| Icon asset signal | PASS | product | Found icon-like SVG assets (2). |
| Base icon quality signal | WARN | product | icon.svg appears mostly rectangular; verify platform icon quality. |
| x:Uid coverage | WARN | product | Detected 7 x:Uid usages across XAML files. |
| Resource file signal | PASS | product | Detected .resw files with key counts: en=5 |
| Localization key parity | PASS | product | All locale .resw files contain matching key sets. |
| Localized icon glyph portability | PASS | product | No high-risk dingbat glyphs found in localized *.Content resource values. |
| ThemeService runtime usage | SKIP | product | ThemeService declaration not detected in App.xaml.cs. |
| Serialization runtime usage | SKIP | product | Serialization declaration not detected in App.xaml.cs. |
| Configuration value consumption | SKIP | product | No explicit configuration registration signal detected in App.xaml.cs. |
| Async UX signal | PASS | product | Detected FeedView/ProgressRing/VisualState markers in XAML. |
| Navigation wiring signal | SKIP | product | No common navigation wiring markers were found in XAML. |
| Tab-route consistency signal | SKIP | product | App.xaml.cs or Presentation/MainPage.xaml not found; skipped tab-route consistency check. |
| Breakpoint/layout adaptation signals | PASS | product | Detected responsive/adaptive XAML markers. |
| Touch target heuristic | SKIP | product | No filter-like buttons detected for touch-target heuristic. |
| CTA button alignment contract | SKIP | product | No configured CTA buttons were detected for alignment checks. |
| Visual regression snapshots | SKIP | product | Visual regression is disabled in harness config. |

## Actionable Missing / High-Risk Items

- [WARN][PRODUCT] Base icon quality signal: icon.svg appears mostly rectangular; verify platform icon quality.
- [WARN][PRODUCT] x:Uid coverage: Detected 7 x:Uid usages across XAML files.

## Notes

- Static checks are heuristic and intentionally conservative.
- Add app-specific checks as needed for product-specific UX and behavior expectations.
