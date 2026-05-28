# QuoteCraft

A professional quoting and invoicing app for contractors and small businesses, built with [Uno Platform](https://platform.uno) and .NET 10.

QuoteCraft helps service professionals create, manage, and send quotes to clients — all from a single cross-platform application that runs on Windows, macOS, Linux, iOS, Android, and WebAssembly.

![QuoteCraft Dashboard](screenshots/dashboard-v2.png)

## Features

- **Quote Management** — Create, edit, duplicate, and track quotes with status indicators (Draft, Sent, Viewed, Accepted, Declined, Expired)
- **Client Directory** — Maintain a searchable client database with contact details and quote history
- **Service Catalog** — Organize services and materials into categories (Electrical, Plumbing, HVAC, etc.) with preset pricing
- **PDF Generation** — Generate professional PDF quotes ready to send to clients
- **Email Integration** — Send quotes directly to clients via email
- **Dashboard Analytics** — At-a-glance metrics including monthly totals, send counts, and acceptance rates with sparkline charts
- **Quote Editor** — Line-item editor with quantity, pricing, tax rate, markup, and custom footer support
- **Dark Mode** — Toggle between light and dark themes from the side rail
- **Responsive Layout** — Adaptive navigation with a side rail on desktop and bottom tab bar on mobile
- **Business Profile** — Configure company name, phone, email, address, logo, and default quote settings
- **Onboarding Flow** — Guided setup for new users to configure their business profile

## Screenshots

| Dashboard | Clients | Catalog | Settings |
|:---------:|:-------:|:-------:|:--------:|
| ![Dashboard](screenshots/dashboard-v2.png) | ![Clients](screenshots/nav-clients-after-chip.png) | ![Catalog](screenshots/nav-catalog-page.png) | ![Settings](screenshots/nav-settings-page.png) |

## Tech Stack

- **Framework:** [Uno Platform](https://platform.uno) 6.5 (Single Project)
- **Runtime:** .NET 10
- **Renderer:** Skia
- **UI:** WinUI / XAML with Uno Toolkit controls
- **Design System:** Material Design 3 (Uno Material)
- **Architecture:** MVUX (Model-View-Update-eXtended)
- **Navigation:** Uno Navigation Extensions (region-based)
- **Dependency Injection:** Microsoft.Extensions.Hosting
- **Database:** SQLite (via Microsoft.Data.Sqlite + SQLitePCLRaw)
- **PDF:** QuestPDF

## Project Structure

```
src/
├── QuoteCraft.sln
├── global.json
├── Directory.Build.props
├── Directory.Packages.props
└── QuoteCraft/
    ├── QuoteCraft.csproj
    ├── App.xaml / App.xaml.cs          # App shell, DI, routing, styles
    ├── Presentation/                    # Pages and MVUX models
    │   ├── Shell.xaml                   # Splash screen container
    │   ├── MainPage.xaml                # Side rail + content region layout
    │   ├── DashboardPage.xaml           # Quote list with analytics
    │   ├── ClientsPage.xaml             # Client directory
    │   ├── CatalogPage.xaml             # Service/material catalog
    │   ├── SettingsPage.xaml            # Business profile & quote config
    │   ├── QuoteEditorPage.xaml         # Line-item quote editor
    │   ├── ClientEditorPage.xaml        # Add/edit client
    │   ├── CatalogEditorPage.xaml       # Add/edit catalog item
    │   └── OnboardingPage.xaml          # First-run setup wizard
    ├── Models/                          # Data entities
    ├── Data/                            # SQLite database & repositories
    ├── Services/                        # PDF, email, sharing, feature gates
    ├── Styles/                          # Color palette, typography, status colors
    ├── Helpers/                         # Value converters & utilities
    ├── Controls/                        # Custom controls (sparklines, etc.)
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
   cd QuoteCraft/src
   ```

2. **Restore and build**
   ```bash
   dotnet build -f net10.0-desktop
   ```

3. **Run the app**
   ```bash
   dotnet run --project QuoteCraft/QuoteCraft.csproj -f net10.0-desktop
   ```

### Target Frameworks

| Platform | Target Framework |
|----------|-----------------|
| Windows / macOS / Linux | `net10.0-desktop` |
| Android | `net10.0-android` |
| WebAssembly | `net10.0-browserwasm` |

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
| Http | HTTP client integration |
| Serialization | JSON serialization |
| Logging | Structured logging |
| SkiaRenderer | Cross-platform Skia rendering backend |

## License

This project is provided as-is for demonstration and educational purposes.
