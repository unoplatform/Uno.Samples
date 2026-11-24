# Showcase samples

This folder holds the samples that appear in the Uno Platform Studio gallery and on the platform.uno samples page. Each one is a complete app that can be tried live in the browser, opened in Studio, and read here on GitHub.

One list decides what shows and in which order: Studio's gallery catalog. A sample in this folder is public once the catalog has an entry pointing at it. The catalog reads the sample's info file for everything else, so the repo stays the source of truth for names, descriptions, platforms and screenshots.

## Layout

![A sample's folder in Uno.Samples: studio/<sample-id>/ holds the app's code, global.json, README.md and a .studiolive/ folder with sample.json, icon.png, screenshots/, and the Studio-only agent-context.json and user-conversation.json. The samples page and Uno Platform Studio both read it.](sample-structure.png)

```text
studio/
  <sample-id>/
    .studiolive/
      sample.json              # the info file (required)
      agent-context.json       # Studio: context injected into the agent (optional)
      user-conversation.json   # Studio: prefilled chat and suggested prompts (optional)
      icon.png                 # 256x256, referenced from sample.json
      screenshots/             # PNG, 1280x800 landscape or 390x844 portrait, referenced from sample.json
    global.json                # pins the public Uno.Sdk; the checker reads the Uno version here
    <App>.sln
    <App>/<App>.csproj         # sample.json points at this
    README.md
```

`<sample-id>` is kebab-case and stable. It is the folder name, the catalog id and the live-demo address, so pick it once.

The samples added before this layout still carry `uno-sample.json` at the sample root. Studio reads that file as a fallback, so nothing breaks, and each of them moves to `.studiolive/` when it is upgraded to Uno Platform 7.0.

## The info file: `.studiolive/sample.json`

The file has two groups of fields. The top level is Studio's sample manifest, schema v1 (`https://schemas.platform.uno/sample/v1`). The `showcase` object holds the fields the samples page and the checker need. Studio does not read `showcase`.

Worked example, from Patina:

```json
{
  "$schema": "https://schemas.platform.uno/sample/v1",
  "displayName": "Patina",
  "description": "Field app for a city's public-art conservation team. Conservators survey outdoor artworks, record findings with photos, and the app grades condition, schedules the next survey and turns serious findings into a treatment queue. Local-first, bilingual (English / French), one C# and XAML codebase for desktop, web, Android and iOS.",
  "platforms": ["desktop", "wasm", "android", "ios"],
  "sourcePath": "Patina/Patina.csproj",
  "iconUrl": "https://raw.githubusercontent.com/unoplatform/Uno.Samples/master/studio/patina/.studiolive/icon.png",
  "screenshots": [
    "https://raw.githubusercontent.com/unoplatform/Uno.Samples/master/studio/patina/.studiolive/screenshots/collection-desktop.png",
    "https://raw.githubusercontent.com/unoplatform/Uno.Samples/master/studio/patina/.studiolive/screenshots/survey-phone.png"
  ],
  "showcase": {
    "summary": "Condition surveys and a treatment queue for a city's outdoor artworks, local-first and bilingual.",
    "type": "showcase",
    "status": "active",
    "owner": ["mtmattei"],
    "industry": "Public sector, cultural heritage",
    "goals": [
      "A responsive shell that moves between phone and desktop layouts",
      "MVUX models backed by a local JSON document store",
      "Navigation Extensions with typed routes and data passing",
      "Localization with one EN/FR string table",
      "A Mapsui map with themed tiles inside an Uno Platform page",
      "Camera capture on Android and iOS"
    ],
    "requires": [
      "OpenStreetMap public tiles for the map",
      "A camera for survey photos (Android, iOS); desktop and web pick files"
    ],
    "tags": ["mvux", "navigation", "toolkit", "material", "localization", "maps", "local-first"]
  }
}
```

### Studio's fields (top level)

| Field | Required | What it is |
|---|---|---|
| `$schema` | yes | `https://schemas.platform.uno/sample/v1`. The format version lives in this URI. |
| `displayName` | yes | Name on the gallery card and in the chat. |
| `description` | yes | The gallery card paragraph: what the app is and which patterns it uses. |
| `platforms` | no | Any of `desktop`, `wasm`, `ios`, `android`, `macos`. Omitted means all. A showcase sample lists `wasm`, since the live demo is the web build. |
| `sourcePath` | yes | Path to the `.csproj`, relative to the sample folder. |
| `iconUrl` | no | Absolute URL to a 256x256 PNG under 200 KB. |
| `screenshots` | no | Absolute URLs to PNGs, up to 10, 1280x800 landscape or 390x844 portrait. |

Studio's two optional companion files keep their current shape:

- `agent-context.json`: `{ "messages": [ { "role", "content" } ] }`, the technical context the agent receives so it can keep editing the app. The checker expects at least one message, since an empty context does nothing.
- `user-conversation.json`: `{ "messages": [...], "suggestedPrompts": [...] }`, the chat Studio shows when the sample opens. Studio needs the `messages` list but not any messages in it, so an empty list with only `suggestedPrompts` is fine.

### Showcase fields (`showcase`)

| Field | Required | What it is |
|---|---|---|
| `summary` | yes | One sentence for the card on the samples page. `description` stays the longer paragraph. |
| `type` | yes | `showcase` (a complete app), `starter` (a shell to build from) or `feature` (one capability, small). |
| `status` | yes | `active`, `preview` (builds against a preview Uno Platform version) or `legacy`. |
| `owner` | yes | GitHub handles of the people who maintain the sample. |
| `industry` | no | The domain the app serves, free text. |
| `goals` | yes | Two to six lines: what the sample is meant to demonstrate. The samples page lists them, and reviewers check the app against them. |
| `requires` | no | External services, hardware or keys the app needs to run fully. Empty means it runs on its own. |
| `tags` | no | Short labels for filtering on the samples page. |
| `liveUrl` | no | The public web build. Set by the deploy step once the sample has one. |

### What is deliberately not in the file

- **Featured, sort order, published**: the catalog decides these. One list.
- **Uno Platform version**: `global.json` already pins it. The checker reads it from there; comparing it with what Studio supports is a planned PR check (see step 4 below).
- **Source link**: the repo plus the folder gives it; nothing to keep in sync.
- **Prompt that generated the app**: `user-conversation.json`, when there is one.

## Adding a sample

1. Create `studio/<sample-id>/` with the app, `global.json` pinned to the public `Uno.Sdk` version the catalog supports, and a README.
2. Add `.studiolive/sample.json` with every required field above, plus `icon.png` and at least one screenshot.
3. Add `agent-context.json` and `user-conversation.json` so the sample opens in Studio with context.
4. Go through [Before you open the PR](#before-you-open-the-pr), including the checker, then open the PR. CI builds every solution in the repo, this one included. It does not run the checker yet, and it doesn't check the Uno version against Studio or post a web preview; those are planned, so run the checker yourself and mention the result in the PR.
5. After merge, ask for a catalog entry (Dev first, then live). The sample appears on the samples page once the catalog lists it.

## Before you open the PR

A sample usually comes from a development repo, and that repo carries things a public sample shouldn't.

- **Leave out development files.** Session and handoff notes, specs, briefs, PRDs, test evidence and screenshots, agent and tool config (`.mcp.json` and the like), and scripts that only write into those folders. Remove the README's links to them too. Keep what a reader needs to build and run the app, and the files its README points to.
- **Drop the old manifest.** A new sample has no `uno-sample.json`; `.studiolive/sample.json` replaces it.
- **Pin the public `Uno.Sdk`.** The sample restores for people outside Uno Platform only on the public SDK, so the checker flags `Uno.Sdk.Private`.
- **Build Release on every platform in `platforms`.** Debug-only code that a Release build still reaches is the usual failure.
- **Check the README against the project.** Versions come from `global.json` and the restored packages, not from memory, and every link resolves.
- **Replace the template logo.** The launcher icon and splash (`Assets/Icons`, `Assets/Splash`) are the app's own, and `icon.png` is that icon at 256x256.
- **Take screenshots from the running app** at exactly 1280x800 or 390x844. A headless browser over the web build gives exact sizes without touching your desktop.
- **Check asset licences.** Bundled photos, fonts and data allow redistribution, and their credits stay with them.
- **Run the checker:** `python -I build/validate_studio_sample.py <sample-id>` checks the layout and info file against this page. It needs Python 3.9 or later and nothing else. `--root <folder>` checks a sample that isn't in this repo yet.

The icon and screenshot URLs point at `master`, so they show only after the merge.
