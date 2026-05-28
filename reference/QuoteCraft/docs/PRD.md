# QuoteCraft - Product Requirements Document (PRD)

## 1. Problem Statement

Solo contractors and small trades businesses (plumbers, electricians, painters, handymen, HVAC techs) create 5-20 quotes per week. Their current options are:

1. **Pen and paper / text messages** - Unprofessional, no tracking, lost quotes
2. **Word/Excel templates** - Slow, no client tracking, manual follow-up
3. **Canva / Google Docs** - Not purpose-built, no status tracking, no catalogs
4. **Jobber ($69/mo) / ServiceTitan ($200+/mo)** - Overkill. Built for teams of 10+, loaded with scheduling, dispatch, CRM features a solo contractor never uses

There is a gap in the market for a tool that does one thing well: **create professional quotes fast, send them, and track what happens next.** At a price point ($15/mo) that a solo plumber will actually pay.

---

## 2. User Personas

### Pete the Plumber (Primary)

| Attribute | Detail |
|-----------|--------|
| **Role** | Solo plumber, 12 years experience |
| **Business** | 1-person operation, occasional helper |
| **Volume** | 8-12 quotes per week |
| **Current tool** | Word template on his laptop + texting photos of handwritten estimates |
| **Pain** | Loses track of which quotes are pending. Clients say his estimates look "amateur." Spends 20 min per quote at the kitchen table. |
| **Device** | Samsung Galaxy phone (primary), old Windows laptop (occasional) |
| **Tech comfort** | Uses Facebook, texts, QuickBooks for invoicing. Not a power user. |
| **Quote** | "I'm a plumber, not a graphic designer. I just want to send something that doesn't look like crap." |

### Chris the Crew Lead (Secondary)

| Attribute | Detail |
|-----------|--------|
| **Role** | General contractor, 3-person crew |
| **Business** | LLC, does kitchen/bath remodels, decks, fences |
| **Volume** | 15-20 quotes per week across crew |
| **Current tool** | Tried Jobber, cancelled after 2 months ("too much stuff") |
| **Pain** | Needs his two guys to create quotes on-site that he approves before sending. Wants to see pipeline: how much is out, what's been accepted. |
| **Device** | iPhone (primary), iPad (job sites), MacBook (office) |
| **Tech comfort** | Moderate. Uses accounting software, some project management. |
| **Quote** | "Jobber wanted me to change how I run my whole business. I just need quotes." |

### The Client (End Recipient)

| Attribute | Detail |
|-----------|--------|
| **Role** | Homeowner or property manager receiving a quote |
| **Need** | Clear breakdown of what they're paying for. Easy way to say yes or no. |
| **Context** | Receives quote via email or text link. Views on phone or laptop. |
| **Expectation** | Professional PDF that looks trustworthy. Accept/Decline buttons. |

---

## 3. Success Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| **Time to first quote** | < 5 minutes from signup | In-app tracking |
| **Time to create a quote** | < 3 minutes (returning user) | In-app tracking |
| **Free-to-paid conversion** | 15% within 30 days | Stripe + Supabase |
| **Weekly active users (WAU)** | 60% of paid users | In-app analytics |
| **Quote acceptance rate** | Track and display to users | In-app analytics |
| **Monthly churn (Pro)** | < 5% | Stripe |
| **MRR at 6 months** | $2,000 | Stripe |
| **App Store rating** | 4.5+ stars | Store reviews |
| **NPS** | > 40 | In-app survey (month 2) |

---

## 4. Feature Requirements

### Priority Definitions

- **P0 (Must Have):** Required for MVP launch. Without this, the app has no value.
- **P1 (Should Have):** Important for paid conversion. Ship within 2 weeks of launch.
- **P2 (Nice to Have):** Differentiators. Ship within first 2 months.
- **P3 (Future):** Backlog. Re-evaluate after 100 paying users.

---

### P0 - MVP (Weeks 1-2)

#### F1: Quote Builder
Create, edit, and manage quotes with line items.

| Requirement | Detail |
|-------------|--------|
| F1.1 | Create a new quote with title, client, and line items |
| F1.2 | Add line items manually (description, unit price, quantity) |
| F1.3 | Edit and delete line items (swipe to delete on mobile) |
| F1.4 | Automatic line total calculation (price x quantity) |
| F1.5 | Automatic subtotal, tax, and grand total |
| F1.6 | Configurable tax rate (default from Settings, override per quote) |
| F1.7 | Add free-text notes to a quote |
| F1.8 | Set quote validity date (default: 14 days from creation) |
| F1.9 | Auto-generated quote number (prefix + sequential, e.g., QC-2026-0001) |
| F1.10 | Auto-save every change to local SQLite |
| F1.11 | Reorder line items via drag handle |

#### F2: Quote List / Dashboard
View and filter all quotes.

| Requirement | Detail |
|-------------|--------|
| F2.1 | List all quotes sorted by most recent |
| F2.2 | Display: client name, title, total amount, date, status badge |
| F2.3 | Filter by status (All, Draft, Sent, Accepted, Declined) via ChipGroup |
| F2.4 | Tap a quote to open it in the builder |
| F2.5 | Empty state with call-to-action when no quotes exist |
| F2.6 | Pull-to-refresh (triggers sync when online) |

#### F3: Quote Status Management
Track where each quote is in its lifecycle.

| Requirement | Detail |
|-------------|--------|
| F3.1 | Statuses: Draft, Sent, Viewed, Accepted, Declined, Expired |
| F3.2 | Status changes via explicit user action (mark as sent, etc.) |
| F3.3 | Color-coded status badges (gray/blue/amber/green/red/gray) |
| F3.4 | Status history preserved (when was it sent, viewed, etc.) |

#### F4: Client Management
Basic contact storage for repeat quoting.

| Requirement | Detail |
|-------------|--------|
| F4.1 | Add a client: name (required), email, phone, address (all optional) |
| F4.2 | List all clients with search |
| F4.3 | Select existing client when creating a quote |
| F4.4 | View client detail: contact info + list of their quotes |
| F4.5 | Edit and delete clients |

#### F5: PDF Generation
Create professional quote documents.

| Requirement | Detail |
|-------------|--------|
| F5.1 | Generate PDF from any quote |
| F5.2 | PDF includes: business info, client info, line items table, totals, notes |
| F5.3 | Clean, professional layout (see Design Brief) |
| F5.4 | "Created with QuoteCraft" watermark on Free tier |
| F5.5 | Share PDF via platform share sheet (email, messaging apps, AirDrop, etc.) |
| F5.6 | On WASM: download PDF to browser |

#### F6: Local Storage
Offline-first data persistence.

| Requirement | Detail |
|-------------|--------|
| F6.1 | All data stored in local SQLite database |
| F6.2 | App works fully offline (create, edit, view quotes) |
| F6.3 | No account required for Free tier (local-only) |

#### F7: Business Profile (Settings)
Configure contractor's business information.

| Requirement | Detail |
|-------------|--------|
| F7.1 | Set business name, phone, email, address |
| F7.2 | Set default tax rate |
| F7.3 | Set default quote validity period (days) |
| F7.4 | Set quote number prefix |

---

### P1 - Paid Conversion (Weeks 3-4)

#### F8: Trade Catalogs
Pre-built pricing templates for common services.

| Requirement | Detail |
|-------------|--------|
| F8.1 | Ship with 5 built-in catalogs: Plumbing, Electrical, General Contracting, Painting, HVAC |
| F8.2 | Each catalog has 20-40 common items grouped by category |
| F8.3 | "Add from Catalog" in quote builder opens catalog browser (bottom sheet) |
| F8.4 | Search and filter within catalog |
| F8.5 | Tapping an item adds it to the current quote with default price |
| F8.6 | Users can customize default prices per item |
| F8.7 | Users can add custom items to any catalog |
| F8.8 | Free tier: only "General" catalog. Pro: all catalogs. |
| F8.9 | Default prices sourced from public averages (HomeAdvisor, HomeGuide, Thumbtack) as midpoint of published ranges |
| F8.10 | Catalogs labeled "Suggested prices - adjust to match your rates" |
| F8.11 | Onboarding prompts user to review and adjust catalog prices for their market before first quote |

#### F9: Branding (Pro)
Custom branding on quotes.

| Requirement | Detail |
|-------------|--------|
| F9.1 | Upload business logo (stored locally + Supabase Storage) |
| F9.2 | Logo appears on PDF header |
| F9.3 | Remove "Created with QuoteCraft" watermark |
| F9.4 | Custom footer text on PDF (e.g., license number, terms) |

#### F10: Cloud Sync (Pro)
Sync data across devices via Supabase.

| Requirement | Detail |
|-------------|--------|
| F10.1 | Account creation: email/password or magic link |
| F10.2 | Background sync every 30 seconds when online |
| F10.3 | Manual sync via pull-to-refresh |
| F10.4 | Sync quotes, clients, catalogs, business profile |
| F10.5 | Offline indicator when disconnected |
| F10.6 | Conflict resolution: last-write-wins |

#### F11: Quote Sharing
Send quotes to clients digitally.

| Requirement | Detail |
|-------------|--------|
| F11.1 | Email quote as PDF attachment |
| F11.2 | Share quote link (client views in browser via lightweight server-rendered page) |
| F11.3 | Copy link to clipboard |
| F11.4 | Send via SMS (opens default messaging app with link) |
| F11.5 | Client-facing page hosted via Supabase Edge Function (server-side HTML, no WASM load) |
| F11.6 | Client page includes Accept/Decline buttons that update quote status in database |
| F11.7 | Page load triggers "Viewed" status update automatically |
| F11.8 | PDF download link available on client-facing page |

#### F12: Notifications (Pro)
Know when clients interact with quotes.

| Requirement | Detail |
|-------------|--------|
| F12.1 | Track when a client views a shared quote link |
| F12.2 | In-app notification: "Bob Johnson viewed your quote" |
| F12.3 | Push notification for viewed/accepted/declined (mobile) |
| F12.4 | Email notification as fallback |

#### F13: Subscription & Payment
Stripe-powered subscription management.

| Requirement | Detail |
|-------------|--------|
| F13.1 | Upgrade prompt when hitting Free tier limits |
| F13.2 | "Upgrade to Pro" opens Stripe Checkout |
| F13.3 | Subscription confirmed via webhook, tier updated |
| F13.4 | "Manage Subscription" links to Stripe Customer Portal |
| F13.5 | Feature gating enforced locally and validated server-side |
| F13.6 | Annual billing option with 20% discount |
| F13.7 | Subscription pausing via Stripe Customer Portal (up to 3 months, data retained, reverts to Free tier limits during pause) |

#### F14: Photo Attachments
Attach job site photos to quotes for context and professionalism.

| Requirement | Detail |
|-------------|--------|
| F14.1 | Attach up to 5 photos per quote |
| F14.2 | Capture from camera or select from device gallery |
| F14.3 | Photos auto-compressed to reduce storage (max 1 MB per image) |
| F14.4 | Photos displayed as thumbnail gallery on the quote builder screen |
| F14.5 | Photos included in PDF as an appendix page (grid layout) |
| F14.6 | Photos included on client-facing quote link page |
| F14.7 | Stored locally (SQLite blob or app sandbox file). Synced to Supabase Storage for Pro users. |
| F14.8 | Tap to view full-size. Long press or swipe to delete. |

---

### P2 - Differentiators (Months 2-3)

#### F15: Quote Duplication
Clone existing quotes for similar jobs.

| Requirement | Detail |
|-------------|--------|
| F15.1 | Duplicate a quote (new number, same line items, new client) |
| F15.2 | Duplicate and modify (opens builder with pre-filled data) |

#### F16: Basic Analytics (Pro)
Help contractors understand their business.

| Requirement | Detail |
|-------------|--------|
| F16.1 | Dashboard cards: total quoted this month, quotes sent, acceptance rate |
| F16.2 | Trend: quoted amount over last 6 months (simple bar chart) |
| F16.3 | Win rate percentage |

#### F17: Quote Expiry Automation
Prevent stale quotes from lingering.

| Requirement | Detail |
|-------------|--------|
| F17.1 | Auto-expire quotes past their validity date |
| F17.2 | Reminder notification before expiry (3 days before) |
| F17.3 | Expired quotes can be re-opened and re-sent |

#### F18: Onboarding Flow
Guide new users to first value quickly.

| Requirement | Detail |
|-------------|--------|
| F18.1 | Welcome screen: "Set up your business in 60 seconds" |
| F18.2 | Guided setup: business name, trade, phone, logo (optional) |
| F18.3 | Review and adjust catalog prices for your market |
| F18.4 | "Create your first quote" prompt after setup |
| F18.5 | Skip option for everything |

#### F19: Multi-Currency (USD + CAD)
Support contractors in the US and Canada.

| Requirement | Detail |
|-------------|--------|
| F19.1 | Currency selector in Settings (USD or CAD) |
| F19.2 | All monetary inputs and displays use selected currency symbol and formatting |
| F19.3 | Currency stored per-business, applied to all quotes |
| F19.4 | PDF displays currency code alongside amounts (e.g., "$1,250.00 CAD") |
| F19.5 | Catalog default prices are in USD; CAD users see a note to adjust for their market |

---

### P3 - Future (Post-100 Users)

| Feature | Description |
|---------|-------------|
| F20: Team / Multi-user | Business tier: invite team members, approval workflow |
| F21: Payment Collection | Stripe payment link on accepted quotes (client pays deposit) |
| F22: Advanced Analytics | Revenue pipeline, avg quote size, win rate by trade, monthly trends |
| F23: CSV/Excel Export | Export quote data for accountants |
| F24: Invoice Generation | Convert accepted quote to invoice (bridge to accounting) |
| F25: Recurring Quotes | Templates for maintenance contracts (e.g., annual HVAC service) |
| F26: Integrations | QuickBooks, Xero, Google Contacts import |
| F27: Multi-language | Spanish, French for diverse contractor base |
| F28: Additional Currencies | GBP, AUD, EUR, and other currencies beyond USD/CAD |
| F29: Community Pricing Data | Anonymized aggregate pricing to improve catalog defaults by region |

---

## 5. User Stories (MVP - P0)

### Quote Creation

**US-1: Create a basic quote**
> As a contractor, I want to create a new quote with a title and line items so that I can estimate a job for a client.

Acceptance criteria:
- Tap FAB or "Create Quote" to start a new quote
- Enter a title for the quote
- Add at least one line item with description, price, and quantity
- See the running total update as items are added
- Quote is auto-saved to local storage
- Quote appears in the dashboard list

**US-2: Add line items**
> As a contractor, I want to add individual service/material line items so that the quote accurately reflects the work scope.

Acceptance criteria:
- Tap "Add Line Item" to open the line item editor
- Enter description (required), unit price, and quantity
- Line total calculated automatically (price x quantity)
- Item added to the quote's line item list
- Can add multiple items sequentially

**US-3: Edit a quote**
> As a contractor, I want to edit an existing quote so that I can update pricing or scope before sending.

Acceptance criteria:
- Tap a quote in the dashboard to open it in the builder
- All fields are editable
- Changes are auto-saved
- Line items can be edited, reordered, or deleted

**US-4: Delete a line item**
> As a contractor, I want to remove a line item from a quote so that I can correct mistakes.

Acceptance criteria:
- Swipe left on a line item to reveal delete action
- Confirmation prompt before deletion
- Total recalculates after deletion

### Clients

**US-5: Add a client**
> As a contractor, I want to save client information so that I can quickly assign them to future quotes.

Acceptance criteria:
- Add a client with name (required), email, phone, address
- Client appears in the client list
- Client is selectable when creating a new quote

**US-6: Assign client to quote**
> As a contractor, I want to select an existing client for a quote so that their info auto-populates on the PDF.

Acceptance criteria:
- Client picker shows existing clients with search
- "Add New" option creates a client inline
- Selected client's info appears on the quote and PDF

### PDF & Sharing

**US-7: Generate and share a PDF**
> As a contractor, I want to generate a professional PDF of my quote so that I can send it to the client.

Acceptance criteria:
- "Preview PDF" shows a preview of the generated document
- PDF includes business info, client info, line items, totals, and notes
- "Share" opens the platform share sheet (email, SMS, etc.)
- On WASM, "Download PDF" saves the file to the browser

**US-8: Track quote status**
> As a contractor, I want to mark quotes as sent/accepted/declined so that I know where each job stands.

Acceptance criteria:
- Can change status from the quote detail screen
- Status badge updates on the dashboard
- Status filter on dashboard works correctly

### Settings

**US-9: Set up my business profile**
> As a contractor, I want to enter my business details so that they appear on every quote.

Acceptance criteria:
- Enter business name, phone, email, address
- Set default tax rate
- Set quote validity period
- Info appears on all new PDF quotes

---

## 6. Out of Scope (MVP)

Explicitly excluded from the first release:

- Invoicing / payment collection
- Job scheduling or dispatch
- Employee time tracking
- Inventory management
- CRM / sales pipeline
- Multi-language support
- Dark mode
- Apple Watch / wearable support
- Offline map integration
- AI-powered pricing suggestions
- Integration with accounting software

---

## 7. Technical Constraints

| Constraint | Detail |
|------------|--------|
| **Budget** | $500 maximum total spend |
| **Timeline** | MVP in 4 weeks (40 hours) |
| **Platform** | Uno Platform + .NET 9, Skia renderer |
| **Backend** | Supabase free tier (500 MB database, 1 GB storage, 50K monthly active users) |
| **PDF** | QuestPDF (free for < $1M revenue) |
| **Payments** | Stripe (2.9% + $0.30 per transaction) |
| **Distribution** | WASM hosted on free tier (Netlify/Azure Static Web Apps), APK sideload or Play Store |

---

## 8. Assumptions

1. Solo contractors will pay $15/mo for a tool that saves them 1+ hours/week on quoting
2. A professional-looking PDF is a meaningful upgrade from handwritten or Word-based estimates
3. Offline capability is critical - many job sites have poor connectivity
4. Contractors prefer simple tools that do one thing well over comprehensive platforms
5. Word of mouth in trade communities (Reddit, Facebook groups, Nextdoor) is an effective acquisition channel
6. 5 quotes/month on the free tier is enough to demonstrate value but restrictive enough to drive upgrades
7. QuestPDF will produce acceptable output on all target platforms including WASM

---

## 9. Risks

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| Contractors won't pay for quoting software | Medium | High | Validate with 10 contractor interviews before building. Free tier proves value first. |
| QuestPDF doesn't work well on WASM | Low | High | Test PDF generation on WASM in Week 1. Fallback: server-side PDF generation via Supabase Edge Function. |
| Supabase free tier limits hit early | Low | Medium | Monitor usage. Upgrade to $25/mo Pro tier if needed (still within budget). |
| App Store review delays (iOS) | Medium | Low | Launch WASM first. iOS is P1, not P0. |
| Offline sync conflicts cause data loss | Low | High | Last-write-wins is acceptable for solo users. Add conflict UI for Business tier multi-user. |
| Stripe Checkout UX is jarring on mobile | Medium | Medium | Test early. Consider in-app Stripe Elements if checkout redirect is problematic. |

---

## 10. Resolved Decisions

| # | Question | Decision |
|---|----------|----------|
| 1 | **Catalog pricing** | Source midpoint of published ranges from HomeAdvisor, HomeGuide, and Thumbtack. Label as "Suggested prices." Onboarding prompts user to review and adjust. P3: aggregate anonymized user data for regional improvements. |
| 2 | **Quote link hosting** | Supabase Edge Function renders a lightweight server-side HTML page. No WASM load for the client. Tracks "Viewed" status. Includes Accept/Decline buttons and PDF download. |
| 3 | **Photo attachments** | Moved to P1. Up to 5 photos per quote, camera capture or gallery, auto-compressed, included in PDF appendix and client-facing page. |
| 4 | **Multi-currency** | USD and CAD from P2. Currency set per-business in Settings. Additional currencies (GBP, AUD, EUR) in P3. |
| 5 | **QuestPDF on WASM** | Still needs a Week 1 spike. Fallback: server-side PDF generation via Supabase Edge Function. |
| 6 | **App naming** | Working name "QuoteCraft." Final name and domain TBD before launch. |
| 7 | **Subscription pausing** | Yes, from launch. Up to 3 months via Stripe Customer Portal. Reverts to Free tier limits during pause. Data fully retained. |

## 11. Remaining Open Questions

1. **QuestPDF on WASM** - Needs a technical spike in Week 1 to confirm viability. If it fails, server-side PDF generation via Supabase Edge Function is the fallback.
2. **Push notification service** - Which service for mobile push notifications? Firebase Cloud Messaging? Supabase doesn't have built-in push.
3. **App Store presence** - Launch on Google Play immediately, or start with WASM + sideloaded APK and add Play Store listing after validation?
