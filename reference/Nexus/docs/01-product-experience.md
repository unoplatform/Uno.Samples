# 01 — Product Experience

> User personas, scenarios, workflows, and product goals

---

## Target Users

### Primary Personas

#### 1. Control Room Operator

| Attribute | Value |
|-----------|-------|
| **Role** | Real-time monitoring, first responder to alerts |
| **Skill Level** | Intermediate technical |
| **Environment** | Control room with large displays, 8-12 hour shifts |
| **Primary Tasks** | Monitor dashboards, acknowledge alerts, log events |
| **Pain Points** | Information overload, alert fatigue, split attention |
| **Success Metric** | Time to detect anomalies, false alarm rate |

**Behavioral Notes:**
- Scans dashboards every 30-60 seconds during normal operation
- Needs to identify problems in <3 seconds of visual scan
- Often monitors multiple systems simultaneously
- May hand off mid-shift; needs clear state representation

#### 2. Plant Supervisor

| Attribute | Value |
|-----------|-------|
| **Role** | Shift oversight, resource allocation, escalation decisions |
| **Skill Level** | Business + technical |
| **Environment** | Office adjacent to floor, mobile between locations |
| **Primary Tasks** | Review KPIs, manage shifts, approve decisions |
| **Pain Points** | Lack of trend visibility, difficulty comparing periods |
| **Success Metric** | Shift efficiency, incident response time |

**Behavioral Notes:**
- Checks dashboard at shift start, end, and periodically
- Needs to understand "why" behind metrics, not just "what"
- Frequently compares current vs. historical performance
- Often explains status to management; needs exportable data

#### 3. Maintenance Technician

| Attribute | Value |
|-----------|-------|
| **Role** | Equipment maintenance, repair, preventive care |
| **Skill Level** | Technical specialist |
| **Environment** | Mobile on factory floor, occasional office |
| **Primary Tasks** | Review work orders, check equipment health, log repairs |
| **Pain Points** | Unclear prioritization, missing parts, surprise failures |
| **Success Metric** | Equipment uptime, MTTR, first-time fix rate |

**Behavioral Notes:**
- Uses system to plan daily work route
- Needs equipment history and failure patterns
- Checks spare parts availability before starting jobs
- Documents completed work in system

#### 4. System Administrator

| Attribute | Value |
|-----------|-------|
| **Role** | System configuration, user management, network oversight |
| **Skill Level** | Advanced technical |
| **Environment** | IT office, remote access |
| **Primary Tasks** | Configure thresholds, manage users, monitor network |
| **Pain Points** | Complex configurations, audit requirements, access control |
| **Success Metric** | System uptime, security compliance, user satisfaction |

**Behavioral Notes:**
- Configures system during implementation and maintenance windows
- Responds to access requests and permission changes
- Monitors network connectivity and data flow
- Rarely uses operational dashboards; focused on settings

---

## User Roles & Permissions

| Permission | Admin | Supervisor | Technician | Operator |
|------------|:-----:|:----------:|:----------:|:--------:|
| View Overview | ✓ | ✓ | ✓ | ✓ |
| View Production | ✓ | ✓ | ✓ | ✓ |
| View Analytics | ✓ | ✓ | ✓ | ✓ |
| View Maintenance | ✓ | ✓ | ✓ | ✓ |
| View Settings | ✓ | ✓ | ✗ | ✗ |
| Create Batch | ✓ | ✓ | ✗ | ✗ |
| Create Work Order | ✓ | ✓ | ✓ | ✗ |
| Control Lines | ✓ | ✓ | ✗ | ✗ |
| Edit Thresholds | ✓ | ✗ | ✗ | ✗ |
| Manage Users | ✓ | ✗ | ✗ | ✗ |
| Export Data | ✓ | ✓ | ✓ | ✗ |

---

## Core Use Cases

### Use Case 1: Real-Time Production Monitoring

**Actor:** Control Room Operator  
**Trigger:** Continuous during shift  
**Goal:** Identify production anomalies before they become critical

**Flow:**
1. Operator opens NEXUS (lands on Overview)
2. Scans 4 KPI cards for red/yellow indicators
3. Reviews production lines table for status changes
4. Checks system log for new alerts
5. **If anomaly detected:** Clicks affected line for detail
6. **If critical:** Initiates response procedure

**Success Criteria:**
- Anomaly visible within 5 seconds of occurrence
- Status clearly distinguishable at 3-meter viewing distance
- No more than 2 clicks to reach detail view

**Error Scenarios:**
- Data feed interrupted → Show "DISCONNECTED" status
- Line status unknown → Show "UNKNOWN" with gray indicator

---

### Use Case 2: Shift Handoff

**Actor:** Plant Supervisor (outgoing and incoming)  
**Trigger:** Shift change (typically 3x daily)  
**Goal:** Transfer operational context between shifts

**Flow:**
1. Outgoing supervisor reviews Production page
2. Notes batch queue status and ETAs
3. Reviews shift progress percentage
4. Checks material inventory for low-stock items
5. Reviews system log for unresolved alerts
6. **Verbal handoff** with incoming supervisor
7. Incoming supervisor acknowledges by viewing same screens

**Success Criteria:**
- Complete operational picture visible on 2 screens
- Historical context (last 8 hours) accessible
- Pending items clearly indicated

**Information Required:**
- Current batch in progress
- Queue of upcoming batches
- Lines in non-active status (standby, maintenance)
- Alerts from shift period
- Inventory warnings

---

### Use Case 3: Preventive Maintenance Planning

**Actor:** Maintenance Technician  
**Trigger:** Start of work day or week  
**Goal:** Prioritize maintenance activities

**Flow:**
1. Technician opens Maintenance page
2. Reviews scheduled work orders (sorted by date)
3. Checks equipment health scores
4. Identifies equipment with health < 80%
5. Reviews spare parts inventory
6. Creates work orders for at-risk equipment
7. Plans route based on locations

**Success Criteria:**
- Health scores updated within last 30 minutes
- Clear prioritization of urgent vs. routine
- Parts availability visible before committing to work

**Edge Cases:**
- Equipment health drops suddenly → Promote to urgent
- Part out of stock → Show warning on work order

---

### Use Case 4: Critical Alert Response

**Actor:** Control Room Operator  
**Trigger:** Critical alert appears  
**Goal:** Acknowledge, diagnose, and initiate response

**Flow:**
1. Critical alert appears in system log (red indicator)
2. Browser notification fires (if permitted)
3. Alert sound plays (if enabled)
4. Operator reads alert message
5. Navigates to affected area (Production or Maintenance)
6. Reviews detailed status
7. Initiates appropriate response (line control, work order, escalation)
8. Logs action taken

**Timing Requirements:**
- Alert visible: < 1 second from occurrence
- Sound plays: < 2 seconds from occurrence
- Detail accessible: < 3 seconds (2 clicks max)

**Audio Behavior:**
- Critical alerts: Distinct tone, repeats 3x
- Warnings: Single tone
- Info: No sound

---

### Use Case 5: Weekly Performance Review

**Actor:** Plant Supervisor or Production Manager  
**Trigger:** Weekly management review  
**Goal:** Assess performance and identify improvements

**Flow:**
1. Opens Analytics page
2. Reviews OEE, MTBF, MTTR, FPY metrics
3. Examines weekly output bar chart
4. Compares this week vs. last week in matrix
5. Reviews defect analysis for top issues
6. Identifies improvement opportunities
7. Exports data for presentation

**Success Criteria:**
- Week-over-week comparison visible without navigation
- Trend direction clear (improving/declining)
- Export produces presentation-ready data

---

### Use Case 6: User Onboarding

**Actor:** System Administrator + New User  
**Trigger:** New employee hire  
**Goal:** Grant appropriate system access

**Flow:**
1. Admin opens Settings page
2. Navigates to User Management panel
3. Clicks "+ ADD USER"
4. Enters user details (name, email)
5. Selects role (determines permissions)
6. Saves new user
7. System sends credentials to user email

**Validation:**
- Email must be valid format
- Email must be unique in system
- Role must be selected

**Confirmation:**
- Toast notification: "User created successfully"
- New user appears in list immediately

---

## User Journey Map

### First-Time User (Operator)

```
LOGIN → OVERVIEW → SCAN KPIs → REVIEW LINES → CHECK ALERTS → ACKNOWLEDGE
         ↓                                                      ↓
    Understand       Memorize normal        Establish mental     Ready for
    layout           baseline              model of system       shift work
```

**Onboarding Requirements:**
- No explicit tutorial needed (self-documenting UI)
- First 10 minutes: Explore all 5 tabs
- First day: Understand normal vs. abnormal patterns
- First week: Confident independent operation

### Daily User (Supervisor)

```
SHIFT START                 MID-SHIFT                    SHIFT END
     ↓                          ↓                            ↓
Overview (30s)            Overview (periodic)         Production review
Production check          Alert response              Analytics check
Batch queue review        Issue resolution            Handoff prep
Team assignment           Progress monitoring         Export if needed
```

---

## Product Goals

### Primary Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| Time to Awareness | < 5 seconds | From event to user notice |
| Navigation Efficiency | < 3 clicks | To any detail view |
| Scan Time | < 10 seconds | Full dashboard assessment |
| Error Rate | < 1% | Misinterpretation of status |

### Secondary Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| User Satisfaction | > 4.2/5 | Quarterly survey |
| Training Time | < 2 hours | Time to independent operation |
| System Uptime | 99.9% | Availability during production |

---

## Constraints

### Technical Constraints

- Must work on Chrome, Firefox, Edge (latest 2 versions)
- Must support displays from 1920x1080 to 4K
- Must function with 5-second data polling
- Must handle up to 50 concurrent users

### Environmental Constraints

- Control rooms may have ambient lighting issues
- Operators may view from 1-3 meters distance
- Some users work 12-hour shifts
- Network connectivity may be variable

### Business Constraints

- Single facility deployment (no multi-plant in v1)
- English language only (v1)
- Dark theme only (v1)
- No mobile support required (v1)
