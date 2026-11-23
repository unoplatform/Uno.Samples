# Supabase backend

Edge functions and migrations for QuoteCraft's hosted mode. Restored from the private `Workflow` snapshot (2026-09-14) after `Uno-Builds` was deleted; this repo had the client (`QuoteCraft/Data/Remote`) but not the backend.

- `migrations/`: tables (`001`) and row-level security (`002`).
- `functions/create-checkout-session`, `functions/stripe-webhook`: Stripe subscription checkout and webhook.
- `functions/quote-view`: public quote view. The app links to it no more: it shows quotes stored in Supabase, and
  cloud sync is off for now (docs/DECISIONS-production-readiness.md, D8).

Secrets are read from the function environment, never from source. Set these in Supabase:

- `STRIPE_BUSINESS_ANNUAL_PRICE_ID`
- `STRIPE_BUSINESS_MONTHLY_PRICE_ID`
- `STRIPE_PRO_ANNUAL_PRICE_ID`
- `STRIPE_PRO_MONTHLY_PRICE_ID`
- `STRIPE_SECRET_KEY`
- `STRIPE_WEBHOOK_SECRET`
- `SUPABASE_SERVICE_ROLE_KEY`
- `SUPABASE_URL`

The app itself only needs the project URL and the **anon** key, in `QuoteCraft/appsettings.json` (section `Supabase`) or
`QUOTECRAFT_SUPABASE_URL` / `QUOTECRAFT_SUPABASE_ANON_KEY`. Without them it runs local-only. The service-role key stays
here, in the function environment; it must never reach the app.
