# Penguins Hockey App — Complete UI Specification

## Document Purpose

This specification provides exhaustive detail for pixel-perfect recreation of the Penguins beer league hockey app. It is written for an AI agent to follow sequentially. Each section builds upon the previous. Follow instructions in order.

---

# PART 1: GLOBAL DESIGN SYSTEM

Before building any page, implement these foundational elements.

## 1.1 Color Tokens

Define these CSS custom properties at `:root` level:

```
--neon-amber: #ffaa00
--neon-amber-glow: rgba(255, 170, 0, 0.6)
--powder-blue: #b4d7e8
--powder-blue-deep: #7eb8d4
--powder-blue-glow: rgba(180, 215, 232, 0.4)
--ice-blue: #00d4ff
--ice-blue-glow: rgba(0, 212, 255, 0.5)
--hot-red: #ff3b3b
--arena-dark: #0a0c10
--boards-dark: #12161d
--boards-mid: #1a1f2a
--text-primary: #f0f4f8
--text-muted: #6b7a8f
--success-green: #10b981
--purple-accent: #8b5cf6
```

**Color Usage Rules:**
- `--arena-dark`: Primary app background
- `--boards-dark`: Card backgrounds, input backgrounds
- `--boards-mid`: Elevated surfaces, hover states, secondary cards
- `--neon-amber`: Beer-related elements, warnings, pending states
- `--powder-blue`: Team accent color (jerseys), interactive elements, avatars, send buttons
- `--ice-blue`: Legacy accent (use sparingly for ice-related icons)
- `--hot-red`: "Out" status, destructive actions
- `--success-green`: "In" status, food duty accent
- `--text-primary`: Primary text on dark backgrounds
- `--text-muted`: Secondary text, labels, timestamps

## 1.2 Typography

**Font Families:**
1. **Bebas Neue** (Google Fonts) — Display font
2. **Barlow** (Google Fonts) — Body font, weights: 400, 500, 600, 700

**Type Scale:**

| Element | Font | Size | Weight | Letter Spacing | Color | Additional |
|---------|------|------|--------|----------------|-------|------------|
| App Title | Bebas Neue | 28px | 400 | 2px | Gradient (white to #a8b5c4) | text-transform: uppercase |
| Section Title | Bebas Neue | 14px | 400 | 3px | --text-muted | text-transform: uppercase |
| Large Counter | Bebas Neue | 72px | 400 | 0 | --neon-amber | Has glow effect |
| Medium Counter | Bebas Neue | 36px | 400 | 0 | Varies | — |
| Card Title | Bebas Neue | 36px | 400 | 1px | --text-primary | — |
| Time Display | Bebas Neue | 20px | 400 | 1px | --text-primary | — |
| Badge Text | Bebas Neue | 18px | 400 | 0 | --powder-blue | — |
| Counter Unit | Bebas Neue | 24px | 400 | 1px | --text-muted | — |
| Body Text | Barlow | 14-15px | 400-600 | 0 | --text-primary | line-height: 1.5 |
| Label | Barlow | 11-12px | 400-600 | 1-1.5px | --text-muted | text-transform: uppercase |
| Small Label | Barlow | 10-11px | 600 | 0.5-1.5px | --text-muted | text-transform: uppercase |
| Hint Text | Barlow | 11px | 400 | 0 | --text-muted | — |

## 1.3 Spacing System

Base unit: 4px. Use multiples:

- 4px (xs)
- 8px (sm)
- 10px
- 12px (md)
- 14px
- 16px (lg)
- 20px (xl)
- 24px (2xl)
- 30px
- 40px (4xl)

## 1.4 Border Radius Scale

- 4px: Small blocks, case blocks
- 6px: Badges, tags, avatars
- 8px: Icon containers
- 10px: Player number badges
- 12px: Standard cards, inputs, stat boxes
- 14px: Navigation items
- 16px: Large cards, sections
- 20px: Navigation container, pill badges
- 24px: Feature display cards

## 1.5 Shadow Definitions

**Card Shadow:**
```css
box-shadow: 0 4px 20px rgba(0, 0, 0, 0.4);
```

**Feature Card Shadow:**
```css
box-shadow: 0 0 40px rgba(180, 215, 232, 0.08), 0 10px 40px rgba(0, 0, 0, 0.3);
```

**Navigation Shadow:**
```css
box-shadow: 0 -10px 40px rgba(0, 0, 0, 0.3);
```

**Neon Glow (Amber):**
```css
box-shadow: 0 0 12px rgba(255, 170, 0, 0.4);
```

**Neon Glow (Powder Blue):**
```css
box-shadow: 0 2px 8px rgba(180, 215, 232, 0.3);
```

**Neon Text Glow (Amber):**
```css
text-shadow: 0 0 20px rgba(255, 170, 0, 0.6), 0 0 40px rgba(255, 170, 0, 0.3);
```

## 1.6 Global Background Treatment

**Body:**
- Background: `--arena-dark`
- Font family: Barlow, sans-serif
- Color: `--text-primary`

**Noise Texture Overlay:**
Apply a fixed pseudo-element covering entire viewport:
- SVG noise filter with fractalNoise, baseFrequency 0.9, numOctaves 4
- Opacity: 0.03
- pointer-events: none
- z-index: 1000

## 1.7 App Container

- Max width: 420px
- Centered horizontally (margin: 0 auto)
- Min height: 100vh
- Display: flex, flex-direction: column
- Background: Linear gradient from `rgba(180, 215, 232, 0.03)` at top to transparent at 30%, over `--arena-dark`

**Decorative Red Line (pseudo-element):**
- Position: absolute, top 0, left 50%, transform translateX(-50%)
- Width: 4px, Height: 100px
- Background: linear-gradient from `--hot-red` to transparent
- Opacity: 0.3

---

# PART 2: GLOBAL COMPONENTS

## 2.1 Header Component

**Container:**
- Padding: 20px horizontal, 20px top, 16px bottom
- Display: flex, align-items: center, gap: 16px
- Border bottom: 1px solid rgba(255, 255, 255, 0.05)

**Team Logo:**
- Size: 56px × 56px
- Border radius: 12px
- Background: linear-gradient(135deg, --boards-mid 0%, --boards-dark 100%)
- Border: 2px solid rgba(180, 215, 232, 0.3)
- Box shadow: 0 4px 20px rgba(0, 0, 0, 0.4), inset 0 1px 0 rgba(255, 255, 255, 0.05)
- Display: flex, centered
- Contains: 🐧 emoji at 28px font-size
- Has shine overlay pseudo-element (45deg gradient with 3% white)

**Team Info:**
- h1: "PENGUINS" — Bebas Neue, 28px, letter-spacing 2px, uppercase
- Text fill: Gradient from #fff to #a8b5c4 (use background-clip: text)
- Subtitle: "Thursday Night League"
- Font: Barlow, 11px, --text-muted, uppercase, letter-spacing 1.5px, margin-top 2px

## 2.2 Bottom Navigation

**Outer Container:**
- Position: fixed, bottom: 0
- Left: 50%, transform: translateX(-50%)
- Width: 100%, max-width: 420px
- Background: linear-gradient(180deg, transparent 0%, --arena-dark 20%)
- Padding: 20px 16px 24px

**Inner Nav Container:**
- Display: flex, justify-content: space-around
- Background: --boards-dark
- Border radius: 20px
- Padding: 8px
- Border: 1px solid rgba(255, 255, 255, 0.05)
- Box shadow: 0 -10px 40px rgba(0, 0, 0, 0.3)

**Nav Item (5 items):**
- Display: flex, flex-direction: column, align-items: center
- Padding: 10px 14px
- Border radius: 14px
- Cursor: pointer
- Background: transparent
- Border: none
- Color: --text-muted
- Transition: all 0.2s ease

**Nav Item Content:**
- Icon: 20px font-size (emoji), margin-bottom 4px
- Label: 10px, font-weight 600, uppercase, letter-spacing 0.5px

**Nav Items (in order):**
1. 📅 "Schedule"
2. 💬 "Chat"
3. 🍺 "Beers"
4. 📋 "Duties"
5. 👥 "Roster"

**Hover State:**
- Color: --text-primary

**Active State:**
- Background: linear-gradient(135deg, rgba(180, 215, 232, 0.15) 0%, rgba(180, 215, 232, 0.05) 100%)
- Color: --powder-blue
- Has top indicator pseudo-element:
  - Position absolute, top 0, left 50%, transform translateX(-50%)
  - Size: 20px × 3px
  - Background: --powder-blue
  - Border radius: 0 0 3px 3px
  - Box shadow: 0 0 10px var(--powder-blue-glow)

## 2.3 Main Content Area

- Flex: 1
- Overflow-y: auto
- Padding: 20px all sides
- Padding-bottom: 100px (clears fixed nav)

## 2.4 Tab Content Visibility

- Default: display none
- Active class: display block
- Animation on active: fadeIn 0.3s ease

**fadeIn Keyframes:**
```css
from { opacity: 0; transform: translateY(10px); }
to { opacity: 1; transform: translateY(0); }
```

## 2.5 Section Title Component

- Font: Bebas Neue, 14px
- Letter spacing: 3px
- Color: --text-muted
- Text transform: uppercase
- Margin bottom: 16px

---

# PART 3: SCHEDULE PAGE

## 3.1 Page Overview

The Schedule page displays upcoming games. It has one featured "Next Game" card at the top, followed by a list of future games.

## 3.2 Next Game Card

**Container:**
- Background: linear-gradient(135deg, --boards-mid 0%, --boards-dark 100%)
- Border radius: 16px
- Padding: 24px
- Margin bottom: 20px
- Position: relative
- Overflow: hidden
- Border: 1px solid rgba(180, 215, 232, 0.2)
- Box shadow: 0 0 40px rgba(180, 215, 232, 0.08), 0 10px 40px rgba(0, 0, 0, 0.3)

**Top Gradient Bar (pseudo-element ::before):**
- Position: absolute, top 0, left 0, right 0
- Height: 3px
- Background: linear-gradient(90deg, --powder-blue, --neon-amber, --powder-blue)

**Badge ("⚡ NEXT GAME"):**
- Display: inline-block
- Font: Bebas Neue, 11px, letter-spacing 2px
- Color: --powder-blue
- Text shadow: 0 0 10px var(--powder-blue-glow)
- Margin bottom: 12px
- Content: "⚡ NEXT GAME"

**Opponent Name:**
- Font: Bebas Neue, 36px, letter-spacing 1px
- Color: --text-primary
- Format: "<span>vs</span> POLAR BEARS"
- The "vs" span: color --text-muted, font-size 24px
- Margin bottom: 4px

**Details Row:**
- Display: flex, gap 24px
- Margin top: 16px

**Detail Item (2 items):**
- Display: flex, align-items center, gap 8px

**Detail Icon Container:**
- Size: 32px × 32px
- Background: rgba(255, 255, 255, 0.05)
- Border radius: 8px
- Display: flex, centered
- Font size: 14px (emoji)

**Detail Info:**
- Label: 12px, --text-muted
- Value: 14px, --text-primary, font-weight 600, display block

**Detail Item 1:**
- Icon: 📅
- Strong: "Thu, Nov 28"
- Text: "9:15 PM"

**Detail Item 2:**
- Icon: 🏟️
- Strong: "Twin Rinks"
- Text: "East Arena"

## 3.3 Upcoming Games Section

**Section Title:**
- Text: "UPCOMING"
- Uses Section Title Component (see 2.5)

## 3.4 Game List Item (3 items)

**Container:**
- Background: --boards-dark
- Border radius: 12px
- Padding: 16px
- Margin bottom: 10px
- Display: flex, align-items center, justify-content space-between
- Border: 1px solid rgba(255, 255, 255, 0.03)
- Transition: all 0.2s ease

**Hover State:**
- Border color: rgba(255, 255, 255, 0.08)
- Transform: translateX(4px)

**Left Content:**
- h3: Opponent name, 15px, font-weight 600, margin-bottom 4px
- p: Date, 12px, --text-muted

**Right Content:**
- Text align: right
- Time: Bebas Neue, 20px, letter-spacing 1px
- Rink: 11px, --text-muted

**Game Data:**

| Opponent | Date | Time | Rink |
|----------|------|------|------|
| vs Ice Hogs | Thu, Dec 5 | 8:30 PM | Twin Rinks - West |
| vs Frozen Four | Thu, Dec 12 | 9:45 PM | Memorial Arena |
| vs Night Owls | Thu, Dec 19 | 8:15 PM | Twin Rinks - East |

---

# PART 4: CHAT PAGE

## 4.1 Page Overview

Simple team chat interface with message history and input field at bottom.

## 4.2 Page Structure

**Section Title:**
- Text: "TEAM CHAT"
- Uses Section Title Component

**Chat Container:**
- Display: flex, flex-direction column
- Height: calc(100vh - 220px)

## 4.3 Chat Messages Area

**Container:**
- Flex: 1
- Overflow-y: auto
- Padding bottom: 16px

## 4.4 Chat Message Component (4 messages)

**Message Container:**
- Margin bottom: 16px
- Animation: slideIn 0.3s ease

**slideIn Keyframes:**
```css
from { opacity: 0; transform: translateX(-10px); }
to { opacity: 1; transform: translateX(0); }
```

**Message Header:**
- Display: flex, align-items center, gap 8px
- Margin bottom: 6px

**Avatar:**
- Size: 28px × 28px
- Background: linear-gradient(135deg, --powder-blue 0%, --powder-blue-deep 100%)
- Border radius: 6px
- Display: flex, centered
- Font: 12px, bold, --arena-dark color
- Content: User initials (2 letters)

**Sender Name:**
- Font weight: 600
- Font size: 13px
- Color: --text-primary

**Timestamp:**
- Font size: 11px
- Color: --text-muted
- Margin-left: auto

**Message Bubble:**
- Background: --boards-mid
- Padding: 12px 16px
- Border radius: 4px 16px 16px 16px (creates speech bubble pointing top-left)
- Font size: 14px
- Line height: 1.5
- Margin left: 36px (aligns past avatar)
- Border: 1px solid rgba(255, 255, 255, 0.03)

**Chat Message Data:**

| Avatar | Sender | Time | Message |
|--------|--------|------|---------|
| MS | Mike S. | 2:30 PM | Everyone good for Thursday? Need a headcount for the beer run 🍺 |
| JT | Jake T. | 2:45 PM | I got the beer this week. Grabbing 2 cases of Molson and a 12-pack of Coors Light for the lightweights 😂 |
| DW | Dave W. | 3:12 PM | Nice! I'll bring the big cooler. Should fit everything plus ice |
| TB | Tom B. | 4:01 PM | Might be 10 mins late, save me a cold one! 🏒 |

## 4.5 Chat Input Area

**Container:**
- Display: flex, gap 10px
- Padding top: 16px
- Border top: 1px solid rgba(255, 255, 255, 0.05)

**Text Input:**
- Flex: 1
- Background: --boards-dark
- Border: 1px solid rgba(255, 255, 255, 0.08)
- Border radius: 12px
- Padding: 14px 18px
- Font: Barlow, 14px
- Color: --text-primary
- Transition: all 0.2s ease

**Input Placeholder:**
- Text: "Type a message..."
- Color: --text-muted

**Input Focus State:**
- Outline: none
- Border color: --powder-blue
- Box shadow: 0 0 20px rgba(180, 215, 232, 0.15)

**Send Button:**
- Size: 50px × 50px
- Background: linear-gradient(135deg, --powder-blue 0%, --powder-blue-deep 100%)
- Border: none
- Border radius: 12px
- Color: --arena-dark
- Font size: 18px
- Cursor: pointer
- Display: flex, centered
- Content: ➤ (arrow character)
- Transition: all 0.2s ease

**Send Button Hover:**
- Transform: scale(1.05)
- Box shadow: 0 0 25px var(--powder-blue-glow)

---

# PART 5: BEER COUNTER PAGE

## 5.1 Page Overview

Tracks beer consumption by cases. Displays 52 blocks (one per week of season). Each case = 30 beers. Tap blocks to mark consumed.

## 5.2 Page Structure

**Outer Container:**
- Padding: 10px 0

## 5.3 Beer Header

**Container:**
- Text align: center
- Margin bottom: 24px

**Total Row:**
- Display: flex, justify-content center, align-items baseline, gap 8px

**Case Count (Large Number):**
- Font: Bebas Neue, 72px
- Color: --neon-amber
- Text shadow: 0 0 20px rgba(255, 170, 0, 0.6), 0 0 40px rgba(255, 170, 0, 0.3)
- Line height: 1
- ID: "beerTotal"
- Initial value: "4"

**Unit Label:**
- Font: Bebas Neue, 24px
- Color: --text-muted
- Letter spacing: 1px
- Text: "cases"

**Subtitle:**
- Font size: 12px
- Color: --text-muted
- Text transform: uppercase
- Letter spacing: 2px
- Margin top: 8px
- Text: "Consumed This Season"

**Beer Count Substat:**
- Font size: 14px
- Color: --neon-amber
- Margin top: 4px
- Format: "<span id='beerCount'>120</span> beers <span>(30 per case)</span>"
- The "(30 per case)" span: color --text-muted

## 5.4 Cases Section

**Container:**
- Background: --boards-dark
- Border radius: 16px
- Padding: 20px
- Margin bottom: 20px
- Border: 1px solid rgba(255, 255, 255, 0.05)

**Section Header:**
- Display: flex, justify-content space-between, align-items center
- Margin bottom: 16px

**Section Title:**
- Font: Bebas Neue, 14px
- Letter spacing: 2px
- Color: --text-muted
- Text: "SEASON TRACKER"

**Section Count:**
- Font size: 13px
- Color: --powder-blue
- Format: "<span id='consumedCount'>4</span> / 52 cases"

## 5.5 Cases Grid

**Container:**
- Display: grid
- Grid template columns: repeat(13, 1fr)
- Gap: 6px
- ID: "casesGrid"

**Case Block (52 total):**
- Aspect ratio: 1 (square)
- Border radius: 4px
- Background: --boards-mid
- Border: 1px solid rgba(255, 255, 255, 0.08)
- Transition: all 0.15s ease
- Cursor: pointer
- Position: relative
- Overflow: hidden

**Case Block Hover:**
- Transform: scale(1.15)
- Z-index: 1
- Border color: rgba(255, 255, 255, 0.2)

**Case Block Consumed State (.consumed):**
- Background: linear-gradient(135deg, --neon-amber 0%, #cc8800 100%)
- Border color: rgba(255, 170, 0, 0.6)
- Box shadow: 0 0 12px rgba(255, 170, 0, 0.4)

**Consumed Block Shine (::before pseudo-element):**
- Position: absolute, top 0, left 0, right 0
- Height: 45%
- Background: linear-gradient(180deg, rgba(255, 255, 255, 0.35) 0%, transparent 100%)
- Border radius: 3px 3px 50% 50%

**Consumed Block Hover:**
- Box shadow: 0 0 20px rgba(255, 170, 0, 0.6)

## 5.6 Beer Legend

**Container:**
- Display: flex, justify-content center, gap 24px
- Margin bottom: 24px

**Legend Item:**
- Display: flex, align-items center, gap 8px
- Font size: 12px
- Color: --text-muted

**Legend Block:**
- Size: 18px × 18px
- Border radius: 4px

**Empty Legend Block:**
- Background: --boards-mid
- Border: 1px solid rgba(255, 255, 255, 0.1)
- Label: "Remaining"

**Full Legend Block:**
- Background: linear-gradient(135deg, --neon-amber 0%, #cc8800 100%)
- Box shadow: 0 0 8px rgba(255, 170, 0, 0.3)
- Label: "Consumed"

## 5.7 Beer Stats Grid

**Container:**
- Display: grid
- Grid template columns: 1fr 1fr
- Gap: 12px

**Stat Box (4 total):**
- Background: --boards-dark
- Border radius: 12px
- Padding: 16px
- Border: 1px solid rgba(255, 255, 255, 0.03)

**Stat Value:**
- Font: Bebas Neue, 28px
- Color: --neon-amber

**Stat Label:**
- Font size: 11px
- Color: --text-muted
- Text transform: uppercase
- Letter spacing: 1px

**Stats Data:**

| Value | Label |
|-------|-------|
| 10 | Avg / Game |
| 12 | Games Played |
| Jake T. | Top Consumer |
| 18 | Most in a Game |

## 5.8 Beer Page Behavior

**State:**
- `TOTAL_CASES`: 52 (constant)
- `BEERS_PER_CASE`: 30 (constant)
- `consumedCases`: 4 (initial value, mutable)

**Toggle Logic (on block click):**
```javascript
function toggleCase(index) {
  if (index < consumedCases) {
    // Clicking consumed block: unconsume from that point
    consumedCases = index;
  } else {
    // Clicking unconsumed block: consume up to and including it
    consumedCases = index + 1;
  }
  renderCases();
}
```

**Update Counts:**
```javascript
function updateBeerCounts() {
  const totalBeers = consumedCases * BEERS_PER_CASE;
  document.getElementById('beerTotal').textContent = consumedCases;
  document.getElementById('beerCount').textContent = totalBeers;
  document.getElementById('consumedCount').textContent = consumedCases;
}
```

**Render Logic:**
- Loop 0 to 51
- If index < consumedCases, add "consumed" class
- Each block gets onclick="toggleCase(index)"

---

# PART 6: DUTIES PAGE

## 6.1 Page Overview

Shows four duty assignments for current week's game: Ice Fee, Beer Run, Cooler, Food & Snacks.

## 6.2 Page Header

**Section Title:**
- Text: "THIS WEEK'S DUTIES"
- Uses Section Title Component

**Subheader:**
- Font size: 13px
- Color: --text-muted
- Margin bottom: 20px
- Text: "Thursday, Nov 28 vs Polar Bears"

## 6.3 Duty Card Component (4 cards)

**Container:**
- Background: --boards-dark
- Border radius: 16px
- Padding: 20px
- Margin bottom: 12px
- Display: flex, align-items center, gap 16px
- Border: 1px solid rgba(255, 255, 255, 0.03)
- Transition: all 0.3s ease
- Position: relative
- Overflow: hidden

**Left Accent Bar (::before pseudo-element):**
- Position: absolute, left 0, top 0, bottom 0
- Width: 4px
- Color varies by duty type (see below)

**Hover State:**
- Transform: translateX(8px)
- Border color: rgba(255, 255, 255, 0.08)

**Icon Container:**
- Size: 52px × 52px
- Border radius: 12px
- Display: flex, centered
- Font size: 24px (emoji)
- Background: 15% opacity version of accent color

**Info Section:**
- Flex: 1

**Role Label:**
- Font size: 12px
- Color: --text-muted
- Text transform: uppercase
- Letter spacing: 1px
- Margin bottom: 4px

**Name:**
- Font weight: 600
- Font size: 16px
- Color: --text-primary

**Badge:**
- Font: Bebas Neue, 18px
- Padding: 6px 12px
- Border radius: 6px
- Background: rgba(180, 215, 232, 0.1)
- Color: --powder-blue
- Content: Jersey number with #

## 6.4 Duty Card Variants

**Ice Fee (.ice):**
- Accent bar: --ice-blue (#00d4ff)
- Icon background: rgba(0, 212, 255, 0.15)
- Icon: 🧊
- Role: "Ice Fee"
- Name: "Mike Sullivan"
- Badge: "#12"

**Beer Run (.beer):**
- Accent bar: --neon-amber (#ffaa00)
- Icon background: rgba(255, 170, 0, 0.15)
- Icon: 🍺
- Role: "Beer Run"
- Name: "Jake Thompson"
- Badge: "#7"

**Cooler (.cooler):**
- Accent bar: #8b5cf6
- Icon background: rgba(139, 92, 246, 0.15)
- Icon: ❄️
- Role: "Cooler"
- Name: "Dave Wilson"
- Badge: "#4"

**Food (.food):**
- Accent bar: #10b981
- Icon background: rgba(16, 185, 129, 0.15)
- Icon: 🍕
- Role: "Food & Snacks"
- Name: "Ryan Cooper"
- Badge: "#88"

---

# PART 7: ROSTER PAGE

## 7.1 Page Overview

Displays team roster with attendance tracking for current week. Shows summary counts and tappable player cards to toggle status.

## 7.2 Page Header

**Section Title:**
- Text: "ROSTER & ATTENDANCE"
- Uses Section Title Component

## 7.3 Attendance Summary

**Container:**
- Display: grid
- Grid template columns: repeat(3, 1fr)
- Gap: 10px
- Margin bottom: 24px

**Summary Box:**
- Text align: center
- Padding: 16px 12px
- Border radius: 12px
- Border: 1px solid rgba(255, 255, 255, 0.05)

**Box Variants:**

**.in (Playing):**
- Background: rgba(16, 185, 129, 0.1)
- Border color: rgba(16, 185, 129, 0.3)
- Count color: #10b981
- ID: "inCount"
- Label: "Playing"

**.out (Out):**
- Background: rgba(255, 59, 59, 0.1)
- Border color: rgba(255, 59, 59, 0.3)
- Count color: #ff3b3b
- ID: "outCount"
- Label: "Out"

**.pending (Pending):**
- Background: rgba(255, 170, 0, 0.1)
- Border color: rgba(255, 170, 0, 0.3)
- Count color: --neon-amber
- ID: "pendingCount"
- Label: "Pending"

**Count Number:**
- Font: Bebas Neue, 36px
- Line height: 1
- Color: varies by variant

**Label:**
- Font size: 10px
- Text transform: uppercase
- Letter spacing: 1.5px
- Color: --text-muted
- Margin top: 4px

**Initial Values:**
- Playing: 6
- Out: 2
- Pending: 2

## 7.4 Hint Text

- Text: "Tap a player to change status"
- Font size: 11px
- Color: --text-muted
- Margin bottom: 12px

## 7.5 Roster List

**Container:**
- Display: flex, flex-direction column, gap 8px
- ID: "rosterList"

## 7.6 Player Card Component (10 cards)

**Container:**
- Background: --boards-dark
- Border radius: 12px
- Padding: 12px 16px
- Display: flex, align-items center, gap 14px
- Cursor: pointer
- Border: 1px solid rgba(255, 255, 255, 0.03)
- Transition: all 0.2s ease

**Hover State:**
- Background: --boards-mid
- Transform: scale(1.01)

**Active/Pressed State:**
- Transform: scale(0.99)

**Jersey Number Badge:**
- Size: 44px × 44px
- Background: linear-gradient(135deg, --powder-blue 0%, --powder-blue-deep 100%)
- Border radius: 10px
- Display: flex, centered
- Font: Bebas Neue, 22px
- Color: --arena-dark
- Box shadow: 0 2px 8px rgba(180, 215, 232, 0.3)

**Info Section:**
- Flex: 1

**Player Name:**
- Font weight: 600
- Font size: 15px
- Margin bottom: 2px

**Position:**
- Font size: 11px
- Color: --text-muted
- Text transform: uppercase
- Letter spacing: 1px

**Status Badge:**
- Padding: 6px 14px
- Border radius: 20px (pill)
- Font size: 11px
- Font weight: 700
- Text transform: uppercase
- Letter spacing: 1px

**Status Variants:**

**.in:**
- Background: rgba(16, 185, 129, 0.2)
- Color: #10b981
- Text: "IN"

**.out:**
- Background: rgba(255, 59, 59, 0.2)
- Color: #ff3b3b
- Text: "OUT"

**.pending:**
- Background: rgba(255, 170, 0, 0.2)
- Color: --neon-amber
- Text: "?"

## 7.7 Player Data

| ID | Name | Number | Position | Initial Status |
|----|------|--------|----------|----------------|
| 1 | Mike Sullivan | 12 | Center | in |
| 2 | Jake Thompson | 7 | Left Wing | in |
| 3 | Chris Martin | 22 | Right Wing | out |
| 4 | Dave Wilson | 4 | Defense | in |
| 5 | Ryan Cooper | 88 | Defense | pending |
| 6 | Tom Bradley | 31 | Goalie | in |
| 7 | Steve Rogers | 19 | Center | in |
| 8 | Paul Anderson | 55 | Left Wing | out |
| 9 | Matt Parker | 8 | Defense | pending |
| 10 | Kevin James | 91 | Right Wing | in |

## 7.8 Roster Page Behavior

**Toggle Status Logic:**
```javascript
function toggleStatus(playerId) {
  const player = players.find(p => p.id === playerId);
  if (player.status === 'in') player.status = 'out';
  else if (player.status === 'out') player.status = 'pending';
  else player.status = 'in';
  renderRoster();
}
```

**Status Cycle:** in → out → pending → in

**Update Counts:**
```javascript
function updateCounts() {
  document.getElementById('inCount').textContent = players.filter(p => p.status === 'in').length;
  document.getElementById('outCount').textContent = players.filter(p => p.status === 'out').length;
  document.getElementById('pendingCount').textContent = players.filter(p => p.status === 'pending').length;
}
```

---

# PART 8: GLOBAL INTERACTIONS

## 8.1 Tab Switching

**Function:**
```javascript
function switchTab(tabId) {
  // Remove active from all tabs
  document.querySelectorAll('.tab-content').forEach(tab => {
    tab.classList.remove('active');
  });
  // Remove active from all nav items
  document.querySelectorAll('.nav-item').forEach(item => {
    item.classList.remove('active');
  });
  
  // Activate selected tab
  document.getElementById(tabId).classList.add('active');
  // Activate clicked nav item
  event.currentTarget.classList.add('active');
}
```

## 8.2 Initialization

On page load:
1. Call `renderRoster()` to populate player list
2. Call `renderCases()` to populate beer grid

## 8.3 Scrollbar Styling

```css
::-webkit-scrollbar {
  width: 4px;
}
::-webkit-scrollbar-track {
  background: transparent;
}
::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 2px;
}
```

---

# PART 9: IMPLEMENTATION CHECKLIST

## 9.1 Assets Required

- [ ] Google Font: Bebas Neue (Regular 400)
- [ ] Google Font: Barlow (400, 500, 600, 700)

## 9.2 Build Order

1. Set up HTML document structure
2. Import Google Fonts
3. Define CSS custom properties (color tokens)
4. Implement global styles (body, scrollbar, noise overlay)
5. Build App Container with decorative elements
6. Build Header component
7. Build Bottom Navigation
8. Build Main Content container
9. Implement tab visibility CSS
10. Build Schedule page components
11. Build Chat page components
12. Build Beer Counter page components
13. Build Duties page components
14. Build Roster page components
15. Implement JavaScript state management
16. Implement tab switching
17. Implement beer counter logic
18. Implement roster toggle logic
19. Initialize on page load

## 9.3 Data Models

**Player:**
```javascript
{
  id: number,
  name: string,
  number: number,
  position: string,
  status: 'in' | 'out' | 'pending'
}
```

**Game:**
```javascript
{
  opponent: string,
  date: string,
  time: string,
  rink: string,
  isNext: boolean
}
```

**Duty:**
```javascript
{
  type: 'ice' | 'beer' | 'cooler' | 'food',
  role: string,
  name: string,
  number: number
}
```

**ChatMessage:**
```javascript
{
  initials: string,
  sender: string,
  time: string,
  message: string
}
```

---

# PART 10: VALIDATION CRITERIA

To confirm pixel-perfect recreation, verify:

## Visual Checks

- [ ] Dark arena background (#0a0c10) covers entire viewport
- [ ] Noise texture overlay visible at 3% opacity
- [ ] Powder blue (#b4d7e8) appears on team logo border, chat avatars, send button, player jersey numbers, duty badges, nav active state
- [ ] Neon amber (#ffaa00) appears on beer counter, consumed blocks, beer stats, pending status
- [ ] Gradient text on "PENGUINS" header (white to gray)
- [ ] 3px tri-color gradient bar at top of Next Game card
- [ ] Bebas Neue font renders correctly for all headlines and counters
- [ ] Barlow font renders correctly for all body text

## Interaction Checks

- [ ] Tapping nav items switches active tab with animation
- [ ] Active nav item shows powder blue highlight and top indicator bar
- [ ] Game list items shift right 4px on hover
- [ ] Chat input border glows powder blue on focus
- [ ] Send button scales up on hover
- [ ] Beer blocks scale up on hover
- [ ] Consumed beer blocks have amber glow
- [ ] Tapping beer block fills/unfills grid sequentially
- [ ] Beer counter updates in real-time
- [ ] Duty cards shift right 8px on hover
- [ ] Player cards scale slightly on hover
- [ ] Tapping player cycles status: IN → OUT → ? → IN
- [ ] Attendance summary counts update in real-time

## Layout Checks

- [ ] App container maxes at 420px width
- [ ] App container is horizontally centered
- [ ] Bottom nav is fixed at bottom
- [ ] Content scrolls independently
- [ ] 100px bottom padding prevents content hiding behind nav
- [ ] Beer grid is 13 columns × 4 rows (52 total blocks)

---

*Specification Complete — Follow parts 1-10 in sequence for pixel-perfect recreation*
