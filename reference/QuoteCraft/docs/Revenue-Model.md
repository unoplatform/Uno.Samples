# QuoteCraft - Revenue Model & Go-to-Market

## Product Positioning

**QuoteCraft** is a focused quote/estimate builder for solo contractors and small trades businesses. It does one thing well: create, send, and track professional quotes. Not a full field-service platform.

**Positioning statement:** The $15/mo alternative to $69-200/mo tools like Jobber and ServiceTitan, for contractors who just need to quote fast and look professional.

---

## Pricing Tiers

### Free - $0/month
The acquisition engine. Gets contractors in the door, lets them experience the core value.

| Feature | Limit |
|---------|-------|
| Quotes per month | 5 |
| Templates | 1 (generic) |
| PDF generation | Basic (QuoteCraft watermark) |
| Client contacts | 10 |
| Quote statuses | Draft, Sent |
| Platforms | Web + Mobile |
| Storage | Local only (no cloud sync) |

**Purpose:** Low friction trial. A contractor can create their first quote in under 5 minutes, see the PDF, and understand the value before paying.

---

### Pro - $15/month ($144/year if annual = 20% discount)
The core paid tier. Covers 90% of solo contractor needs.

| Feature | Included |
|---------|----------|
| Quotes per month | Unlimited |
| Templates | All trade catalogs (plumbing, electrical, painting, HVAC, general) |
| PDF generation | Branded (your logo, colors, no watermark) |
| Client contacts | Unlimited |
| Quote statuses | Draft, Sent, Viewed, Accepted, Declined, Expired |
| Client notifications | Email when quote is sent; push when viewed/accepted |
| Cloud sync | Supabase sync across devices |
| Offline mode | Full offline quoting, syncs when back online |
| Quote duplication | Clone and modify previous quotes |
| Basic analytics | Total quoted this month, acceptance rate |

**Target conversion:** 15-20% of free users convert to Pro within 30 days.

---

### Business - $29/month ($276/year if annual)
For contractors with a small crew or high volume.

| Feature | Included |
|---------|----------|
| Everything in Pro | Yes |
| Team members | Up to 5 users |
| Quote approval workflow | Owner approves before send |
| Advanced analytics | Revenue pipeline, avg quote size, win rate by trade, monthly trends |
| Payment collection | Stripe link on accepted quotes (client pays deposit online) |
| Quote expiry automation | Auto-expire after configurable days, send reminder before expiry |
| CSV/Excel export | Export quote data for accounting |
| Priority support | Email response within 24 hours |

**Target conversion:** 5-10% of Pro users upgrade within 90 days.

---

## Revenue Projections

### Conservative (6-month target)

| Month | Free Users | Pro ($15) | Business ($29) | MRR |
|-------|-----------|-----------|----------------|-----|
| 1 | 50 | 5 | 0 | $75 |
| 2 | 120 | 15 | 1 | $254 |
| 3 | 200 | 35 | 3 | $612 |
| 4 | 300 | 55 | 6 | $999 |
| 5 | 400 | 80 | 10 | $1,490 |
| 6 | 500 | 110 | 15 | $2,085 |

**Break-even:** ~$150/mo in costs (domain, Supabase paid tier at scale, Stripe fees). Break-even at month 2.

---

## Payment Integration Plan

### Stripe Checkout (not embedded billing)
Simplest integration. No PCI compliance burden.

**Flow:**
1. User taps "Upgrade to Pro" in-app
2. App opens Stripe Checkout session (web redirect)
3. Stripe handles card entry, billing, receipts
4. Webhook confirms payment, app unlocks Pro features
5. Stripe Customer Portal for managing subscription (cancel, update card)

**Implementation:**
- Stripe Checkout Sessions API (server-side via Supabase Edge Function)
- Stripe Webhooks to Supabase Edge Function to update user tier in database
- Client polls or listens for tier change after redirect back
- No card data touches our code

**Stripe fees:** 2.9% + $0.30 per transaction. At $15/mo that's $0.74/transaction = $14.26 net.

### Annual Billing
- Offer 20% discount for annual commitment
- Pro: $144/year (saves $36)
- Business: $276/year (saves $72)
- Reduces churn and improves cash flow

---

## Feature Gating Strategy

Feature gates are enforced locally and validated server-side.

```
Free:
  max_quotes_per_month: 5
  max_clients: 10
  pdf_watermark: true
  cloud_sync: false
  templates: ["general"]
  statuses: ["draft", "sent"]

Pro:
  max_quotes_per_month: unlimited
  max_clients: unlimited
  pdf_watermark: false
  cloud_sync: true
  templates: ["all"]
  statuses: ["all"]
  notifications: true
  analytics: "basic"

Business:
  everything_in_pro: true
  team_members: 5
  approval_workflow: true
  analytics: "advanced"
  payment_collection: true
  csv_export: true
```

**Upgrade prompts appear contextually:**
- Hit 5-quote limit -> "Upgrade to create unlimited quotes"
- Try to add logo -> "Brand your quotes with Pro"
- Create 6th client -> "Upgrade for unlimited clients"

---

## Go-to-Market Funnel

### Phase 1: Seed (Weeks 1-4, $0 spend)

**Goal:** 50 free signups, 5 paying users, validate demand.

| Channel | Action | Target |
|---------|--------|--------|
| Reddit | Post in r/Construction, r/Plumbing, r/Electricians, r/HVAC, r/smallbusiness. Share the problem, link to free tool. | 20 signups |
| Facebook Groups | Join 5-10 local contractor groups. Offer free tool, ask for feedback. | 15 signups |
| Nextdoor | Post as neighbor offering a free tool for local contractors. | 10 signups |
| Direct outreach | DM 20 contractors on Instagram/Facebook who post job photos. | 5 signups |

### Phase 2: Validate (Weeks 5-8, $100 spend)

**Goal:** 200 free signups, 30 paying users.

| Channel | Action | Budget |
|---------|--------|--------|
| Facebook/Instagram Ads | Target "small business owner" + "contractor/plumber/electrician" in 3 metro areas. Ad: "Stop sending ugly quotes. Try QuoteCraft free." | $80 |
| Hardware store flyers | Print 100 half-page flyers with QR code. Leave at 5 stores. | $20 |
| Referral program | "Give a month free, get a month free" for every contractor referred. | $0 (deferred revenue) |

### Phase 3: Scale (Months 3-6, $200/mo spend)

| Channel | Action | Budget |
|---------|--------|--------|
| Google Ads | Target "contractor quote template," "plumbing estimate software," "free quote builder." | $150/mo |
| Content/SEO | Blog posts: "How to write a plumbing estimate," "Contractor quote template PDF." Rank for long-tail trade keywords. | $0 (time) |
| Trade show presence | Attend 1 local trade show, demo on tablet. | $50 one-time |
| Partnerships | Reach out to trade schools, apprenticeship programs. Offer free tier to students. | $0 |

### Conversion Funnel Metrics

```
Visit landing page              100%
Sign up (free)                   25%    <- optimize landing page
Create first quote               60%    <- optimize onboarding
Complete + send first quote      40%    <- optimize quote builder UX
Hit free tier limit (month 1)    30%    <- natural gate
Upgrade to Pro                   15%    <- optimize upgrade prompts
Retain at month 3                80%    <- optimize value delivery
```

---

## Churn Mitigation

| Risk | Mitigation |
|------|------------|
| "I only quote seasonally" | Subscription pausing: up to 3 months, data fully retained, resume anytime. Managed via Stripe Customer Portal. |
| "I found a free alternative" | Free tier exists; remind them of cloud sync, branding, tracking they lose |
| "Too expensive" | Annual discount; remind them 1 extra accepted quote/month pays for it |
| "I don't use it enough" | Monthly email: "You quoted $X this month, Y quotes accepted" - show value |

---

## Key Metrics to Track

| Metric | Tool | Target |
|--------|------|--------|
| MRR / ARR | Stripe Dashboard | $2,000 MRR by month 6 |
| Free-to-paid conversion | Supabase + analytics | 15% within 30 days |
| Quotes created per user/week | In-app analytics | 3+ (indicates habit) |
| Quote acceptance rate | In-app tracking | Show users their rate to drive engagement |
| Monthly churn (Pro) | Stripe | < 5% |
| CAC (paid channels) | Ad platform + Stripe | < $30 (2-month payback at $15/mo) |

---

## Currency Support

MVP supports **USD and CAD**. Currency is set per-business in Settings.

- All monetary values stored as decimals with a currency code
- PDF and quote display use locale-appropriate formatting ($1,250.00 for USD, $1,250.00 CAD)
- Catalog default prices are in USD; CAD users see a note to adjust for their market
- No live exchange rates needed - each contractor works in a single currency
- Additional currencies (GBP, AUD, EUR) can be added post-launch by extending the currency enum

---

## Subscription Pausing

Available to Pro and Business subscribers via Stripe Customer Portal.

| Rule | Detail |
|------|--------|
| Max pause duration | 3 months |
| During pause | Account reverts to Free tier limits. Data fully retained. Cloud sync paused but data preserved. |
| Resume | Billing resumes immediately. Full tier access restored. |
| Auto-resume | Subscription automatically resumes after 3 months if not manually resumed. |
| Implementation | Stripe's built-in subscription pause feature. No custom logic needed. |
