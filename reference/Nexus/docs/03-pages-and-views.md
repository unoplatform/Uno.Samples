# 03 — Pages and Views

> Detailed specifications for every page, screen, and major view

---

## Global Layout

### Page Structure

```
┌─────────────────────────────────────────────────────────────────┐
│ HEADER                                                    80px  │
│ [Logo] [Nav Tabs]                    [System Time] [Status]     │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│ MAIN CONTENT AREA                                               │
│                                                       flex: 1   │
│                                                                 │
├─────────────────────────────────────────────────────────────────┤
│ FOOTER                                                    48px  │
│ [Node Count] [Last Sync] [Latency]               [Version]      │
└─────────────────────────────────────────────────────────────────┘
```

### Header Specifications

| Element | Position | Size | Content |
|---------|----------|------|---------|
| Logo Mark | Left, 24px from edge | 36×36px | Diamond icon with corner nodes |
| Logo Text | Adjacent to mark | 18px | "NEXUS" + subtitle |
| Nav Tabs | Center | Auto | 5 tab buttons |
| System Time | Right | Auto | "SYS.TIME" label + HH:MM:SS |
| Connection Status | Far right | Auto | Pulse dot + "CONNECTED" |

### Footer Specifications

| Element | Position | Content |
|---------|----------|---------|
| Node Count | Left | "NODE COUNT: 847" |
| Last Sync | Center-left | "LAST SYNC: 2.4s AGO" |
| Latency | Center-right | "LATENCY: 12ms" |
| Version | Right | "v4.2.1" |

---

## Page: Overview

### Purpose

Provide at-a-glance operational health summary. This is the default landing page and should communicate overall system status within 10 seconds of viewing.

### Layout Grid

```
┌─────────────────────────────────────────────────────────────────┐
│ [Metric] [Metric] [Metric] [Metric]                   4-col     │
├─────────────────────────────────────────────────────────────────┤
│ ┌─────────────────────────────┐ ┌─────────────────────────────┐ │
│ │                             │ │                             │ │
│ │   PRODUCTION LINES          │ │   NETWORK TOPOLOGY          │ │
│ │   Table (5 rows)            │ │   SVG visualization         │ │
│ │                             │ │                             │ │
│ │                        1.5fr│ │                         1fr │ │
│ └─────────────────────────────┘ └─────────────────────────────┘ │
├─────────────────────────────────────────────────────────────────┤
│ ┌─────────────────────────────────────────────────────────────┐ │
│ │                                                             │ │
│ │   SYSTEM LOG                                      span 2    │ │
│ │   Alert list (4 items)                                      │ │
│ │                                                             │ │
│ └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### Section: KPI Metrics

**Grid:** 4 columns, equal width, 16px gap

| Card | Label | Value Format | Unit | Status Logic |
|------|-------|--------------|------|--------------|
| 1 | THROUGHPUT | ###.# | units/hr | Nominal if ≥800, Warning if <600 |
| 2 | EFFICIENCY | ##.# | % | Nominal if ≥90, Warning if <80 |
| 3 | UPTIME | ##.# | % | Nominal if ≥99, Warning if <95 |
| 4 | ENERGY | #.## | MW | Optimal if ≤1.5, Warning if >2.0 |

**Trend Calculation:**
- Compare current value to same hour yesterday
- Display as "+X.X%" or "-X.X%"
- Green for positive trends (except Energy, where negative is better)
- Yellow for negative trends

### Section: Production Lines

**Panel Header:** "PRODUCTION LINES" + "VIEW ALL" button

**Table Columns:**

| Column | Width | Content | Alignment |
|--------|-------|---------|-----------|
| UNIT | 1.5fr | ID badge + line name | Left |
| STATUS | 0.8fr | Status indicator | Left |
| OUTPUT | 0.8fr | Progress bar (0-100%) | Left |
| TEMP °C | 0.8fr | Temperature value | Right |
| PRESSURE | 0.8fr | Pressure value + "bar" | Right |

**Row States:**
- Default: `bg-secondary` background
- Hover: `rgba(255,255,255,0.02)` overlay
- Selected: `border-left: 2px solid text-secondary`

### Section: Network Topology

**Panel Header:** "NETWORK TOPOLOGY" (no action button)

**SVG Specifications:**
- ViewBox: Percentage-based positioning for responsiveness
- Nodes: 6 nodes representing network components
- Connections: Dashed animated lines between nodes
- Animation: 20-second stroke-dashoffset cycle

**Node Layout:**
```
PLC-01 (15%, 25%)
    ├── HMI-A (35%, 15%)
    │      └── SERVER (55%, 25%)
    └── SCADA (35%, 35%)
           ├── SERVER (55%, 25%)
           └── DB-01 (55%, 45%)
                  └── CLOUD (75%, 30%)
```

### Section: System Log

**Panel Header:** "SYSTEM LOG" + "EXPORT" button

**Alert Item Structure:**
```
[TIME]  [●]  [MESSAGE]
14:32:08  ●   Batch #4892 completed successfully
```

**Type Indicators:**
- Info: Blue dot (#60a5fa)
- Warning: Yellow dot (#fbbf24)
- Success: Green dot (#4ade80)
- Critical: Red dot (#f87171) + pulse animation

**Display Limit:** 4 most recent alerts (scrollable to 50)

### Overview States

| State | Behavior |
|-------|----------|
| Loading | Skeleton placeholders for all cards and tables |
| Empty | "No production data available" message |
| Error | "Unable to load data. Retrying..." with spinner |
| Disconnected | Header shows "DISCONNECTED" in red, data grayed |

---

## Page: Production

### Purpose

Manage active production operations including batch scheduling, shift oversight, and resource monitoring.

### Layout Grid

```
┌─────────────────────────────────────────────────────────────────┐
│ ┌───────────────────┐ ┌───────────────────┐ ┌───────────────────┐
│ │                   │ │                   │ │                   │
│ │   BATCH QUEUE     │ │   SHIFT INFO      │ │   MATERIAL        │
│ │                   │ │                   │ │   INVENTORY       │
│ │              1fr  │ │              1fr  │ │              1fr  │
│ └───────────────────┘ └───────────────────┘ └───────────────────┘
├─────────────────────────────────────────────────────────────────┤
│ ┌─────────────────────────────────────────────────────────────┐ │
│ │                                                             │ │
│ │   LINE STATUS DETAIL                               span 3   │ │
│ │   Card grid (5 cards)                                       │ │
│ │                                                             │ │
│ └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### Section: Batch Queue

**Panel Header:** "BATCH QUEUE" + "+ NEW BATCH" button

**Table Columns:**

| Column | Width | Content |
|--------|-------|---------|
| BATCH ID | 1fr | Badge with ID (e.g., "B-4893") |
| PRODUCT | 1.5fr | Product name |
| QTY | 0.8fr | Quantity (formatted with commas) |
| PRIORITY | 0.8fr | Status indicator (high/normal/low) |
| ETA | 0.8fr | Time (HH:MM) |

**Priority Colors:**
- High: Red indicator
- Normal: Green indicator
- Low: Yellow indicator

### Section: Shift Information

**Panel Header:** "SHIFT INFORMATION" (no action button)

**Content Structure:**
```
┌─────────────────────────────────────────┐
│  [SHIFT B]                    06:00—14:00│
├─────────────────────────────────────────┤
│  SUPERVISOR │  OPERATORS  │ BREAKS TAKEN │
│   M. Chen   │     12      │     2/3      │
├─────────────────────────────────────────┤
│  SHIFT PROGRESS                     67%  │
│  ████████████████░░░░░░░░░░░░░░░░░░░░   │
└─────────────────────────────────────────┘
```

**Progress Calculation:**
- Based on current time within shift window
- Updates every minute

### Section: Material Inventory

**Panel Header:** "MATERIAL INVENTORY" + "REORDER" button

**Item Structure:**
```
Material Name                         IN STOCK / LOW STOCK
████████████████░░░░░░│░░░░░░░░░░░░░░  82 tons
                       ▲
                    threshold
```

**Status Logic:**
- "IN STOCK" (green): stock ≥ reorder threshold
- "LOW STOCK" (yellow): stock < reorder threshold

### Section: Line Status Detail

**Panel Header:** "LINE STATUS DETAIL" (no action button)

**Card Grid:** 5 columns, 16px gap

**Card Content:**
```
┌─────────────────────────┐
│  [A1]     ● ACTIVE      │
│                         │
│  Assembly Line A1       │
├─────────────────────────┤
│  OUTPUT        89%      │
│  TEMP       42.3°C      │
│  PRESSURE   2.4 bar     │
├─────────────────────────┤
│  [DETAILS] [CONTROL]    │
└─────────────────────────┘
```

**Card Actions:**
- DETAILS: Opens detail modal (read-only)
- CONTROL: Opens control panel (with permission)

### Production States

| State | Behavior |
|-------|----------|
| No batches | "No batches in queue. Click + NEW BATCH to add." |
| Shift ended | Shift panel shows "Shift ended. Next: [time]" |
| Line offline | Card dimmed, "OFFLINE" badge, no metrics |

---

## Page: Analytics

### Purpose

Provide historical analysis and trend visualization for performance optimization decisions.

### Layout Grid

```
┌─────────────────────────────────────────────────────────────────┐
│ [Metric] [Metric] [Metric] [Metric]                   4-col     │
├─────────────────────────────────────────────────────────────────┤
│ ┌─────────────────────────────┐ ┌─────────────────────────────┐ │
│ │                             │ │                             │ │
│ │   WEEKLY OUTPUT             │ │   EFFICIENCY TREND          │ │
│ │   Bar chart                 │ │   Line chart                │ │
│ │                        1.5fr│ │                         1fr │ │
│ └─────────────────────────────┘ └─────────────────────────────┘ │
├─────────────────────────────────────────────────────────────────┤
│ ┌─────────────────────────────┐ ┌─────────────────────────────┐ │
│ │                             │ │                             │ │
│ │   DEFECT ANALYSIS           │ │   COMPARISON MATRIX         │ │
│ │   Ranked bar list           │ │   Table                     │ │
│ │                         1fr │ │                         1fr │ │
│ └─────────────────────────────┘ └─────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### Section: Performance Metrics

| Card | Label | Value Format | Subtext |
|------|-------|--------------|---------|
| 1 | OEE | ##.#% | "vs last week" |
| 2 | MTBF | ###h | "mean time between failures" |
| 3 | MTTR | #.#h | "mean time to repair" |
| 4 | FPY | ##.#% | "first pass yield" |

### Section: Weekly Output

**Panel Header:** "WEEKLY OUTPUT" + "EXPORT DATA" button

**Chart Specifications:**
- Type: Vertical bar chart
- X-axis: Days (MON, TUE, WED, THU, FRI, SAT, SUN)
- Y-axis: Units produced (auto-scale)
- Target line: Dashed horizontal line at target value
- Node markers: Circle at top of each bar

### Section: Efficiency Trend

**Panel Header:** "EFFICIENCY TREND (12 MONTHS)" (no action)

**Chart Specifications:**
- Type: Line chart with point markers
- X-axis: Months (JAN through DEC)
- Y-axis: Percentage (0-100%)
- Line: 1.5px stroke, muted gray
- Points: Circles with dark fill, light stroke

### Section: Defect Analysis

**Panel Header:** "DEFECT ANALYSIS" (no action)

**Item Structure:**
```
#1  Surface Scratch          23 units
    ████████████████░░░░░░░░░░░░░░░░░░░░
```

**Display:** Top 4 defect types by count

### Section: Comparison Matrix

**Panel Header:** "COMPARISON MATRIX" (no action)

**Table Structure:**

| Metric | THIS WEEK | LAST WEEK | CHANGE |
|--------|-----------|-----------|--------|
| Units Produced | 24,180 | 23,450 | +3.1% |
| Defect Rate | 1.2% | 1.4% | -0.2% ✓ |
| Downtime Hours | 4.2h | 6.8h | -38% ✓ |
| Energy Usage | 8.4MW | 8.9MW | -5.6% ✓ |

**Change Colors:**
- Positive change: Green (except where decrease is good)
- Negative change: Default gray

---

## Page: Maintenance

### Purpose

Plan and track equipment maintenance activities, manage spare parts, and monitor equipment health.

### Layout Grid

```
┌─────────────────────────────────────────────────────────────────┐
│ ┌───────────────────────────────────────┐ ┌───────────────────┐ │
│ │                                       │ │                   │ │
│ │   SCHEDULED MAINTENANCE               │ │   SPARE PARTS     │ │
│ │   Table (4 rows)                 2fr  │ │   List       1fr  │ │
│ │                                       │ │                   │ │
│ └───────────────────────────────────────┘ └───────────────────┘ │
├─────────────────────────────────────────────────────────────────┤
│ ┌─────────────────────────────────────────────────────────────┐ │
│ │                                                             │ │
│ │   EQUIPMENT HEALTH MONITOR                         span 3   │ │
│ │   Card grid (5 cards with rings)                            │ │
│ │                                                             │ │
│ └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### Section: Scheduled Maintenance

**Panel Header:** "SCHEDULED MAINTENANCE" + "+ WORK ORDER" button

**Table Columns:**

| Column | Width | Content |
|--------|-------|---------|
| WORK ORDER | 1fr | WO ID badge |
| EQUIPMENT | 1.5fr | Equipment name |
| TYPE | 1fr | Type badge |
| DATE | 1fr | YYYY-MM-DD |
| TECHNICIAN | 1fr | Technician name |
| STATUS | 0.8fr | Status indicator |

### Section: Spare Parts Inventory

**Panel Header:** "SPARE PARTS INVENTORY" (no action)

**Item Structure:**
```
Part Name                    ● OK / LOW / CRITICAL
IN STOCK: 24    MIN: 10
```

**Status Logic:**
- OK: in_stock > min_stock
- LOW: in_stock ≤ min_stock AND in_stock > 0
- CRITICAL: in_stock < min_stock / 2

### Section: Equipment Health Monitor

**Panel Header:** "EQUIPMENT HEALTH MONITOR" (no action)

**Card Content:**
```
┌─────────────────────────┐
│  CNC Mill #1      ┌──┐  │
│                   │94│  │
│                   │% │  │
│                   └──┘  │
│                   ring  │
├─────────────────────────┤
│  NEXT SERVICE   12 days │
│  RUNTIME        2,847h  │
└─────────────────────────┘
```

**Health Ring Colors:**
- Green: ≥80%
- Yellow: 60-79%
- Red: <60%

---

## Page: Settings

### Purpose

Configure system behavior, manage users, and set operational thresholds.

### Layout Grid

```
┌─────────────────────────────────────────────────────────────────┐
│ ┌─────────────────────────────┐ ┌─────────────────────────────┐ │
│ │   SYSTEM PREFERENCES        │ │   DATA & DISPLAY            │ │
│ │   Toggle list          1fr  │ │   Dropdown list        1fr  │ │
│ └─────────────────────────────┘ └─────────────────────────────┘ │
├─────────────────────────────────────────────────────────────────┤
│ ┌─────────────────────────────┐ ┌─────────────────────────────┐ │
│ │   NETWORK CONFIGURATION     │ │   ALERT THRESHOLDS          │ │
│ │   Status table         1fr  │ │   Input list           1fr  │ │
│ └─────────────────────────────┘ └─────────────────────────────┘ │
├─────────────────────────────────────────────────────────────────┤
│ ┌─────────────────────────────────────────────────────────────┐ │
│ │   USER MANAGEMENT                                  span 2   │ │
│ │   Table with actions                                        │ │
│ └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### Section: System Preferences

| Setting | Control | Default |
|---------|---------|---------|
| AUTO BACKUP | Toggle | ON |
| ALERT SOUNDS | Toggle | ON |
| DARK MODE | Toggle | ON (locked in v1) |
| COMPACT VIEW | Toggle | OFF |

### Section: Data & Display

| Setting | Control | Options | Default |
|---------|---------|---------|---------|
| DATA RETENTION | Dropdown | 30/60/90/180/365 days | 90 days |
| REFRESH RATE | Dropdown | 1/5/10/30/60 sec | 5 sec |
| TEMPERATURE UNIT | Dropdown | Celsius/Fahrenheit | Celsius |
| PRESSURE UNIT | Dropdown | bar/psi/kPa | bar |

### Section: Network Configuration

| Parameter | Value | Status |
|-----------|-------|--------|
| Primary Server | 192.168.1.100 | ● CONNECTED |
| Backup Server | 192.168.1.101 | ● STANDBY |
| SCADA Gateway | 192.168.1.50 | ● CONNECTED |
| PLC Network | 10.0.0.0/24 | ● ACTIVE |

**Read-only display** — Requires system admin access to modify

### Section: Alert Thresholds

| Parameter | Input | Unit |
|-----------|-------|------|
| Temperature Max | 85 | °C |
| Pressure Max | 4.5 | bar |
| Vibration Limit | 2.5 | mm/s |
| Power Surge | 15 | % |

**Validation:**
- Numeric input only
- Range limits per parameter
- Inline error if invalid

### Section: User Management

**Panel Header:** "USER MANAGEMENT" + "+ ADD USER" button

**Table Columns:**

| Column | Width | Content |
|--------|-------|---------|
| USER | 1.5fr | Avatar + name + email |
| ROLE | 1fr | Role badge |
| LAST ACCESS | 1fr | Relative time |
| STATUS | 0.8fr | Status indicator |
| ACTIONS | 1fr | Edit + Remove buttons |

**Role Restrictions:**
- Only Admin can see this section
- Admin cannot remove self
- At least one Admin must exist
