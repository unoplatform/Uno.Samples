# Supabase backend (optional)

This is a **sample app**. It ships running on built-in mock data (`MockSupabaseService`)
and **never connects to anyone's backend by default**. Wiring a real Supabase project is
opt-in and only happens when you provide your own credentials.

## How service selection works

At startup (`App.xaml.cs`) the app reads `Supabase:Url` and `Supabase:AnonKey` from config:

- **Both present** → `SupabaseService` (real backend)
- **Empty / missing** → `MockSupabaseService` (mock data)

`appsettings.json` (committed + embedded in the app) ships with **empty** values, so the
distributed build always runs on mock.

## Use your own Supabase project (local dev)

1. Create a project at [supabase.com](https://supabase.com).
2. Create the local, **gitignored** dev config and add YOUR project's URL + publishable key:

   `Pens/appsettings.development.json`
   ```json
   {
     "AppConfig": { "Environment": "Development" },
     "Supabase": {
       "Url": "https://YOUR-PROJECT.supabase.co",
       "AnonKey": "sb_publishable_YOUR_KEY"
     }
   }
   ```
3. Run a **DEBUG** build. It loads `appsettings.development.json` (Development environment)
   and uses the real backend. Release builds use the empty `appsettings.json` → mock.

`appsettings.development.json` is in `.gitignore` and is **excluded from Release builds**
(see `Pens.csproj`), so your credentials are never committed and never shipped.

## Why this is safe for distribution

- The `sb_publishable_*` key is **designed to be public** — it's meant to be embedded in
  client apps. It is *not* a secret like the `sb_secret_`/service-role key. You cannot hide
  it in any client app, so hiding it is not the security model.
- **Row-Level Security (RLS) is the actual security boundary.** Turn it on (below).
- The distributed sample ships with empty config, so strangers run on mock and never reach
  your project, your data, or your quota.
- If a publishable key is ever exposed (e.g. an old committed value), **rotate it** in the
  Supabase dashboard → Settings → API. RLS limits the blast radius regardless.

## One-shot setup (new project)

Run **`supabase-setup.sql`** in the Supabase SQL editor — it creates the schema, the RLS
policies below, and fictional seed data in a single paste. Use this when standing up a fresh
project.

## Schema

Tables used: `players`, `attendance`, `chat_messages`, `beer_tracker`, `games`, `duties`
(column shapes are in `Models/SupabaseModels.cs`).

## Enable RLS (run in Supabase SQL editor)

This app uses a local picked-player identity (no Supabase Auth), so all requests arrive as
the `anon` role. The policies below enable RLS and grant `anon` exactly the operations the
app performs (least privilege). For a public multi-tenant production app, add Supabase Auth
and scope writes per authenticated user instead.

```sql
-- Turn RLS on for every table (denies all access until a policy allows it)
alter table players       enable row level security;
alter table games         enable row level security;
alter table attendance    enable row level security;
alter table chat_messages enable row level security;
alter table beer_tracker  enable row level security;
alter table duties        enable row level security;

-- Read-only reference data
create policy "read players" on players       for select to anon using (true);
create policy "read games"   on games         for select to anon using (true);

-- Attendance: read + upsert (insert/update)
create policy "read attendance"   on attendance for select to anon using (true);
create policy "insert attendance" on attendance for insert to anon with check (true);
create policy "update attendance" on attendance for update to anon using (true) with check (true);

-- Chat: read + post
create policy "read chat"   on chat_messages for select to anon using (true);
create policy "insert chat" on chat_messages for insert to anon with check (true);

-- Beer tracker: read + update count
create policy "read beer"   on beer_tracker for select to anon using (true);
create policy "update beer" on beer_tracker for update to anon using (true) with check (true);

-- Duties: read + reassign (insert/delete)
create policy "read duties"   on duties for select to anon using (true);
create policy "insert duties" on duties for insert to anon with check (true);
create policy "delete duties" on duties for delete to anon using (true);
```

> These policies let anyone with the app read and write team data (it's a beer-league team
> tool, not a secured multi-tenant SaaS). They do **not** grant schema changes or access to
> other tables. Lock writes down further with Supabase Auth if you ever make it public.
