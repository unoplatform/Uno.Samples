# QuoteCraft

- **Owner:** Matthew Mattei (@noknowscode)
- **Status:** ready for review
- **What it is:** Quotes for contractors: line items from a catalog, a PDF on every platform and a status lifecycle, local-first.

A professional quoting app for contractors and small businesses, built with [Uno Platform](https://platform.uno) and .NET 10.

QuoteCraft helps service professionals create, manage, and send quotes to clients, all from a single cross-platform application that runs on Windows, macOS, Linux, iOS, Android, and WebAssembly.

![QuoteCraft: the quotes list with a quote open](screenshots/quotes.png)

## Features

- **Quote Management:** Create, edit, duplicate, and track quotes with status indicators (Draft, Sent, Accepted, Declined, Expired); sent quotes expire after their validity period, with a reminder 3 days before (the rail's bell, or Alerts in the bottom tab bar on a phone), and an expired quote can be reopened and sent again
- **Client Directory:** Maintain a searchable client database with contact details and quote history
- **Service Catalog:** Organize services and materials into categories (Electrical, Plumbing, HVAC, etc.) with preset pricing
- **PDF Generation:** Generate professional PDF quotes ready to send to clients, on every platform (saved, downloaded or offered to the system's save dialog; see [Platform notes](#platform-notes))
- **Email Integration:** Send quotes directly to clients via email
- **Dashboard Analytics:** At-a-glance metrics including monthly totals, send counts, and acceptance rates with sparkline charts
- **Quote Editor:** Line-item editor with quantity and pricing, the quote's tax rate and valid-until date (both start at the Settings defaults), notes and job photos; subtotal, tax and total as the PDF shows them, and the quote's status history. Catalog items come in with the markup set in Settings, and the PDF prints the Settings custom footer
- **Light, Dark or System theme:** Follows the system by default; choose Light or Dark in Settings (Appearance) or from the side rail's Theme menu, and the choice is remembered
- **Responsive Layout:** Four layouts, from a 360 px phone to a wide desktop: bottom tabs and one pane below 800 px, the side rail from 800 px, list and detail side by side from 1080 px (see [Layouts](#layouts))
- **Business Profile:** Configure company name, phone, email, address, logo (upload or remove), and default quote settings, with a preview of a new quote that follows the fields as they are typed (from 1400 px)
- **Onboarding Flow:** Guided setup for new users to configure their business profile

## Screenshots

| Clients | Catalog | Settings | A quote on a phone |
|:-------:|:-------:|:--------:|:------------------:|
| ![Clients](screenshots/clients.png) | ![Catalog](screenshots/catalog.png) | ![Settings](screenshots/settings.png) | ![A quote on a phone](screenshots/quote-phone.png) |

Debug desktop build, sample data (`--qc-seed=sample`), Light theme, 1280 × 800 and 390 × 844.

## Tech Stack

- **Framework:** [Uno Platform](https://platform.uno) 7.0 preview (`Uno.Sdk` 7.0.0-dev.115, Single Project)
- **Runtime:** .NET 10
- **Renderer:** Skia
- **UI:** WinUI / XAML with Uno Toolkit controls
- **Design System:** Material Design 3 through Uno.Themes (`MaterialToolkitTheme`): the brand palette in `Styles/ColorPaletteOverride.xaml` (the file Uno Platform Studio's Themes tab edits), role brushes, semantic text and control styles, DM Sans as the theme's `DefaultFontFamily`, and the theme's `Space*`, `Radius*` and `ControlHeight*` tokens
- **Architecture:** MVUX (Model-View-Update-eXtended)
- **Navigation:** Uno Navigation Extensions (region-based)
- **Dependency Injection:** Microsoft.Extensions.Hosting
- **Database:** SQLite, encrypted with SQLCipher (Microsoft.Data.Sqlite + SQLitePCLRaw), local-first; the key in the platform's vault (`PasswordVault`) on Android, iOS and Windows (WinAppSDK)
- **PDF:** SkiaSharp's PDF backend (`SKDocument`), set in the app's DM Sans; no extra package
- **Cloud (optional):** Supabase sign-in and Stripe billing (cloud sync is off in this version); see [Local-first, with optional sign-in and billing](#local-first-with-optional-sign-in-and-billing)

## Project Structure

```
├── QuoteCraft.sln
├── global.json
├── .studiolive/                         # Showcase gallery info: sample.json, agent context, conversation, icon, screenshots
├── Directory.Build.props
├── Directory.Packages.props
├── docs/                                # Production-readiness decisions (DECISIONS-production-readiness.md)
└── QuoteCraft/
    ├── QuoteCraft.csproj
    ├── App.xaml / App.xaml.cs          # App shell, DI, routing, styles
    ├── Presentation/                    # Pages and MVUX models
    │   ├── Shell.xaml                   # Splash screen container
    │   ├── MainPage.xaml                # Side rail + content region layout
    │   ├── DashboardPage.xaml           # Quote list with analytics (DashboardModel)
    │   ├── QuoteDetailView.xaml         # The open quote, in the Quotes page's detail region (QuoteDetailModel)
    │   ├── ClientsPage.xaml             # Client directory
    │   ├── CatalogPage.xaml             # Service/material catalog
    │   ├── SettingsPage.xaml            # Business profile & quote config
    │   ├── QuoteEditorPage.xaml         # Line-item quote editor (clients and catalog items edit in dialogs)
    │   └── OnboardingPage.xaml          # First-run setup wizard
    ├── Models/                          # Data entities
    ├── Data/                            # SQLite database & repositories
    ├── Services/                        # PDF, email, sharing, feature gates, quote creation, status and expiry rules
    ├── Styles/                          # The palette (ColorPaletteOverride.xaml), card styles, layout values with no theme token
    ├── Helpers/                         # Value converters & utilities
    ├── Controls/                        # Custom controls (sparkline, status pill)
    └── Assets/                          # Fonts (DM Sans, JetBrains Mono), icons
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Uno Platform extension](https://platform.uno/docs/articles/get-started.html) for your IDE (Visual Studio, VS Code, or Rider)
- Run `uno-check` to verify your environment:
  ```bash
  dotnet tool install -g uno.check
  uno-check
  ```

## Getting Started

1. **Clone the repository**
   ```bash
   git clone https://github.com/mtmattei/QuoteCraft.git
   cd QuoteCraft
   ```

2. **Restore and build**
   ```bash
   dotnet build QuoteCraft/QuoteCraft.csproj -f net10.0-desktop
   ```

3. **Run the app**
   ```bash
   dotnet run --project QuoteCraft/QuoteCraft.csproj -f net10.0-desktop
   ```

4. **Tests**
   This folder holds the one app project Studio opens. The unit tests (301: the page and dialog models, money,
   numbering, migrations, status and expiry rules, quote creation, the PDF and the stored secrets) live in the
   development repository, [noknowscode/QuoteCraft](https://github.com/noknowscode/QuoteCraft).
   `QuoteCraftTargetFrameworks` limits the app to one head, so a restore needs no mobile workload.

### Target Frameworks

| Platform | Target Framework |
|----------|-----------------|
| Windows / macOS / Linux (Skia) | `net10.0-desktop` |
| Windows (WinAppSDK, packaged) | `net10.0-windows10.0.19041` (x64 or ARM64) |
| Android | `net10.0-android` |
| iOS | `net10.0-ios` |
| WebAssembly | `net10.0-browserwasm` |

iOS builds on macOS and the WinAppSDK head on Windows; elsewhere the SDK skips them.

### Platform notes

| | Desktop | Windows (WinAppSDK) | Android | iOS | WebAssembly |
|---|---|---|---|---|---|
| Local database | SQLCipher, encrypted | SQLCipher, encrypted | SQLCipher, encrypted | SQLCipher, encrypted | SQLite in IndexedDB, not encrypted |
| Database key and sign-in token | Files in the app's data folder | `PasswordVault` (Credential Locker) | `PasswordVault` (AndroidKeyStore) | `PasswordVault` (Keychain) | No key; token in a file in IndexedDB |
| PDF export | Written to `exports/` and opened | Written to `exports/` and opened | Save dialog | Export sheet (Save to Files) | Browser download |

An older install's `.dbkey` and `.auth_token` files move into the vault at the first start of this version. The
reasons are in [docs/DECISIONS-production-readiness.md](docs/DECISIONS-production-readiness.md) (D1, D12, D13).

## Layouts

Every page uses the Uno Toolkit's `utu:Responsive` with the default breakpoints:

| Category | Window width | Navigation | Quotes, Clients, Catalog | Settings, editor, onboarding |
|---|---|---|---|---|
| Narrow | below 600 | bottom tabs, with Alerts (notifications) | one pane: the list, or the picked item with a back button; search under the title | one column, 16 px sides |
| Normal | 600 to 800 | bottom tabs, with Alerts (notifications) | one pane, header on one row | one column, 24 px sides |
| Wide | 800 to 1080 | side rail | one pane, stat cards above the list | one column, capped and centred |
| Widest | 1080 and up | side rail | list and detail side by side, page capped at 1600 px | Settings adds the quote preview from 1400 px |

The look comes from the Material theme. Colours are the palette's roles (`Styles/ColorPaletteOverride.xaml`, Light and
Dark), used through their brushes; the two status colours with no role (accepted and sent) are App.xaml's own Light and
Default brushes. Spacing, radii and control sizes are the theme's `Space*`, `Radius*` and `ControlHeight*` tokens;
`QuoteCraft/Styles/Layout.xaml` keeps only what no token says (two-axis insets, page frames, widths, the 1400 px split).
The PDF prints in fixed colours (`Services/PrintColors.cs`).

## Local-first, with optional sign-in and billing

QuoteCraft runs entirely on the device: quotes, clients, the catalog and settings live in the local database, and no
account is needed. Without a Supabase project there are no plans, so no quote or client limits and no watermark;
Settings shows that data stays on this device.

Sign-in, plans and billing appear only when the app is given a Supabase project:

1. Create a Supabase project and apply `supabase/migrations` (tables, then row-level security).
2. Deploy `supabase/functions` and set their secrets as listed in [supabase/README.md](supabase/README.md).
3. Put the project URL and **anon** key in `QuoteCraft/appsettings.json`:
   ```json
   "Supabase": {
     "Url": "https://YOUR-PROJECT.supabase.co",
     "AnonKey": "YOUR-ANON-KEY",
     "CustomerPortalUrl": "https://billing.stripe.com/p/login/YOUR-PORTAL-LINK"
   }
   ```
   On desktop, `QUOTECRAFT_SUPABASE_URL` and `QUOTECRAFT_SUPABASE_ANON_KEY` override the file.

The anon key is meant for clients (row-level security protects the data). Never put the service-role key in the
app: it bypasses row-level security and belongs to the Edge Functions only. Keep real values out of commits.

**Cloud sync is off in this version, with or without a project:** quotes, clients, the catalog and settings stay on the
device, and Settings says so. The sync service would upload quotes without their line items (so they arrive at $0),
and never the catalog or the business profile; what it needs before it is turned on is in
[docs/DECISIONS-production-readiness.md](docs/DECISIONS-production-readiness.md) (D8). Other gaps with a project
configured: on desktop and in the browser the sign-in refresh token is kept in a file in the app's data folder
(Android, iOS and Windows keep it in `PasswordVault`). Settings offers Sign In while nobody is signed in, and the
yearly plan beside the monthly one (D17).

## UnoFeatures

The project uses the following [Uno SDK features](https://aka.platform.uno/singleproject-features):

| Feature | Purpose |
|---------|---------|
| Material | Material Design 3 theme and styles |
| Toolkit | Extended UI controls (TabBar, AutoLayout, Card, etc.) |
| MVUX | Reactive state management with feeds and states |
| Navigation | Region-based declarative navigation |
| Hosting | Microsoft.Extensions dependency injection |
| Configuration | App configuration and settings |
| Logging | Structured logging |
| SkiaRenderer | Cross-platform Skia rendering backend |

## License

This project is provided as-is for demonstration and educational purposes.
