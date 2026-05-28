# Idea #3: Quote/Estimate Builder for Contractors

**Rank:** #2 (21/25 points) - TOP 3
**Mapped Signals:** S4 (Vertical workflows remain manual), S7 (Hyper-niche wins)
**One-liner:** Contractors quote daily; Jobber is $50-200/mo and overkill for a one-person plumber.

---

## Target User + Painful Problem

Solo contractors and small trades businesses (plumbers, electricians, painters, handymen). They create 5-20 quotes per week. Currently they use Word templates, handwritten estimates, or overpriced all-in-one tools (Jobber at $69/mo, ServiceTitan at $200+/mo) when all they need is: pick services from a list, adjust quantities, add notes, generate a professional PDF, send it, track if accepted.

---

## Differentiation (Why It Beats Alternatives)

- **Does ONE thing well:** quotes. Not a full field service management suite.
- **$15/mo vs $69-200/mo** for Jobber/ServiceTitan
- **Works offline on mobile** (quote on-site, no signal needed)
- **Pre-built pricing catalogs** per trade (plumbing, electrical, painting, etc.)
- **Client sees a branded, professional quote** - not a scribbled napkin

---

## Monetization Model + Pricing

| Tier | Price | Features |
|------|-------|----------|
| Free | $0 | 5 quotes/month, basic template |
| Pro | $15/mo | Unlimited quotes, custom branding, client tracking, acceptance notifications |
| Business | $29/mo | Multiple users, quote analytics, payment integration |

**Revenue target:** 200 users at $15/mo = $3,000 MRR

---

## MVP Scope (2-4 Weeks / 20-40 Hours)

### Week 1 (10 hrs)
- Quote builder UI (line items, quantities, pricing, notes, totals)
- Local SQLite storage
- Basic quote CRUD operations

### Week 2 (10 hrs)
- PDF generation + email/share (QuestPDF)
- 3 pre-built trade catalogs (plumbing, electrical, general contracting)
- Quote status tracking (draft/sent/accepted/declined)

### Week 3 (10 hrs)
- Client tracking dashboard (sent/viewed/accepted/declined)
- Basic analytics (total quoted, conversion rate)
- Branding customization (logo, colors, business info)

### Week 4 (10 hrs)
- Auth + cloud sync (Supabase)
- Stripe Checkout for subscriptions
- WebAssembly deployment + mobile testing

---

## Go-to-Market Plan (First 50 Users)

1. Post in r/Construction, r/Plumbing, r/Electricians, trade Facebook groups
2. Nextdoor - find local contractors and DM them
3. Visit 5 local hardware stores - leave business cards at the counter
4. Run a $100 Facebook ad targeting "small business owner" + "contractor" in your metro area
5. Offer referral program: give a month free for each signup they bring

---

## Risks + How to Test/Kill It Fast

| Risk | Test | Kill Criteria |
|------|------|---------------|
| Contractors are tech-averse | Show 10 contractors a Figma mockup on your phone | If <5 say "I'd try that," pivot |
| Free tools (Canva templates, Excel) are good enough | Ask about follow-up: do they know which quotes were accepted? | If tracking is the pain, you have a wedge |
| Incumbents add a cheap tier | Ship fast, build loyalty, stay 5x cheaper | Monitor competitor pricing quarterly |

---

## Uno Platform Fit

- **Offline-first** for on-site quoting
- **PDF generation** for professional estimates
- **Cross-platform:** quote on mobile, manage on desktop
- **Simple form UI** is fast to build with Uno Platform

---

## Budget Estimate

| Item | Cost |
|------|------|
| Domain | $12 |
| Supabase (free tier) | $0 |
| Hosting (free tier) | $0 |
| QuestPDF (free) | $0 |
| Facebook Ads | $100 |
| Carrd landing page | $19/yr |
| Hardware store flyers (print) | $20 |
| **Total** | **~$151** |
