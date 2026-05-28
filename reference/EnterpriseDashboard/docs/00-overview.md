# EnterpriseDashboard (Observatory) — Design Brief

## 00 — Overview

### What this is

EnterpriseDashboard (codename: Observatory) is a desktop-first SaaS analytics dashboard. A single operator (founder, head of growth, RevOps lead) opens it to read the state of the business in under a minute: revenue trajectory, engagement health, cohort movement. It is not a configuration surface. It is not a multi-user collaborative tool. It is a reading instrument.

### Audience

Primary: solo operators and small teams who already know what they are looking for. They do not need explanations of MRR, retention, or cohort analysis. They need the numbers presented with discipline.

Secondary: stakeholders viewing on demand (an investor, a leadership team in standup). They need legibility at a glance.

### Product personality

Calibrated. Cool. Instrument-like. The dashboard reads like a JPL telemetry display or a Greenwich Observatory readout, not a marketing dashboard. It is the opposite of a "delightful" SaaS UI. Delight here comes from precision, restraint, and the feeling that every pixel was placed for a reason.

Adjectives that fit: precise, cool, instrumented, technical, considered, quiet.
Adjectives that do not fit: friendly, playful, warm, lush, dramatic, ornamental.

### Design philosophy

Three commitments shape every decision in this brief.

**1. The data is the design.**
Numbers are the heroes. Chrome, ornament, and decoration recede. A card title is small. A metric value is large. A delta is mono. The hierarchy is built so that scanning the dashboard is reading the business.

**2. Type carries the load.**
Two typefaces do all the work. *Bricolage Grotesque* (sans, optical sizing) for display and UI. *JetBrains Mono* for data, metadata, and technical decoration. No serif. No italic. The interest comes from scale contrast and weight contrast within each family, not from face-switching.

**3. One accent, used with discipline.**
A single cool blue accent (`#7DB8FF` dark / `#2F66C9` light) marks the active state of toggleable UI, the current data point in a series, the median line in a strip plot, and the live pulse on a signal feed. It never decorates. If a use of the accent does not signify "this is the current/active/live thing," it is wrong.

### What success looks like

- An operator can identify the three most important numbers on each section in under five seconds without scrolling.
- The same dashboard, in both dark and light themes, reads as one product — same hierarchy, same restraint, same instrument feel.
- A new component (a new chart type, a new card variant) can be added without designing a new component shell.
- The dashboard, screenshot side-by-side with the rest of the operator's tools (Linear, Notion, Stripe), reads as distinctly more precise — not louder, more precise.

### What is out of scope (for this brief)

- Authentication, account management, billing surfaces
- Data source connections (Stripe, Mixpanel, etc.) — those are referenced in card footers as fixtures
- Admin / settings surfaces beyond the implied settings entry in the bottom-left rail
- Multi-tenant or team-shared views
- Notifications outside the in-app Signals panel
- Export, share, and embed flows

### Document map

| File | Covers |
|---|---|
| `00-overview.md` | This document. Product purpose, philosophy, success criteria. |
| `01-product-experience.md` | Art direction, tone of voice, micro-copy patterns. |
| `02-information-architecture.md` | Section model, navigation, sidebar context, hierarchy. |
| `03-pages-and-views.md` | The three views (Acquisition, Engagement, Cohorts) in full. |
| `04-components.md` | Every component, every variant, every state. |
| `05-interactions-and-motion.md` | Hover, focus, transition, animation, page choreography. |
| `06-theme-and-visual-system.md` | Color, type, spacing, borders, elevation tokens. |
| `07-responsive-behavior.md` | Desktop, tablet, mobile adaptation. |
| `08-accessibility.md` | Contrast, keyboard, screen reader, motion, semantics. |
| `09-implementation-notes.md` | Uno Platform–specific guidance, MVUX patterns, naming. |

### How to read this brief

Each file stands alone. There is no required reading order. The most common entry points:

- **Designer / art directing a related surface** → `01`, `06`, `04`.
- **Frontend / Uno developer implementing** → `09`, `04`, `06`.
- **Researcher / IA review** → `02`, `03`, `08`.
- **PM / stakeholder approval** → `00`, `01`, `03`.

### A note on the name

The project ships as `EnterpriseDashboard`; "Observatory" is the design codename and the in-app wordmark. It is not decorative. It is the design direction. Every aesthetic choice in this brief asks the question: *does this feel like a reading instrument?* If a proposed change does not, the change is wrong, even if it would look good in isolation.
