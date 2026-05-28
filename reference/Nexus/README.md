# 🏭 Nexus — *Industrial Control System*

> An industrial-SCADA-style monitoring dashboard: production lines, maintenance, alerts, KPIs — dark / light theme aware.

<img src="../screenshots/Nexus/Nexus/Screenshot%202026-05-27%20161845.png" alt="Nexus SCADA overview" width="640" />

## What you get
The sample to copy for **complete FeedView state coverage** and a convincing operations-console identity.

## Highlights
- **`FeedView` with all four templates** — `Progress` + `Error` + `None` + `Value` populated on every list and metric surface. Exemplary state coverage.
- **MVUX model layer** — `OverviewModel` is a `partial record` with `IFeed<DashboardMetrics>` / `IListFeed<ProductionLine>` / `IListFeed<Alert>` from a singleton mock `INexusService`.
- **Industrial-SCADA identity** — Space Grotesk + IBM Plex Mono, KPI cards with sparkline area-fills, status pills, and a connection-dot pulse in the shell.
- **Theme-aware Dark / Light** — full Light dictionary; Dark is the default.
- **Region-based navigation** with lightweight-styled `TabBar` chrome.

## Stack & platforms
**MVUX** + region nav (Uno.Extensions) · Uno.Sdk 6.5.36 · `net10.0-desktop` ✅ (desktop-only by construction)

## Run it
```powershell
dotnet run --project Nexus/Nexus.csproj -f net10.0-desktop
```
