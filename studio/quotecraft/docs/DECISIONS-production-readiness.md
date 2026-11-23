# Decisions: production readiness (phase A, Uno 7 foundation)

QuoteCraft moved from Uno.Sdk 6.5.31 to 7.0.0-dev.115 and gained the iOS and Windows (WinAppSDK) heads next to Android,
WebAssembly and desktop. These are the choices that change behaviour on some heads, with what they cost.

## D1. SQLite on every head: SQLCipher, without a key in the browser (2026-11-01)

Decision: every head keeps `SQLitePCLRaw.bundle_e_sqlcipher` 2.1.10 with `Microsoft.Data.Sqlite.Core`. No head needed a
different package.

| Head | Native library | Encryption | Where the data lives |
|---|---|---|---|
| Desktop (Windows, Linux, macOS) | `e_sqlcipher` per runtime | On, random key in `.dbkey` | LocalApplicationData/QuoteCraft |
| Windows (WinAppSDK) | `e_sqlcipher.dll` (x64/ARM64) | On | LocalApplicationData/QuoteCraft (packaged: redirected to the package's LocalCache) |
| Android | `libe_sqlcipher.so` (arm64-v8a, x86_64 in the APK) | On | the app's private files |
| iOS | `SQLitePCLRaw.lib.e_sqlcipher.ios` (static, `__Internal`) | On | the app's sandbox |
| WebAssembly | `e_sqlcipher.a` linked into `dotnet.native.wasm` | Off | IndexedDB (IDBFS) under ApplicationData's LocalFolder |

WebAssembly needed four things (`QuoteCraft.csproj`, `Data/AppDatabase.cs`, `Helpers/AppStorage.cs`):
- **The matching static library.** SQLitePCLRaw's targets link the `net8.0` `e_sqlcipher.a` (built for .NET 8's emscripten)
  on any .NET. In a .NET 10 build every write failed with "disk I/O error". The package's `net9.0` build matches the
  emscripten that .NET 9 and 10 use (3.1.56); the `QuoteCraftLinkMatchingSqlCipher` target swaps it in before the link.
- **IDBFS** (`WasmShellEnableIDBFS`). Without it the browser file system is in memory and nothing survives a page load.
  Uno writes it to IndexedDB every 10 seconds and when the page unloads, so the last few seconds before a tab is killed
  can be lost.
- **The data folder under `ApplicationData.Current.LocalFolder`**, awaited through a `StorageFolder` call before the first
  `System.IO` access (`AppStorage.EnsureReadyAsync`): only those folders are persisted.
- **No WAL.** It is for concurrent readers, and the browser runtime has one thread.

The key is off in the browser, as the original code intended (its `#if __BROWSERWASM__` never compiled: no SDK defines
that symbol): the key would sit in the same IndexedDB as the database, and SQLCipher's key derivation would run on the
single UI thread at every open.

Tradeoff and open points:
- WASM0001 (SQLitePCLRaw declares varargs `sqlite3_config` and `sqlite3_db_config`, which WebAssembly cannot call) is
  reported as a message. Microsoft.Data.Sqlite does not call them; creating, reading and writing quotes worked in the
  browser.
- On the other heads the key is a file next to the database. That protects a copied database file, not a copied app
  folder. Moving the key to the platform store (Keychain, Android Keystore, DPAPI) is a code-review item.
- When SQLitePCLRaw ships targets for .NET 10 (or the app moves to a later SQLitePCLRaw), delete the swap target.
- Verified at runtime: desktop (Linux, Xvfb) and WebAssembly (headless Chromium: a quote created, still there after
  the browser was closed and reopened). Android is build-verified (the APK carries `libe_sqlcipher.so`); iOS and
  WinAppSDK are built by CI only.

## D2. PDF export: QuestPDF on desktop and Windows; a clear note elsewhere (2026-11-01)

Decision: QuestPDF stays the PDF engine, referenced by the `net10.0-desktop` and WinAppSDK heads only (`QUESTPDF`
symbol). On Android, iOS and WebAssembly, `IPdfGenerator` is `UnsupportedPdfGenerator`, `IShareService.CanExportPdf` is
false, and the pages show "PDF export is available in the desktop and Windows apps." where the Download PDF (dashboard
preview) and Export PDF (editor) buttons would be. The Send dialog drops "Download PDF Only" there and opens the email
without an attachment.

Reason: QuestPDF ships native libraries for Windows, Linux and macOS only. Before this change the mobile and browser
heads compiled QuestPDF and would have failed at runtime. The old browser-download branch never compiled.

What SkiaSharp's own PDF backend would take (evaluated, not implemented):
- **Rendering is possible.** SkiaSharp 4.151.1, which Uno 7 already ships to every head, exports
  `sk_document_create_pdf_from_stream` in its Android `libSkiaSharp.so` and WebAssembly `libSkiaSharp.a` (checked here;
  the iOS native package was not inspected). `SKDocument.CreatePdf` needs no new package.
- **It is not small.** The QuestPDF layout would have to be drawn by hand on `SKCanvas`: the header with logo and
  contact lines, the line-item table with alternating rows and page breaks, totals, wrapped notes, the footer and
  watermark, and the photo appendix (about 200 lines). The browser has no system fonts, so DM Sans must be loaded from
  the package (`ms-appx:///Assets/Fonts/...`) into an `SKTypeface`.
- **Delivery differs per head.** WebAssembly needs a browser download (JS interop). Android and iOS need a
  `FileSavePicker`: Uno's share sheet (`DataTransferManager`) carries text and URIs, not files.
- Android and iOS could not be verified at runtime here.

Options for the owner:
1. Keep this state (smallest; honest about the gap).
2. Add an `SKDocument` generator for Android, iOS and WebAssembly behind `IPdfGenerator`, keeping QuestPDF on desktop and
   Windows. Two layouts to keep in step.
3. Replace QuestPDF with the `SKDocument` generator everywhere: one layout, one fewer package, and no QuestPDF Community
   licence terms to track, at the cost of rewriting a working desktop renderer.

Verified: on desktop (Linux) Export PDF wrote `exports/QC-2026-0001.pdf` with the business name, line item, tax,
total and notes. Opening it needs `xdg-open`, which this machine lacks (logged).

## D3. Local-first; sign-in, sync and billing only with a Supabase project (2026-11-01)

Owner decision (round 2 plan): QuoteCraft runs fully on local data with no account.

- `Data/Remote/SupabaseConfig` no longer holds constants. The service-role key is gone from the app: it bypasses
  row-level security and belongs to the Edge Functions in `supabase/` only.
- The project URL and anon key come from the `Supabase` section of `appsettings.json` (empty in the repo), or from
  `QUOTECRAFT_SUPABASE_URL` and `QUOTECRAFT_SUPABASE_ANON_KEY`. `Supabase:CustomerPortalUrl` replaces the placeholder
  Stripe portal link.
- Without them: no session is restored, the Supabase client, share links and checkout refuse to run, and the
  connectivity poll (which called httpbin.org every 15 seconds) does not start; when it runs, it probes the configured
  project. Settings hides the upgrade banner, the subscription section and Sign Out, and says the data stays on this
  device.
- **No plans without billing.** `FeatureGateService` lifts the Free limits (5 quotes a month, 10 clients) and the PDF
  watermark when billing is not configured, and keeps cloud sync off. An upgrade prompt that cannot be acted on would
  be a dead end. **Owner decision (2026-11-03): keep them lifted.** `HasPlans` stays the one switch; the
  `LocalOnly_*` tests in `FeatureGateServiceTests` pin the behaviour.

Open points for the next phases (behaviour with a Supabase project configured, unchanged here):
- `ISyncService.StartAsync` is never called, so sync never runs. Since round 2 this is the decision (D8), not a gap.
- The Auth page is reachable only through Sign Out; there is no Sign In entry point.
- The refresh token is stored in a plain file (`.auth_token`), not in platform secure storage as the Architecture Brief
  says (still open; see D8).
- The trimmed WebAssembly build serializes the Supabase payloads by reflection (IL2026, suppressed because
  `LinkerConfig.xml` preserves the assembly); cloud mode in the browser has not been run.

## D4. Smaller Uno 7 changes (2026-11-01)

- **Fonts (Uno Themes 9).** The type ramp reads `DefaultFontFamily`, and `FontOverrideSource` no longer applied, so every
  page fell back to Roboto. The override is an inline `FontOverrideDictionary` that sets `DefaultFontFamily` and the
  `Material*FontFamily` keys; weights resolve through `DMSans-Regular.ttf.manifest`.
- **Onboarding.** It never showed on a first run, on 6.5 as on 7: `ShellModel`'s constructor raced the default route.
  `App.OnLaunched` now opens it after the first navigation completes. Opening it earlier (from MainPage) left the
  WebAssembly splash screen up for good. Skip and Create First Quote now go to `Main` and `Main/QuoteEditor`.
- **Navigation from code-behind.** The dashboard's New Quote dialog and the client page's quote rows took `INavigator`
  from the root service provider; under Uno.Extensions 8 the dialog closed without creating a quote. They use the
  page's navigator now.
- **Pickers.** The WinAppSDK owner window is set under `#if WINDOWS` (the old `#if !__BROWSERWASM__` was always true),
  `SuggestedStartLocation` is set (WinUI requires it), and picked files are read through `StorageFile` streams.
- **Trimming.** `MvuxHelper` reflects over MVUX-generated view models (IL2070) and the Supabase payloads are
  serialized by reflection (IL2026); both live in this assembly, which `Platforms/WebAssembly/LinkerConfig.xml`
  preserves, so the warnings are suppressed with that justification.
- **WebAssembly splash.** Toolkit 11's `ExtendedSplashScreen` requests `appmanifest.js` in lower case while the bootstrapper
  serves `AppManifest.js`. On a case-sensitive host it logs "Error while getting native splash screen" and shows its
  loading template instead; the app is unaffected.

# Phase B: UI hardening (2026-11-02)

## D5. Four layouts with the default breakpoints; one pane below 1080

Every page answers the four `utu:Responsive` categories (Narrow below 600, Normal 600 to 800, Wide 800 to 1080, Widest
1080 and up); the table is in the README under Layouts.

- **Quotes, Clients and Catalog are one pane below 1080.** The list fills the page; picking an item opens it over the
  list, with a back button (`BackToList` in each model clears the selection). Before, the detail pane was simply hidden
  below 800, so a tap on a quote or client did nothing on a phone, and the 380 px list column clipped at 360 px.
- **Side by side from 1080.** At 800 the rail leaves 644 px; a 380 px list would leave the detail 264 px, too little
  for the action row or the line-item table.
- **One custom layout, `SplitLayout` (Widest at 1400), for splits inside a pane**: the quote's client/details/totals
  column and the client's summary sit beside the main column, and Settings shows its live preview, only from 1400.
  At 1080 the detail is about 470 px wide, and a 340 px side column left the line-item table below 130 px. Below 1400
  these stack.
- **Navigation:** bottom tabs below 800, the side rail from 800 (unchanged). Notifications and the rail's theme menu
  are rail-only; below 800 the theme is in Settings. Notifications have no entry point on phones (open point).

## D6. Theme: System by default, Light or Dark by choice, through the Uno.Extensions theme service

`UseThemeSwitching()` (already in Uno.Extensions.Hosting, no new package) registers the window's `IThemeService`,
which applies the theme and saves it. Settings has an Appearance row (System default, Light, Dark) and the rail's
Theme button opens the same three choices; the old toggle flipped Light/Dark and was forgotten at the next launch.

- Colours set from code read the element's theme (`Helpers/ThemeResources`) and are set again on
  `ActualThemeChanged` (card hover/selection in `InteractionHelper`). The status pills are a control whose colours are
  `ThemeResource` setters; the old converters cached the first theme's brushes, so pills kept Light colours in Dark.
- Contrast: text on the amber accent is now `OnAccentBrush` #1A365D (5.6:1; the slate band colour gave 4.0:1), and
  hover/pressed ambers are lighter so the label stays above 4.5:1. `OnSurfaceVariant` moved from #6B7280 to #5F6673
  (Light) and from #9CA3AF to #AAB1BD (Dark) to reach 4.5:1 on the search fields and surface-variant panels. Draft
  and Expired pill text moved to #4B5563 (Light) and #D1D5DB (Dark); Expired was 2.3:1.
- Hex left on purpose: the palettes themselves (`ColorPaletteOverride.xaml`, `StatusColors.xaml`, brand colours in
  `App.xaml`) and QuestPDF's colours in `Services/PdfGenerator.cs`, which draw a printed page, not the UI.

## D7. Smaller Phase B fixes

- **Quote editor back arrow** is an explicit action on the navigation bar and falls back to the quotes list when
  there is no page to go back to (the editor opened straight from onboarding had none). Tapping the selected tab now
  navigates to it, so the Quotes tab works after the editor (the editor is not a tab, so Quotes stayed selected).
- **Relative dates** handle the future: a valid-until date four weeks ahead read "Just now"; it reads "In 4 weeks"
  (`Helpers/RelativeDate`, tested). Day words (Today, Tomorrow) use the local day.
- **Dead or broken controls:** Edit client did nothing (it read a selection only an unwired tap handler set); the
  catalog item dialog opened a second dialog for Delete, which a ContentDialog cannot do (Delete is now the dialog's
  own button); "+ Add Catalog" in Settings was text with no action (now "Manage catalog", which opens Catalog); the
  dashboard's Upgrade buttons had no action (they open Settings, where plans live).
- **States:** every list and detail feed has Progress, None and Error templates, and Retry reaches the page model by
  `ElementName` (the error template's DataContext is the exception). The editors show validation messages (dialogs
  no longer just refuse to close), Settings and the quote editor show a saving bar, and Settings says when it saved.

# Round 2 code review (2026-11-02)

## D8. Cloud sync is off, even with a Supabase project

Decision: `FeatureGateService.HasCloudSync` is false for every plan and nothing starts `SyncService`. Settings says
"Cloud sync is off in this version" in the account section (a project configured), and the local-mode note says the
data stays on the device (none configured); the offline banner no longer promises a sync.

Reason: syncing what the review asked for (line items, the catalog and the business profile, beside the quotes and
clients it pushes now) is not a contained change, and none of it could be run against a project here:
- **Line items** are hard-deleted locally and the remote `line_items` table has no `is_deleted`, so a deleted item
  could never reach another device. That needs a tombstone column, or "replace the quote's items" on every push, in
  both schemas. Without line items a synced quote arrives on another device at $0.
- **Business profile**: the remote `business_profile` key is `id TEXT PRIMARY KEY DEFAULT 'default'`, so a second user
  could not store one; the key has to become the user id (a server migration).
- **Catalog**: every install seeds its 70 items with new random ids, so two devices would merge into 140. The seed
  needs stable ids first.
- **Churn**, to fix in the same pass: `SaveAsync` stamps `UpdatedAt` on every save (the push marks rows synced through
  it, so they look changed again and re-push every 30 seconds), `ReadClient` never reads `synced_at`, and pulled rows
  are saved with a new `UpdatedAt`. A sync pass should mark rows synced with their own `UpdatedAt`, save pulled rows
  as they are, and push only rows whose `updated_at` is later than `synced_at`.
- Quote numbers: each device has its own per-year counter (D10), so two devices can hand out the same number; the
  unique index would refuse the second on pull. Synced numbering needs a device part or a server-side counter.

Token: the refresh token stays in a plain file in the app's data folder. The app uses no platform secure store
(Keychain, Android Keystore, DPAPI, the WinAppSDK PasswordVault), and adding one per head is part of the same pass.

What stays: sign-in, plans and billing work as before when a project is configured (D3). `SyncService`, the
connectivity check and the Supabase tables are kept for that pass.


## D9. Money: decimal amounts, tax rounded once to the cent, rates as entered

The rules live in `Helpers/Money` and nowhere else; the UI converter, the editor's totals, the dashboard and the PDF all
go through them:
- Amounts are `decimal`. Unit prices are whole cents: the item and catalog dialogs refuse a price with more than two
  decimals rather than round it. A line total is unit price × quantity.
- Tax is subtotal × rate, rounded once, to the cent, half away from zero (10.00 at 8.25 % is 0.83; .NET's default,
  half to even, would give 0.82). The total is subtotal + tax.
- Tax and markup rates are kept as typed (8.25, 9.975) and shown in full. They were shown with one decimal and read back
  on every save, so 8.25 % became 8.3 %.
- Typed numbers are read in the user's culture ("8,25" in German); amounts are shown in the convention of the
  business's currency (USD as "$1,250.00", EUR as "1 250,00 €"), on screen and in the PDF. A thousands separator must
  group by three, so "1,5" in English is refused instead of read as 15; a currency symbol in a price is ignored.

Tradeoff: amounts and rates are still stored as SQLite `REAL`. Converting a stored double back to `decimal` keeps 15
significant digits, which is exact for any price, rate or total this app handles (tested with 8.25 and 9.975). Moving
to integer cents or text would be a migration for every money column and the remote schema, with no visible gain now.

## D10. Quote numbers: a per-year counter that never goes back

`quote_counters` holds one row per year; `NextQuoteNumberAsync` takes the next number in a transaction and skips any
number a quote already has, and a unique index on `quote_number` refuses a duplicate. Migration 6 seeded the counters
from the highest number each year had and renumbered quotes that shared a number (the oldest kept it).
- A number taken and not saved (the New Quote dialog creates the quote at once, so this is rare) leaves a gap. Gaps are
  fine; reuse is not, since PDF exports are named after the number.
- Deleted quotes keep their numbers (they are soft-deleted), so their old PDFs stay unambiguous.
- The year is the UTC year at creation. The counter is per device; synced numbering needs more (D8).

## D11. Smaller round-2 changes

- **Status rules** (`QuoteStatusService`, tested): only Sent and Viewed quotes expire; sending makes the quote valid for
  the profile's number of days from today (or keeps a later date); Reopen turns an expired quote into a Draft with a
  fresh validity period (a banner on the expired quote offers it); Send by email leaves Accepted and Declined alone;
  Export PDF changes no status. `QuoteExpiryService` runs while the app runs (at start, then every 5 minutes) and adds
  one 3-day reminder per quote and validity period.
- **Removed:** share links and SMS sharing (they need the quote on the server, and sync is off: D8), and the routed
  ClientEditor and CatalogEditor pages (nothing reached them; clients and catalog items are edited in dialogs).
  Migration 5, which rewrote quotes found by title, is a no-op.
- **Data access:** one pooled connection per operation instead of one shared connection, so MVUX's background reads
  and the pages' writes no longer interleave on one connection (a concurrency test fails on the old code).
- **Errors:** page handlers report failures through `ErrorReporter` (logged, and a dialog says what did not happen);
  failed bound commands show on the page (a banner on Quotes, a message in the editor); `App` logs what nothing else
  caught and keeps running; a startup failure shows its message instead of a blank window.

## D12. PDF export: SkiaSharp's `SKDocument` on every head; QuestPDF removed (2026-11-03)

Owner delegated the call (D2 option 3).

Decision: one `IPdfGenerator` drawn with `SKDocument.CreatePdf` on all five heads, and QuestPDF is removed.

Reason:
- Contractors quote on site from a phone, so PDF export missing on Android, iOS and WebAssembly left the core flow
  broken on three heads.
- SkiaSharp 4 already ships to every head, so there is one layout, one fewer package and no QuestPDF licence terms.

Tradeoff:
- A working desktop renderer is rewritten.
- Delivery differs per head: a file on desktop and Windows, a save picker on Android and iOS, a browser download on
  WebAssembly.
- Android and iOS are verified by CI builds only.

Implementation (2026-11-03):
- **Layout** (`Services/PdfGenerator.cs`, `QuotePdf.Render`), US Letter with 40 pt margins, as the QuestPDF layout had it:
  the header (logo, business name and contact lines, QUOTE with the number, date and validity) and the footer (custom
  footer, the watermark when `HasPdfWatermark`, the date) on every page; the title and client; the line-item table with
  alternating rows, which breaks between rows and repeats its header row on the next page; subtotal, tax and total
  through `Helpers/Money` as the app shows them; the notes, wrapped and broken across pages by line; then a photo
  appendix, two photos a row. DM Sans Regular and Bold are read from the package (`ms-appx:///Assets/Fonts`) into
  `SKTypeface`; the PDF embeds subsets of them. DM Sans has no narrow no-break space, which fr-FR uses to group digits
  ("1 250,00 €"), so a space glyph the font lacks prints as a plain space.
- **Delivery** (`Services/ShareService.cs`): desktop and WinAppSDK write `exports/<number>.pdf` and open it, as before.
  Android uses `FileSavePicker` (the system's Create document dialog, the quote number as the suggested name).
  WebAssembly uses `FileSavePicker` in Uno's download mode (`WasmPickerConfiguration.DownloadUpload`, set for that call
  only), so every browser downloads the file: the File System Access save dialog needs a fresh user gesture, and the
  PDF is drawn before the picker opens. iOS: Uno's docs mark `FileSavePicker` partial there, and Uno 7's iOS build throws
  "FileSavePicker extension is not registered" unless the app supplies one, so the PDF is written to the exports
  folder and handed to `UIDocumentPickerViewController` in export mode (Save to Files). Pickers open on the UI thread;
  MVUX runs commands on a background thread.
- **Removed:** the QuestPDF package and the `QUESTPDF` symbol, `UnsupportedPdfGenerator`, `IPdfGenerator.IsSupported`,
  `IShareService.CanExportPdf` (always true now, so the property and its branches went with it) and the "PDF export is
  available in the desktop and Windows apps" notes. Download PDF (dashboard preview), Export PDF (editor) and Download
  PDF Only (Send dialog) show on every head, and Send via Email always saves the PDF first.

Verified:
- **Unit tests** (`PdfGeneratorTests`, fonts from the app's assets): the output starts with `%PDF`; 60 line items give
  more than one page. With poppler's `pdftotext`: the amounts read as `Money.Format` writes them; every page repeats the
  header, the table header and the footer date; the watermark appears only when on. The text tests are skipped where
  `pdftotext` is missing, which includes CI's Ubuntu image (no poppler-utils).
- **Desktop** (Linux, Xvfb, Debug, `--qc-seed=sample`, with 24 more line items, a logo and three photos added to
  QC-2026-0001): Export PDF (editor), Download PDF (preview) and Download PDF Only (Send dialog) each wrote
  `exports/QC-2026-0001.pdf` (321 KB, three Letter pages, DM Sans embedded). Rendered with `pdftoppm`
  (`docs/evidence/pdf-export.jpg`): the header with the logo on both quote pages; the table breaking after 18 rows with
  its header row repeated; subtotal $9,058.00 USD, tax (13%) $1,177.54 and total $10,235.54 USD, as the editor shows
  them; the notes, the custom footer, and the photo appendix. The watermark is off without billing (D3); a render of the
  same layout with it on (from a test) showed it centred above the date.
- **WebAssembly** (Release publish, served locally, headless Chromium through Playwright, a fresh browser profile):
  skipped onboarding, created a quote, added "Panel upgrade to 200 A", 2 × $1,250.50, in the editor, and pressed Export
  PDF. The browser downloaded `QC-2026-0001.pdf` (83 KB, one page, DM Sans Regular and Bold embedded, read from the
  app package): the header, the item row, subtotal $2,501.00 USD, tax (8.5%) $212.59 and total $2,713.59 USD, as the
  editor showed them. The page showed no error.
- **Android**: Release build, 0 warnings; not run (no device or emulator here). **iOS** and **WinAppSDK**: CI builds.

## D13. Database key and refresh token in `PasswordVault` where Uno has one (2026-11-03)

Owner delegated the call (D1 and D3 open points).

Decision: Android, iOS and WinAppSDK keep the SQLCipher key and the refresh token in
`Windows.Security.Credentials.PasswordVault`, and an existing `.dbkey` or `.auth_token` file moves into it. Desktop
keeps the files. WebAssembly stays unencrypted (D1).

Reason:
- Uno implements the vault on exactly those heads, on AndroidKeyStore and the iOS Keychain.
- No new package or UnoFeature is needed.

Tradeoff:
- Desktop (Windows, Linux, macOS on Skia) keeps a key file next to the database. DPAPI would be a new package, and
  Uno has no vault on Linux or macOS.
- Android, iOS and WinAppSDK are verified by CI builds only.

Implementation (2026-11-03): `Helpers/StoredSecret.cs`. `AppDatabase` and `AuthService` read their secret through a
`StoredSecret` (a name in the vault, and the file earlier versions wrote):
- With a vault, a file left by an earlier version is written to the vault and deleted once the vault gives the same
  value back. It wins over a different value already in the vault, since it belongs to the database beside it (an iOS
  Keychain entry can outlive an uninstall). Otherwise the vault's value is used, and a new key goes to the vault only.
- Without a vault (desktop, WebAssembly) the files stay as they were.
- The app uses one vault instance: Uno's Android and iOS vaults keep the whole list in memory and write it whole at every
  change, so a second instance would drop the first one's entries.

Verified: `StoredSecretTests` (six cases, in-memory vault: create, reuse, move a file in, a file winning over a stale
value, a vault that does not keep the value leaving the file in place, delete); the Android Release build (0 warnings);
the desktop app still creates and reads `.dbkey`. The vault itself runs only on Android, iOS and WinAppSDK, none of
which ran here.

Open points:
- **Android backup: the app's files stay on the device (owner decision, 2026-11-03).** A backup would carry the
  database and Uno's encrypted `.vault` file but not the AndroidKeyStore key that opens them, so a restored copy could
  not be opened. `Platforms/Android/Resources/xml/backup_rules.xml` (Android 11 and earlier) and
  `data_extraction_rules.xml` (Android 12 and later, cloud backup and device transfer) exclude the `file` domain;
  settings (SharedPreferences) still travel. A new phone starts empty until cloud sync exists (D8). Checked: the
  Release APK carries both rule files and the merged manifest references them; a real backup and restore was not run.
- **iOS Keychain access.** Uno's vault throws "not allowed to use the keychain" on status -34018, and
  `Platforms/iOS/Entitlements.plist` is empty. Apps normally get their default access group without an entry; confirm on
  the first signed device build.

# Uno audit (2026-11-04)

Proposed by the uno-audit (`docs/UNO-AUDIT.md`); approved by the owner on 2026-11-04.

## D14. Dialogs are XAML `ContentDialog` routes, opened from models with `!` (owner decision, 2026-11-04)

Decision: every dialog is a XAML `ContentDialog` with an MVUX model in `Presentation/Dialogs`, registered as a route in
`App.xaml.cs` and opened from a model with the `!` qualifier (`Helpers/DialogNavigation`: `Confirm`, `NewQuote`,
`SendQuote`, `LineItemEditor`, `CatalogBrowser`, `ClientEditor`, `CatalogItemEditor`). Form dialogs keep their actions
in the content, so the model validates before the dialog closes and the dialog stays open with a message; the result
goes back through `NavigateBackWithResultAsync`. Cancel is `Navigation.Request="-"`.

Reason: the dialogs were built in C# inside page code-behind (about 1,500 lines of handlers across the pages), so
their validation and results could not be tested, and a second dialog opened from a code-built one never showed
(deleting a catalog item). As routes they are XAML with models like the pages, and callers get a typed result.

Tradeoff: a dialog's buttons are ordinary buttons in its content, not the `ContentDialog` primary and close buttons,
so the keyboard default (Enter) is the focused button rather than the dialog's primary action. The form's own Cancel
carries a `xaml-lint: allow backbar` comment, since a dialog has no navigation bar.

## D15. `!` routes to a Page open in a right-hand drawer (owner decision, 2026-11-04)

Decision: `Presentation/DrawerFlyout` (a `DrawerFlyoutPresenter`, right-hand, 420 px) replaces the full-window
`NavigationFlyout` for `!` routes to a Page; it is registered as the navigation `Flyout` after `UseNavigation`. The
notifications drawer (`!Notifications`) is the one such route today.

Reason: the notifications overlay was a panel shown and filled from MainPage's code-behind, with an empty `catch`
that hid its failures (it stayed blank on every launch after the first). As a routed Page with its own model it loads
and fails like any page, and the drawer keeps the page behind it in view.

Tradeoff: the drawer's width is the Toolkit's `DrawerLength`, set through the Toolkit's definition-only workaround
style (an attached-property setter in a style based on a Toolkit template is otherwise refused on WinAppSDK,
microsoft-ui-xaml#6388). On a phone the drawer covers all but a strip of the page behind it.

## D16. Cards are `CardContentControl`s with the styles in `Styles/Cards.xaml` (owner decision, 2026-11-04)

Decision: rounded containers are Toolkit `CardContentControl`s with the app's styles in `Styles/Cards.xaml`
(`StaticCardStyle`, `PaddedCardStyle`, `TonalCardStyle`, `BannerCardStyle`, `AvatarCardStyle`,
`ElevatedSurfaceCardStyle`), not Borders with a corner radius. Material's hover, pressed and focus tints on cards are
off app-wide (lightweight keys in `App.xaml`): a card is a surface, not a button. Raised cards use Material's elevated
card, whose `ElevatedView` draws the shadow on every head.

Reason: 71 Borders were cards drawn by hand (the semantic lint gate's CARD rule), each with its own corners, stroke
and shadow, and `ThemeShadow`/`Translation` on a Toolkit control does not compile for WinAppSDK (WMC0612). One set of
styles keeps the surfaces consistent in both themes.

Tradeoff: with the tints off, a card that can be tapped needs its states from what it sits in: a list item container
or a button inside it, never from code-behind. On touch, a swipe that starts on a card can no longer leave a hover
tint up. The Material card template passes its alignment to its own fill, so a fixed-size card that is centred
shrinks its fill to its content: avatars stay `Stretch` and are placed by their parent.

# A pass (2026-11-04)

## D17. Cloud mode keeps D3's shape, with its two missing controls built (proposed, 2026-11-04)

Decision: cloud mode stays as D3 settled it (sign-in, plans and billing appear only with a Supabase project; sync
stays off, D8). The two gaps the completeness audit found there are small, so they are built, not cut:
- **Sign In**: Settings' account section offers Sign In while nobody is signed in and Sign Out once someone is
  (`SettingsModel.IsSignedIn` follows `IAuthService.AuthStateChanged`). The Auth page was reachable only through Sign
  Out. Sign-up mode on the Auth page gets its Create Account button (the mode switch toggled a flag nothing showed).
- **Annual billing** (PRD F13.6): the upgrade banner offers the yearly plan ($144/yr, 20% off, from
  `docs/Revenue-Model.md`) beside the monthly one; `UpgradeToProAnnual` asks checkout for the annual interval.
  Checkout and portal failures now say so in a dialog instead of only logging.

Reason: "build small, cut large". Each gap is one control over code that already exists; cutting cloud mode would
reopen D3, an owner decision.

Tradeoff: neither control can run here (no Supabase project); they are covered by model tests (`SettingsModelTests`:
Sign In opens `Auth`, the annual plan asks for `annual` and reports an unavailable checkout) and by code reading.

## D18. Below 800 px an Alerts tab opens the notifications (proposed, 2026-11-04)

Decision: the bottom tab bar gets a fifth item, Alerts (screen readers: "Notifications"), that opens the same
`!Notifications` drawer as the rail's bell, with the unread count as its badge. It is not selectable: it opens the
drawer and the current tab stays selected. D5 left this an open point.

Reason: expiry reminders (PRD F17.2) were unreachable on phones; the rail, with the only bell, is hidden below 800 px.
A tab item is the smallest change that is always in view, and the Material bottom tab bar draws the badge.

Tradeoff: five items at 360 px leave each about 72 px, so the label is the short "Alerts" rather than
"Notifications".

## D19. The Quotes page is two models; list cards are Material buttons (proposed, 2026-11-04)

Decision:
- The Quotes page is `DashboardModel` (the list, search, filter, counts, analytics, New Quote) and
  `QuoteDetailModel` (the open quote), shown by `QuoteDetailView` in a `ContentControl` region on the page (route
  `./QuoteDetail`, nested under `Dashboard`). They share `Services/QuoteBoard` (which quote is open, and that quotes
  changed); the editor's Save, status buttons and line-item changes raise it too, so the list and the detail read the
  quotes again after the editor.
- A tappable list card (quotes, clients, catalog categories and items) holds a `ListCardButtonStyle` button (Material
  text button) that fills it: Material draws hover, pressed and focus, the button is the keyboard stop with Enter and
  Space, and it carries the item's name. The open quote's card keeps a bound tint. `Helpers/InteractionHelper`, which
  painted these states from code, is removed.

Reason: the approved refactor (DashboardModel was 498 lines doing six jobs; InteractionHelper set brushes and
`Translation` from pointer and key handlers). The button pattern is the one the editor's line items and the client's
quote rows already used, and it follows D16 (cards stay surfaces; states come from what they hold).

Tradeoff: each card is its own tab stop (a `ListView` would give one stop and arrow keys), and the cards lose the
small lift on hover that InteractionHelper drew. Coming back from the full editor makes the Quotes page again, with no
quote open, as before the split.

# A pass 2 (2026-11-04)

## D20. A quote's valid-until date is set in the editor, and sending keeps it (proposed, 2026-11-04)

Decision: PRD F1.8 is built. The full editor has a Valid Until date picker under the tax rate. A new quote starts at
the Settings default ("Valid For", 14 days unless changed, counted from creation); expiry, the 3-day reminder, the Send email
and the PDF read the same date. A picked day is stored as its last second in UTC (`Helpers/ValidUntilDate`), so the
quote is valid through that day; a date the user did not touch keeps its exact time. A changed day before today is
refused ("The valid-until date cannot be in the past."); an old quote past its date still saves when its date is left
alone.

This changes one rule of D11: sending kept the profile's number of days from today unless the quote's own date was
later, which would have overwritten a shorter date picked in the editor. Now sending (Mark Sent, Send by email) and
Reopen keep the quote's date while it is ahead, and give a quote whose date has passed the profile's number of days
from today, as before.

Reason: the review found F1.8 neither built nor cut. The date, its default, expiry and the PDF already existed; the
editor field and the send rule are the small part that was missing (build small).

Tradeoff: a Draft created some days before it is sent keeps the date it was given at creation (the PRD's "14 days from
creation"), where it used to get a full period from the day it was sent. The editor shows the date, so it can be moved
before sending.

## D21. The Viewed filter and count go; stored Viewed quotes read as Sent (A pass 2 list, 2026-11-04)

Decision: the Quotes page has no Viewed filter chip and no Viewed count, and the README no longer lists the status.
Nothing in the app sets Viewed since D11 removed share links. `QuoteStatus.Viewed` stays so a quote an older install
stored with it still loads: it keeps its pill, it is counted and filtered with Sent, and it expires like a Sent quote.
The sample data seeds that quote as Sent.

Reason: the cut in D11 left the controls for a status nothing can reach (the audit's open question).

## D22. A photo thumbnail opens the photo enlarged in a dialog; no full-window viewer (proposed, 2026-11-04)

Decision: PRD F14.8 ("tap to view full-size") is built small. Each thumbnail in the quote editor is a button (tap,
Enter or Space) that opens `!PhotoViewer`, a XAML `ContentDialog` route with a model like the other dialogs (D14), with
the photo scaled to fit and a Close button. The thumbnail's delete button stays as the way to delete; long press and
swipe to delete are not built.

The view is as large as Material's dialog allows: 560 px wide at most (a 4:3 photo shows at 512 × 384 px on a desktop
window), and 56 px in from each side of a phone's screen (230 px of photo at 390 px). Raising that limit for this
dialog did not work: Material's dialog template reads `MaterialContentDialogMaxWidth` and `MaxHeight` as static
resources, so neither its own keys nor WinUI's `ContentDialogMaxWidth` set on the dialog change it (two attempts).

What a full-window view would take (not built): a page outside the dialogs. The editor lives in MainPage's Visibility
region, which has no back stack, so a photo page there would have to hand the editor back with its unsaved edits;
that is a navigation change, not a small feature.

# Transfer to uno.labs (2026-11-04)

## D23. A quote icon in place of the Uno template's (proposed, 2026-11-04)

Decision: the launcher, splash and gallery icon is a quote: a white sheet with a folded corner and three lines, with
the amber check (the accent, `SecondaryColor` #F59E0B) on its corner, on the primary slate blue (`PrimaryColor`
#2B4C7E, `Styles/ColorPaletteOverride.xaml`). `Assets/Icons/icon.svg` is the ground, `icon_foreground.svg` the glyph,
and `Assets/Splash/splash_screen.svg` the same glyph in slate on the splash's white. The template's viewBoxes stay; the
glyph spans about 61% of the width and 70% of the height at the foreground scale of 1.0 (desktop, WebAssembly,
Windows) and stays inside Android's adaptive safe zone at 0.65. Checked: the desktop and Android builds pass, and the
Android launcher icon Resizetizer generates shows the glyph whole.

Reason: the owner approved a simple custom icon for the apps that still shipped the template's (info file brief).

Tradeoff: flat and generic, and no wordmark; the in-app "QC" mark stays in the rail and onboarding.

## D24. The `.studiolive` info file links to the planned Uno.Samples home (proposed, 2026-11-04)

Decision: `.studiolive/sample.json` (sample id `quotecraft`) gives `iconUrl` and `screenshots` as
`https://raw.githubusercontent.com/unoplatform/Uno.Samples/master/studio/quotecraft/.studiolive/...`, the folder the
sample moves to after uno.labs, as the format guide's example does (Uno.Samples draft PR #967). They resolve only
after that move.

Reason: the guide asks for absolute URLs; the Studio team has been asked (uno-private#2444, question 1) whether
relative paths can be used instead. If they can, the URLs become `icon.png` and `screenshots/<name>.png`.

Also in the file: `showcase.requires` lists only an email app (Send opens a mailto link). Supabase sign-in and Stripe
billing are left out: the app runs fully without them and hides them (D3).

# Theme pass (studio branch, 2026-11-06)

## D25. The look comes from Uno.Themes: palette, role brushes, semantic styles and tokens (owner rule, 2026-11-05)

Decision: QuoteCraft keeps Material (`MaterialToolkitTheme`) and takes its whole look from it, so Uno Platform Studio's
Themes tab drives the app.

- Colours: the brand's are only in `Styles/ColorPaletteOverride.xaml` (the theme's `Colors`, Light and Dark). Pages,
  cards and dialogs use the role brushes. The slate band is `PrimaryBrush`/`OnPrimaryBrush` under
  `RequestedTheme="Light"` (the design keeps it the same in both modes); the amber call to action is the filled button
  on the Secondary role, whose Dark value is now the same amber, with `OnSecondary` #1A365D in both modes. Status
  pills: Draft and Expired `SurfaceVariant`, Viewed `SecondaryContainer`, Declined `ErrorContainer`; Accepted (Success)
  and Sent have no role and are App.xaml's only Light and Default brushes. `BrandColors.xaml` and `StatusColors.xaml`
  are gone.
- Type: DM Sans is the theme's `DefaultFontFamily`; JetBrains Mono stays an app key for figures.
- Spacing, radius, sizes: `Space*`, `Radius*CornerRadius`, `ControlHeight*`, `IconSize*`, `TouchTargetMinSize`
  (measured: `SpaceN00` = N × 8 px, `RadiusN00` = N × 4 px); `DefaultSpacing="8"` and `DefaultCornerRadius="4"` on the
  theme. `Styles/Layout.xaml` keeps what no token says.
- The PDF keeps print colours (`Services/PrintColors.cs`): paper does not follow Dark mode. It is the theme gate's one
  remaining finding.
- Dialogs follow the theme: `AppDialogStyle` gives the surface through ThemeResource (Material's dialog style uses a
  StaticResource), and each dialog takes the window's theme (`Helpers/DialogTheme`).

Reason: an app's own colour, spacing and radius keys outrank the theme, so Studio's palette edits did not reach the
screen (Cargo, 2026-11-05); the owner's rule is that samples use Uno.Themes and the libraries' resources.

Tradeoff: small visible shifts. The selected rail item is the `PrimaryContainer` pair (its label darker in Light);
Declined in Dark is the brighter `ErrorContainer`; table stripes are `BackgroundBrush`; the band's gradient is solid;
the accent button takes the theme's padding (8 px wider); icons 18 → 16, 20 → 24, 36 → 32; Settings' section gap
28 → 24. The status pill keeps an 8 px radius: `RadiusFull` draws an oval on Skia.
