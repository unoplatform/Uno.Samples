---
name: uno-sample-lessons
description: "Hard-won fixes and gotchas for modernizing the Uno Platform samples in this repo (WinUI/Uno XAML layout, theming, navigation, icon/splash, platform targeting, and verification). Read before any UI/XAML work on a sample here."
when_to_use: "Use BEFORE editing or building any sample in this repo's `studio/` folder — especially when touching XAML layout, responsive/adaptive UI, ComboBox/TextBox/ListView styling, theme brushes or light/dark, Uno.Extensions navigation/shells, item-template commands, app icon/splash, charts, or when verifying a change on iOS/WASM/Android. These are repo-specific traps the general Uno skills don't cover."
metadata:
  author: uno-platform
  version: "1.0"
  category: samples
---

# Uno Sample Lessons — Agent Skill

This repo keeps a catalogue of **sample-specific** fixes and gotchas accumulated while
modernizing the Uno Platform samples. They are the kind of thing that wastes an afternoon if
you don't know them: silent no-ops, load-order traps, Skia-renderer quirks, and "this
compiles but does nothing" bindings.

## How to use this skill

1. **Read [`specs/lessons.md`](../../../specs/lessons.md) now.** It is the single source of
   truth — the lessons live there, not duplicated here, so this skill never drifts.
2. Scan its **Quick reference** table and match your task to the relevant lesson(s).
3. Apply the lesson, but first follow its **`Skill:`** cross-reference into the canonical
   `uno-*` studio skills (responsive, navigation, lightweight styling, semantic theme
   brushes, ancestor binding, testing, …) — those give the full, current API. The lesson
   only adds the repo-specific gotcha on top. **If the two ever disagree, the skill wins.**

## What's in the catalogue (high level)

- **Platform targeting & build** — every sample must build for iOS, Android, WASM, and
  desktop (Skia); native-head scaffolding; using the public `Uno.Sdk`.
- **Layout** — responsive (`{utu:Responsive}`, never `AdaptiveTrigger`), capped-width
  centering, filling `*` columns, aligned input rows, uniform cards, fluid wide layouts.
- **Control styling** — corner radius, re-skinning built-ins via lightweight styling,
  removing the gray `ListViewItem`/Button chrome.
- **Theming** — distinct light/dark from one palette, semantic role-named brushes in
  `ThemeDictionaries`, resolving brushes from code-behind.
- **Navigation** — Uno.Extensions shells/regions, tab back-stack, injected-page traps.
- **Bindings** — invoking page-VM commands from item templates (`{utu:ItemsControlBinding}`
  / `AncestorBinding`), `x:Bind` instance-vs-static helpers.
- **Icon & splash** — `Uno.Resizetizer`, `ExtendedSplashScreen` (white on iOS).
- **Rendering** — emoji "tofu" on Skia → use `FontIcon` glyphs.
- **Correctness** — static field init order.
- **Verification** — iOS sim / WASM+Playwright / Android (JDK 17) screenshot loops.

When a user correction yields a new sample-specific lesson, add it to `specs/lessons.md`
(per [`AGENTS.md`](../../../AGENTS.md)) — do **not** copy it here.
