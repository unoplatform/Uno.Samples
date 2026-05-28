# QuoteCraft - Architecture Brief

## Technology Stack

| Layer | Technology | Rationale |
|-------|-----------|-----------|
| **UI Framework** | Uno Platform (Skia renderer) | Cross-platform from single codebase: WASM, iOS, Android, Desktop |
| **Project Model** | Uno.Sdk Single Project | One .csproj, all platforms. Simplest build and maintenance. |
| **UI Pattern** | MVUX (Model-View-Update-eXtended) | Reactive, immutable records, less boilerplate than MVVM. Feeds + States for async data. |
| **Design System** | Uno Material + Uno Toolkit | Material Design 3 styling, AutoLayout, Card, TabBar, NavigationBar, ChipGroup, DrawerControl |
| **Navigation** | Uno Navigation Extensions | XAML-based region navigation. Route definitions in code, navigation via attached properties. |
| **Local Database** | SQLite via `Microsoft.Data.Sqlite` | Offline-first storage. Supported on all Uno Platform targets including WASM. |
| **Cloud Backend** | Supabase (Postgres + Auth + Storage) | Free tier for MVP. Auth, real-time database, file storage (logos). No custom server needed. |
| **HTTP** | `Uno.Extensions.Http.Kiota` or raw `HttpClient` | Kiota for typed Supabase API calls, or raw HTTP with `System.Text.Json` serialization. |
| **PDF Generation** | QuestPDF | Free for <$1M revenue. Fluent C# API for PDF creation. Runs on all platforms. |
| **Payments** | Stripe Checkout (via Supabase Edge Functions) | No PCI burden. Server-side session creation, client-side redirect. |
| **DI / Hosting** | Uno.Extensions.Hosting | `IHostBuilder` for dependency injection, configuration, and logging. |
| **Logging** | `Uno.Extensions.Logging` + `ILogger` | Structured logging for debugging across platforms. |
| **Configuration** | `Uno.Extensions.Configuration` | `appsettings.json` for environment-specific settings (Supabase URL, Stripe keys). |
| **Storage** | `Uno.Extensions.Storage` | Secure local storage for tokens, user preferences. |
| **Serialization** | `Uno.Extensions.Serialization` | JSON serialization for API communication and local data. |

---

## Target Platforms

| Platform | TFM | Priority | Notes |
|----------|-----|----------|-------|
| **WebAssembly** | `net9.0-browserwasm` | P0 | Primary distribution. No app store needed. Instant access via URL. |
| **Android** | `net9.0-android` | P0 | Most contractors use Android phones. |
| **iOS** | `net9.0-ios` | P1 | iPhone users. Requires Apple Developer account ($99/yr). |
| **Windows** | `net9.0-windows10.0.19041` | P2 | Desktop management. Nice-to-have for office use. |
| **macOS** | `net9.0-desktop` | P3 | Low priority. Desktop Skia target covers this. |
| **Linux** | `net9.0-desktop` | P3 | Low priority. Desktop Skia target covers this. |

---

## UnoFeatures Configuration

```xml
<UnoFeatures>
    Material;
    Toolkit;
    Extensions;
    ExtensionsCore;
    Hosting;
    MVUX;
    Navigation;
    Http;
    Storage;
    Serialization;
    Logging;
    ThemeService;
    Skia;
</UnoFeatures>
```

---

## Project Structure

```
QuoteCraft/
├── QuoteCraft.sln
├── QuoteCraft/                           # Single Project (all platforms)
│   ├── QuoteCraft.csproj
│   ├── App.xaml / App.xaml.cs            # App startup, DI registration
│   ├── appsettings.json                  # Supabase URL, Stripe key, defaults
│   ├── appsettings.development.json      # Local overrides
│   │
│   ├── Assets/                           # Images, icons, splash screens
│   │   ├── Icons/
│   │   └── Images/
│   │
│   ├── Themes/                           # Style overrides and resource dictionaries
│   │   ├── ColorPaletteOverride.xaml      # Brand colors
│   │   ├── TextBlock.xaml                 # Typography overrides if needed
│   │   └── CustomStyles.xaml              # App-specific control styles
│   │
│   ├── Strings/                          # Localization
│   │   └── en/
│   │       └── Resources.resw
│   │
│   ├── Models/                           # Domain models (immutable records)
│   │   ├── Quote.cs
│   │   ├── LineItem.cs
│   │   ├── Client.cs
│   │   ├── CatalogItem.cs
│   │   ├── BusinessProfile.cs
│   │   └── UserSubscription.cs
│   │
│   ├── Services/                         # Business logic and data access
│   │   ├── IQuoteService.cs              # Quote CRUD operations
│   │   ├── QuoteService.cs
│   │   ├── IClientService.cs
│   │   ├── ClientService.cs
│   │   ├── ICatalogService.cs
│   │   ├── CatalogService.cs
│   │   ├── IPdfService.cs               # PDF generation
│   │   ├── PdfService.cs
│   │   ├── ISyncService.cs              # Local <-> Supabase sync
│   │   ├── SyncService.cs
│   │   ├── IAuthService.cs              # Authentication
│   │   ├── AuthService.cs
│   │   ├── ISubscriptionService.cs      # Tier management
│   │   ├── SubscriptionService.cs
│   │   ├── IPhotoService.cs             # Photo capture, compression, storage
│   │   └── PhotoService.cs
│   │
│   ├── Data/                             # Data layer
│   │   ├── Local/
│   │   │   ├── Database.cs               # SQLite setup, migrations
│   │   │   ├── QuoteRepository.cs
│   │   │   ├── ClientRepository.cs
│   │   │   └── CatalogRepository.cs
│   │   └── Remote/
│   │       ├── SupabaseClient.cs         # Supabase HTTP client wrapper
│   │       ├── SupabaseQuoteApi.cs
│   │       ├── SupabaseClientApi.cs
│   │       └── SupabaseAuthApi.cs
│   │
│   ├── Presentation/                     # UI layer (Views + MVUX Models)
│   │   ├── Shell/
│   │   │   ├── ShellPage.xaml            # Root shell with NavigationView/TabBar
│   │   │   └── ShellModel.cs
│   │   ├── Dashboard/
│   │   │   ├── DashboardPage.xaml        # Quote list with filters
│   │   │   └── DashboardModel.cs
│   │   ├── QuoteBuilder/
│   │   │   ├── QuoteBuilderPage.xaml     # Core quote editing
│   │   │   ├── QuoteBuilderModel.cs
│   │   │   ├── LineItemSheet.xaml        # Bottom sheet for line items
│   │   │   ├── CatalogSheet.xaml         # Bottom sheet for catalog browser
│   │   │   └── PhotoGallery.xaml         # Photo attachment thumbnail gallery
│   │   ├── QuotePreview/
│   │   │   ├── QuotePreviewPage.xaml     # PDF-like preview
│   │   │   └── QuotePreviewModel.cs
│   │   ├── Clients/
│   │   │   ├── ClientListPage.xaml
│   │   │   ├── ClientListModel.cs
│   │   │   ├── ClientDetailPage.xaml
│   │   │   └── ClientDetailModel.cs
│   │   ├── Settings/
│   │   │   ├── SettingsPage.xaml
│   │   │   └── SettingsModel.cs
│   │   └── Onboarding/
│   │       ├── WelcomePage.xaml           # First-run setup
│   │       └── WelcomeModel.cs
│   │
│   └── Helpers/                          # Utilities
│       ├── CurrencyFormatter.cs
│       ├── QuoteNumberGenerator.cs
│       └── ConnectivityHelper.cs
│
├── QuoteCraft.Tests/                     # Unit tests
│   ├── QuoteCraft.Tests.csproj
│   ├── Services/
│   │   ├── QuoteServiceTests.cs
│   │   ├── PdfServiceTests.cs
│   │   └── SyncServiceTests.cs
│   └── Models/
│       └── QuoteCalculationTests.cs
│
└── supabase/                             # Supabase project config
    ├── migrations/                       # SQL migrations
    │   ├── 001_create_tables.sql
    │   └── 002_row_level_security.sql
    └── functions/                        # Edge Functions
        ├── create-checkout-session/       # Stripe Checkout
        ├── stripe-webhook/                # Payment confirmation
        └── quote-view/                    # Client-facing quote page (server-rendered HTML)
```

---

## Domain Models (Immutable Records)

MVUX uses immutable records. All state changes produce new instances.

```csharp
// Core domain models

public record Quote(
    Guid Id,
    string Title,
    Guid ClientId,
    ImmutableList<LineItem> LineItems,
    ImmutableList<QuotePhoto> Photos,
    string Notes,
    decimal TaxRate,
    string CurrencyCode,          // "USD" or "CAD"
    QuoteStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? SentAt,
    DateTimeOffset? ValidUntil,
    string QuoteNumber
);

public record QuotePhoto(
    Guid Id,
    Guid QuoteId,
    string LocalPath,             // Local file path
    string? RemoteUrl,            // Supabase Storage URL (Pro)
    int SortOrder,
    DateTimeOffset CreatedAt
);

public record LineItem(
    Guid Id,
    string Description,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal  // Computed: UnitPrice * Quantity
);

public record Client(
    Guid Id,
    string Name,
    string? Email,
    string? Phone,
    string? Address
);

public record CatalogItem(
    Guid Id,
    string TradeCategory,    // "Plumbing", "Electrical", etc.
    string ItemCategory,     // "Installations", "Repairs", etc.
    string Description,
    decimal DefaultPrice
);

public record BusinessProfile(
    string BusinessName,
    string? Phone,
    string? Email,
    string? Address,
    string? LogoPath,
    string? PrimaryColor,
    decimal DefaultTaxRate,
    string CurrencyCode,          // "USD" or "CAD"
    int QuoteValidDays,
    string QuoteNumberPrefix,
    string? CustomFooter
);

public enum QuoteStatus
{
    Draft,
    Sent,
    Viewed,
    Accepted,
    Declined,
    Expired
}
```

---

## Offline-First Architecture

SQLite is the source of truth. Supabase is the sync target.

### Data Flow

```
User Action
    |
    v
MVUX Model (State/Feed)
    |
    v
Service Layer (IQuoteService)
    |
    v
Local Repository (SQLite)     <-- Always writes here first
    |
    v
Sync Service (background)     <-- Pushes to Supabase when online
    |
    v
Supabase (Postgres)           <-- Cloud backup + cross-device sync
```

### Sync Strategy

1. **Write-local-first:** Every create/update/delete goes to SQLite immediately. UI updates instantly.
2. **Background sync:** A `SyncService` runs periodically (every 30 seconds when online) and pushes unsynced changes to Supabase.
3. **Change tracking:** Each local record has `synced_at` and `updated_at` timestamps. If `updated_at > synced_at`, the record needs syncing.
4. **Conflict resolution:** Last-write-wins. For a solo contractor this is fine. For Business tier (multi-user), server timestamp wins.
5. **Pull on launch:** On app start, pull any changes from Supabase that are newer than the last sync timestamp.
6. **Connectivity awareness:** Use `ConnectivityHelper` to detect online/offline. Show subtle banner when offline.

### SQLite Schema

```sql
CREATE TABLE quotes (
    id TEXT PRIMARY KEY,
    title TEXT NOT NULL,
    client_id TEXT REFERENCES clients(id),
    notes TEXT,
    tax_rate REAL NOT NULL DEFAULT 0,
    status TEXT NOT NULL DEFAULT 'draft',
    quote_number TEXT NOT NULL,
    created_at TEXT NOT NULL,
    sent_at TEXT,
    valid_until TEXT,
    updated_at TEXT NOT NULL,
    synced_at TEXT,
    is_deleted INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE line_items (
    id TEXT PRIMARY KEY,
    quote_id TEXT NOT NULL REFERENCES quotes(id),
    description TEXT NOT NULL,
    unit_price REAL NOT NULL,
    quantity INTEGER NOT NULL DEFAULT 1,
    sort_order INTEGER NOT NULL DEFAULT 0,
    updated_at TEXT NOT NULL,
    synced_at TEXT
);

CREATE TABLE clients (
    id TEXT PRIMARY KEY,
    name TEXT NOT NULL,
    email TEXT,
    phone TEXT,
    address TEXT,
    updated_at TEXT NOT NULL,
    synced_at TEXT,
    is_deleted INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE catalog_items (
    id TEXT PRIMARY KEY,
    trade_category TEXT NOT NULL,
    item_category TEXT NOT NULL,
    description TEXT NOT NULL,
    default_price REAL NOT NULL,
    is_custom INTEGER NOT NULL DEFAULT 0,
    updated_at TEXT NOT NULL,
    synced_at TEXT
);

CREATE TABLE quote_photos (
    id TEXT PRIMARY KEY,
    quote_id TEXT NOT NULL REFERENCES quotes(id),
    local_path TEXT NOT NULL,
    remote_url TEXT,
    sort_order INTEGER NOT NULL DEFAULT 0,
    created_at TEXT NOT NULL,
    synced_at TEXT
);

CREATE TABLE business_profile (
    id TEXT PRIMARY KEY DEFAULT 'default',
    business_name TEXT,
    phone TEXT,
    email TEXT,
    address TEXT,
    logo_path TEXT,
    primary_color TEXT,
    default_tax_rate REAL NOT NULL DEFAULT 0,
    currency_code TEXT NOT NULL DEFAULT 'USD',
    quote_valid_days INTEGER NOT NULL DEFAULT 14,
    quote_number_prefix TEXT NOT NULL DEFAULT 'QC-',
    custom_footer TEXT,
    updated_at TEXT NOT NULL,
    synced_at TEXT
);

CREATE TABLE sync_metadata (
    key TEXT PRIMARY KEY,
    value TEXT NOT NULL
);
```

---

## Supabase Configuration

### Database Tables

Mirror the SQLite schema in Postgres with added columns:

- `user_id UUID REFERENCES auth.users(id)` on all tables
- Row Level Security (RLS) policies: users can only access their own data
- `created_at TIMESTAMPTZ DEFAULT now()` server-side timestamps

### Row Level Security

```sql
-- Example for quotes table
ALTER TABLE quotes ENABLE ROW LEVEL SECURITY;

CREATE POLICY "Users can only see their own quotes"
    ON quotes FOR SELECT
    USING (auth.uid() = user_id);

CREATE POLICY "Users can only insert their own quotes"
    ON quotes FOR INSERT
    WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Users can only update their own quotes"
    ON quotes FOR UPDATE
    USING (auth.uid() = user_id);
```

### Auth

- Email/password signup (simplest for contractors)
- Magic link as alternative (no password to remember)
- Supabase Auth handles JWT tokens
- Store token securely via `Uno.Extensions.Storage`

### Storage

- Supabase Storage bucket for business logos and quote photos
- Upload logo/photos from device, store URL in database
- Logos: public bucket (visible on shared quote PDFs and client pages)
- Photos: per-user bucket with RLS (accessible via signed URLs on client-facing pages)
- Photo compression: client-side before upload (max 1 MB per image, JPEG at 80% quality)

### Edge Functions

Three Deno edge functions:

1. **`create-checkout-session`**: Creates a Stripe Checkout Session, returns URL
2. **`stripe-webhook`**: Receives Stripe webhook, updates user subscription tier in database
3. **`quote-view`**: Client-facing quote page. Receives a short ID, fetches quote data from Postgres, renders server-side HTML with inline CSS. Includes:
   - Professional quote layout matching the PDF design
   - Photo gallery (if attached)
   - Accept/Decline buttons (POST back to update quote status)
   - PDF download link (triggers server-side PDF generation)
   - "Viewed" status tracking on page load
   - No authentication required for the client (access via unique short URL with expiry token)

---

## Navigation Architecture

### Route Map

```csharp
// Route registration in App.xaml.cs or host builder setup

new RouteMap(
    new RouteMap("Shell", View: views.FindByViewModel<ShellModel>,
        Nested: new RouteMap[]
        {
            new("Dashboard", View: views.FindByViewModel<DashboardModel>),
            new("QuoteBuilder", View: views.FindByViewModel<QuoteBuilderModel>),
            new("QuotePreview", View: views.FindByViewModel<QuotePreviewModel>),
            new("ClientList", View: views.FindByViewModel<ClientListModel>),
            new("ClientDetail", View: views.FindByViewModel<ClientDetailModel>),
            new("Settings", View: views.FindByViewModel<SettingsModel>),
            new("Welcome", View: views.FindByViewModel<WelcomeModel>),
        }
    )
);
```

### Navigation Flow

```
App Launch
    |
    v
[Has completed onboarding?]
    |           |
   No          Yes
    |           |
    v           v
Welcome     Shell (Dashboard)
    |           |
    v           +-> QuoteBuilder -> QuotePreview
  Shell         +-> ClientList -> ClientDetail
                +-> Settings
```

### Navigation via XAML

All navigation uses attached properties, never code-behind:

```xml
<!-- Navigate to QuoteBuilder with quote ID -->
<Button Content="Edit"
        uen:Navigation.Request="QuoteBuilder"
        uen:Navigation.Data="{Binding QuoteId}" />
```

---

## PDF Generation (QuestPDF)

### Approach

- Generate PDF as byte array in `PdfService`
- Save to local temp file
- Share via platform share sheet or email
- On WASM: trigger browser download

### PDF Template Structure

```
+------------------------------------------+
| [Logo]    Business Name                   |
|           Phone | Email                   |
|           Address                         |
+------------------------------------------+
| QUOTE #QC-2026-0042                      |
| Date: Feb 11, 2026                        |
| Valid Until: Feb 25, 2026                  |
+------------------------------------------+
| Prepared for:                             |
| Client Name                               |
| Client Address                            |
+------------------------------------------+
| Description        Qty    Unit    Total   |
|-------------------------------------------|
| Sink Installation    1   $350   $350.00   |
| Copper Pipe (ft)    25    $12   $300.00   |
|-------------------------------------------|
|                    Subtotal:    $650.00    |
|                    Tax (8.5%):   $55.25    |
|                    TOTAL:       $705.25    |
+------------------------------------------+
| Notes:                                    |
| Includes all materials and labor...       |
+------------------------------------------+
| Created with QuoteCraft                   |  <- Free tier only
+------------------------------------------+
```

---

## Dependency Injection Setup

```csharp
// In App.xaml.cs host builder configuration

private static IHost BuildHost()
{
    return UnoHost
        .CreateDefaultBuilder()
        .Configure(hostBuilder =>
        {
            hostBuilder
                .UseConfiguration()
                .UseLogging()
                .UseSerialization()
                .UseHttp()
                .UseNavigation(RegisterRoutes)

                // Services
                .ConfigureServices(services =>
                {
                    // Data layer
                    services.AddSingleton<Database>();
                    services.AddSingleton<IQuoteRepository, QuoteRepository>();
                    services.AddSingleton<IClientRepository, ClientRepository>();
                    services.AddSingleton<ICatalogRepository, CatalogRepository>();

                    // Remote
                    services.AddSingleton<SupabaseClient>();

                    // Services
                    services.AddSingleton<IQuoteService, QuoteService>();
                    services.AddSingleton<IClientService, ClientService>();
                    services.AddSingleton<ICatalogService, CatalogService>();
                    services.AddSingleton<IPdfService, PdfService>();
                    services.AddSingleton<ISyncService, SyncService>();
                    services.AddSingleton<IAuthService, AuthService>();
                    services.AddSingleton<ISubscriptionService, SubscriptionService>();
                    services.AddSingleton<IPhotoService, PhotoService>();
                });
        })
        .Build();
}
```

---

## Security Considerations

| Concern | Mitigation |
|---------|------------|
| Auth tokens | Stored via `Uno.Extensions.Storage` (platform secure storage) |
| Supabase keys | `anon` key in config (safe for client-side; RLS enforces access) |
| Stripe keys | Secret key only in Supabase Edge Functions (server-side). Publishable key in client. |
| Client data | RLS ensures users only see their own data. No cross-tenant access. |
| PDF links | Unique per-quote URLs with short-lived tokens for client viewing |
| Offline data | SQLite stored in app sandbox. No encryption for MVP (consider SQLCipher later). |

---

## Performance Targets

| Metric | Target |
|--------|--------|
| App launch to dashboard | < 2 seconds |
| Create new quote (tap to ready) | < 500ms |
| Add line item | < 200ms (local only) |
| Generate PDF | < 3 seconds |
| Sync cycle | < 5 seconds for 50 quotes |
| WASM bundle size | < 15 MB (first load), cached after |

---

## Testing Strategy

| Type | Tool | Coverage |
|------|------|----------|
| Unit tests | xUnit + NSubstitute | Services, calculations, model logic |
| Model tests | xUnit | Quote totals, tax calculations, status transitions |
| Integration tests | xUnit + SQLite in-memory | Repository layer against real SQLite |
| UI tests | Manual (MVP) | Key flows on WASM + Android |
| PDF tests | xUnit + file comparison | PDF generation produces expected output |

---

## Build & Deploy

### WebAssembly (Primary)
- Build: `dotnet publish -f net9.0-browserwasm -c Release`
- Host: Netlify or Azure Static Web Apps (free tier)
- CI: GitHub Actions on push to `main`

### Android
- Build: `dotnet publish -f net9.0-android -c Release`
- Distribute: Google Play Store (or direct APK for beta)
- Signing: keystore in CI secrets

### iOS
- Build: `dotnet publish -f net9.0-ios -c Release`
- Distribute: TestFlight (beta), App Store (release)
- Requires Apple Developer account ($99/year)
