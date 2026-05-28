# 🏒 Pens — *Dorval Youngtimers Penguins*

> A beer-league hockey team app — schedule, chat, beer leaderboard, locker-room duties, roster.

<img src="../screenshots/TeamManagement/TeamManagement/Screenshot%202026-05-27%20134653.png" alt="Pens schedule screen" width="360" />

## What you get
A real-world-shaped app showing a **config-driven real-or-mock backend** and a clean **responsive shell** swap between desktop and mobile.

## Highlights
- **Real-or-mock backend, config-driven** — `ISupabaseService` has real + mock implementations; the app reads `Supabase:Url` / `AnonKey` at startup and falls back to fixture data. **The distributed build ships empty, so it always runs on mock data and never touches anyone's backend.**
- **Responsive shell** — desktop side rail ↔ mobile bottom-bar swap via `utu:Responsive`; `SafeArea.Insets` on the header and tab strip handle notches.
- **Identity typography** — Bebas Neue titles + Barlow body, hand-picked spacing and case.
- **Region-based navigation** — login gate → tab shell → 5 section routes, no code-behind `Frame.Navigate`.
- **4 localized locales** (`en`, `es`, `fr`, `pt-BR`) via `x:Uid`.

## Stack & platforms
**MVVM** (CommunityToolkit.Mvvm) · Configuration + HttpKiota · Uno.Sdk 6.5.36 · `net10.0-desktop` ✅, `net10.0-android` (declared)

## Run it
```powershell
dotnet run --project Pens/Pens.csproj -f net10.0-desktop
```
Ships on mock data. To wire your own Supabase project, follow `SUPABASE.md`.
