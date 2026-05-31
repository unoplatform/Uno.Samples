# 02 — Information Architecture

> Navigation structure, sitemap, content hierarchy, and data relationships

---

## Navigation Model

### Primary Navigation

NEXUS uses a **flat horizontal tab navigation** with 5 primary sections. This model was chosen because:

- All sections are peer-level in importance
- Users need rapid switching between sections
- Flat structure reduces cognitive load
- Control room context requires minimal navigation overhead

```
┌─────────────────────────────────────────────────────────────────┐
│  [LOGO]  OVERVIEW  PRODUCTION  ANALYTICS  MAINTENANCE  SETTINGS │
└─────────────────────────────────────────────────────────────────┘
```

### Navigation Behavior

| Behavior | Specification |
|----------|---------------|
| Default landing | Overview |
| Active indicator | Underline + text color change |
| Hover state | Text color lighten |
| Transition | Instant (no animation delay) |
| URL routing | `/overview`, `/production`, `/analytics`, `/maintenance`, `/settings` |
| Browser back/forward | Supported via history API |
| Keyboard navigation | Tab to focus, Enter to activate |

### Role-Based Navigation

| Role | Visible Tabs |
|------|--------------|
| Admin | All 5 tabs |
| Supervisor | All 5 tabs |
| Technician | 4 tabs (no Settings) |
| Operator | 4 tabs (no Settings) |

When a user lacks permission for a tab, it should be **hidden entirely** (not disabled).

---

## Site Map

```
NEXUS
├── Overview
│   ├── KPI Metrics (4 cards)
│   ├── Production Lines Table
│   ├── Network Topology
│   └── System Log
│
├── Production
│   ├── Batch Queue Table
│   ├── Shift Information Panel
│   ├── Material Inventory List
│   └── Line Status Detail Grid
│
├── Analytics
│   ├── Performance Metrics (4 cards)
│   ├── Weekly Output Chart
│   ├── Efficiency Trend Chart
│   ├── Defect Analysis List
│   └── Comparison Matrix
│
├── Maintenance
│   ├── Scheduled Work Orders Table
│   ├── Spare Parts Inventory List
│   └── Equipment Health Grid
│
└── Settings
    ├── System Preferences Panel
    ├── Data & Display Panel
    ├── Network Configuration Panel
    ├── Alert Thresholds Panel
    └── User Management Panel
```

---

## Content Hierarchy

### Overview Page

```
OVERVIEW
├── [PRIMARY] KPI Metrics Row
│   ├── Throughput (units/hr)
│   ├── Efficiency (%)
│   ├── Uptime (%)
│   └── Energy (MW)
│
├── [SECONDARY] Content Grid
│   ├── [LEFT] Production Lines Panel (1.5fr)
│   │   └── Table: Unit, Status, Output, Temp, Pressure
│   │
│   └── [RIGHT] Network Topology Panel (1fr)
│       └── SVG: Node graph with connections
│
└── [TERTIARY] System Log Panel (full width)
    └── List: Time, Type Indicator, Message
```

### Production Page

```
PRODUCTION
├── [PRIMARY] 3-Column Grid
│   ├── [COL 1] Batch Queue Panel
│   │   └── Table: Batch ID, Product, Qty, Priority, ETA
│   │
│   ├── [COL 2] Shift Information Panel
│   │   ├── Shift Badge + Time Range
│   │   ├── Details Grid: Supervisor, Operators, Breaks
│   │   └── Progress Bar
│   │
│   └── [COL 3] Material Inventory Panel
│       └── List: Material, Stock Bar, Threshold, Value
│
└── [SECONDARY] Line Status Detail Panel (full width)
    └── Card Grid: 5 line cards with metrics
```

### Analytics Page

```
ANALYTICS
├── [PRIMARY] Performance Metrics Row
│   ├── OEE (%)
│   ├── MTBF (hours)
│   ├── MTTR (hours)
│   └── FPY (%)
│
└── [SECONDARY] 2-Column Grid
    ├── [LEFT] Weekly Output Panel
    │   └── Bar Chart
    │
    ├── [RIGHT] Efficiency Trend Panel
    │   └── Line Chart
    │
    ├── [LEFT] Defect Analysis Panel
    │   └── Ranked Bar List
    │
    └── [RIGHT] Comparison Matrix Panel
        └── Table: Metric, This Week, Last Week, Change
```

### Maintenance Page

```
MAINTENANCE
├── [PRIMARY] 3-Column Grid
│   ├── [COL 1-2] Scheduled Maintenance Panel (2fr)
│   │   └── Table: WO ID, Equipment, Type, Date, Tech, Status
│   │
│   └── [COL 3] Spare Parts Panel (1fr)
│       └── List: Part, Status, Stock, Min
│
└── [SECONDARY] Equipment Health Panel (full width)
    └── Card Grid: 5 equipment cards with health rings
```

### Settings Page

```
SETTINGS
└── 2-Column Grid
    ├── [LEFT] System Preferences Panel
    │   └── Toggle List: Auto Backup, Alert Sounds, Dark Mode, Compact View
    │
    ├── [RIGHT] Data & Display Panel
    │   └── Dropdown List: Retention, Refresh, Temp Unit, Pressure Unit
    │
    ├── [LEFT] Network Configuration Panel
    │   └── Table: Param, Value, Status
    │
    ├── [RIGHT] Alert Thresholds Panel
    │   └── Input List: Param, Value, Unit
    │
    └── [FULL WIDTH] User Management Panel
        └── Table: User, Role, Last Access, Status, Actions
```

---

## Data Entities

### Entity Relationship Diagram

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│  PRODUCTION     │     │     BATCH       │     │    PRODUCT      │
│     LINE        │◄────│                 │────►│                 │
├─────────────────┤     ├─────────────────┤     ├─────────────────┤
│ id              │     │ id              │     │ id              │
│ name            │     │ line_id (FK)    │     │ name            │
│ status          │     │ product_id (FK) │     │ sku             │
│ output_percent  │     │ quantity        │     │ category        │
│ temperature     │     │ priority        │     └─────────────────┘
│ pressure        │     │ eta             │
│ created_at      │     │ status          │
└─────────────────┘     │ created_at      │
                        └─────────────────┘

┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│   EQUIPMENT     │     │   WORK ORDER    │     │      USER       │
│                 │◄────│                 │────►│                 │
├─────────────────┤     ├─────────────────┤     ├─────────────────┤
│ id              │     │ id              │     │ id              │
│ name            │     │ equipment_id(FK)│     │ name            │
│ health_score    │     │ type            │     │ email           │
│ next_service    │     │ scheduled_date  │     │ role            │
│ runtime_hours   │     │ technician_id   │     │ last_access     │
│ location        │     │ status          │     │ status          │
└─────────────────┘     │ notes           │     │ created_at      │
                        └─────────────────┘     └─────────────────┘

┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│   SPARE PART    │     │     ALERT       │     │    SETTING      │
├─────────────────┤     ├─────────────────┤     ├─────────────────┤
│ id              │     │ id              │     │ key             │
│ name            │     │ type            │     │ value           │
│ in_stock        │     │ message         │     │ category        │
│ min_stock       │     │ source          │     │ updated_at      │
│ status          │     │ timestamp       │     │ updated_by      │
│ location        │     │ acknowledged    │     └─────────────────┘
└─────────────────┘     └─────────────────┘
```

### Entity Definitions

#### Production Line

| Field | Type | Description |
|-------|------|-------------|
| `id` | string | Unique identifier (e.g., "A1", "B1") |
| `name` | string | Full name (e.g., "Assembly Line A1") |
| `status` | enum | `active`, `standby`, `maintenance` |
| `output_percent` | number | Current output as percentage (0-100) |
| `temperature` | number | Current temperature in Celsius |
| `pressure` | number | Current pressure in bar |

#### Batch

| Field | Type | Description |
|-------|------|-------------|
| `id` | string | Batch identifier (e.g., "B-4893") |
| `product_name` | string | Product being produced |
| `quantity` | number | Units to produce |
| `priority` | enum | `high`, `normal`, `low` |
| `eta` | string | Estimated completion time (HH:MM) |
| `status` | enum | `queued`, `in_progress`, `completed` |

#### Equipment

| Field | Type | Description |
|-------|------|-------------|
| `id` | string | Unique identifier |
| `name` | string | Equipment name |
| `health_score` | number | Health percentage (0-100) |
| `next_service` | string | Time until next service |
| `runtime_hours` | string | Total runtime hours |

#### Work Order

| Field | Type | Description |
|-------|------|-------------|
| `id` | string | Work order ID (e.g., "WO-2847") |
| `equipment_id` | string | Reference to equipment |
| `type` | string | "Preventive", "Inspection", "Calibration", "Repair" |
| `scheduled_date` | date | Planned execution date |
| `technician_id` | string | Assigned technician |
| `status` | enum | `scheduled`, `pending`, `in_progress`, `completed` |

#### User

| Field | Type | Description |
|-------|------|-------------|
| `id` | string | Unique identifier |
| `name` | string | Full name |
| `email` | string | Email address |
| `role` | enum | `admin`, `supervisor`, `technician`, `operator` |
| `last_access` | datetime | Last login timestamp |
| `status` | enum | `active`, `standby`, `inactive` |

#### Alert

| Field | Type | Description |
|-------|------|-------------|
| `id` | string | Unique identifier |
| `type` | enum | `info`, `warning`, `success`, `critical` |
| `message` | string | Alert message text |
| `timestamp` | datetime | When alert occurred |
| `acknowledged` | boolean | Whether operator acknowledged |

---

## Data Refresh Strategy

### Real-Time Data (5-second polling)

- Production line status
- KPI metrics
- Equipment health scores
- Active batch progress

### Event-Driven Data (WebSocket push)

- New alerts
- Status changes
- Critical events

### On-Demand Data (user action)

- Historical analytics
- Full equipment history
- User management list
- Settings configuration

### Data Freshness Indicators

| Indicator | Meaning |
|-----------|---------|
| Pulse animation on connection dot | Live connection active |
| "LAST SYNC: Xs AGO" in footer | Time since last successful fetch |
| Grayed data | Stale (>30 seconds old) |
| "DISCONNECTED" status | Connection lost |

---

## URL Structure

### Route Definitions

```
/                   → Redirect to /overview
/overview           → Overview page
/production         → Production page
/analytics          → Analytics page
/maintenance        → Maintenance page
/settings           → Settings page
/login              → Authentication (if not authenticated)
```

### Query Parameters

```
/analytics?range=7d       → Filter to last 7 days
/maintenance?status=pending → Filter by status
/settings?panel=users     → Open specific panel
```

### Deep Linking

All pages support direct linking. Browser refresh maintains current view state.

---

## Search & Filter

### Global Search

**Not implemented in v1.** The interface is designed for monitoring, not searching. Future versions may add:

- Equipment search
- Batch search
- Alert search

### Local Filters

| Location | Filter Type | Options |
|----------|-------------|---------|
| Production Lines | Status | All, Active, Standby, Maintenance |
| Work Orders | Status | All, Scheduled, Pending, Completed |
| System Log | Type | All, Info, Warning, Critical |
| Users | Role | All, Admin, Supervisor, Technician, Operator |

### Sort Behavior

| Table | Default Sort | Available Sorts |
|-------|--------------|-----------------|
| Production Lines | Line ID (asc) | Status, Output |
| Batch Queue | Priority (desc) → ETA (asc) | ETA, Quantity |
| Work Orders | Date (asc) | Type, Status, Technician |
| Equipment Health | Health (asc) | Name, Runtime |
| Users | Name (asc) | Role, Last Access |
