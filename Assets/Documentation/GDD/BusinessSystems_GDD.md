# Jacameno: Adult Life - Business Systems Game Design Document
## Version 1.0 | Studio-Ready GDD

---

# Table of Contents

1. [Overview](#overview)
2. [Common Business Systems](#common-business-systems)
3. [Business 1: Hypermarket](#business-1-hypermarket)
4. [Business 2: Retail Fashion Store](#business-2-retail-fashion-store)
5. [Business 3: Restaurant](#business-3-restaurant)
6. [Business 4: Construction/Home Remodeling](#business-4-constructionhome-remodeling)
7. [Business 5: Uber/Taxi Company](#business-5-ubertaxi-company)
8. [Technical Specifications](#technical-specifications)
9. [UI Wireframes](#ui-wireframes)

---

# Overview

## Design Philosophy

"Jacameno: Adult Life" delivers AAA-quality business simulation where players experience the full journey from solo entrepreneur to business mogul. Each of the five starter businesses provides unique gameplay loops while sharing common progression and management systems.

### Core Pillars

1. **Owner-Employee Duality**: Player begins as sole owner AND employee, performing all tasks personally before growing to delegate
2. **Operational Realism**: Focus on legitimate business challenges - no theft mechanics; instead: supply delays, equipment breakdowns, inspections, weather events, competitor actions
3. **Meaningful Progression**: Unlock floor expansions, product tiers, decorations, equipment, employee slots, marketing, and specialized services
4. **3rd-Person Immersion**: NFS-level environmental fidelity with day/night cycles, weather, realistic NPC behaviors

---

# Common Business Systems

## Business Points (BP) Currency

Business Points serve as the meta-progression currency earned through:

| Source | BP Earned |
|--------|-----------|
| Daily profit milestone ($500+) | 5 BP |
| Daily profit milestone ($1,000+) | 10 BP |
| Weekly profit milestone ($5,000+) | 25 BP |
| Monthly profit milestone ($20,000+) | 100 BP |
| Customer satisfaction 4.5+ stars | 15 BP/week |
| Mission completion | 10-50 BP |
| Perfect day (no incidents) | 5 BP |
| Staff training completed | 10 BP |

### BP Unlock Categories

- Floor Expansions: 50-500 BP
- Equipment Upgrades: 25-200 BP
- Decoration Tiers: 10-100 BP
- Employee Slots: 75 BP per slot
- Marketing Campaigns: 50-150 BP
- Specialized Services: 100-300 BP

---

## Reputation & Rating System

### Rating Components

| Factor | Weight | Description |
|--------|--------|-------------|
| Service Speed | 25% | Time from customer arrival to service completion |
| Quality | 30% | Product/service quality based on staff skills and equipment |
| Cleanliness | 15% | Environment cleanliness level |
| Ambiance | 15% | Decorations, music, lighting |
| Value | 15% | Price-to-quality ratio |

### Rating Tiers

| Stars | Foot Traffic Multiplier | Price Markup Allowed |
|-------|------------------------|---------------------|
| 1.0 - 1.9 | 0.5x | -20% |
| 2.0 - 2.9 | 0.75x | -10% |
| 3.0 - 3.4 | 1.0x | Base |
| 3.5 - 3.9 | 1.25x | +10% |
| 4.0 - 4.4 | 1.5x | +20% |
| 4.5 - 5.0 | 2.0x | +30% |

---

## Event System

### Event Categories

#### Supply Events
- **Supply Delay**: 30% chance when ordering, affects specific product category for 1-3 days
- **Supply Shortage**: Regional shortage, 10% chance, affects all businesses, 3-7 days
- **Bulk Discount**: 15% chance, 20% off certain supplies for 24 hours

#### Equipment Events
- **Minor Breakdown**: Equipment operates at 50% efficiency for 2-4 hours until repaired
- **Major Breakdown**: Equipment non-functional, requires repair person, 4-24 hour downtime
- **Maintenance Due**: Scheduled notification, ignoring increases breakdown chance by 50%

#### Inspection Events
- **Health Inspection**: 2-week notice, affects restaurants and hypermarkets
- **Safety Inspection**: 1-week notice, affects all businesses
- **Surprise Audit**: No notice, checks financial records, affects all businesses

#### Weather Events
- **Rain**: -20% foot traffic outdoor areas, +10% delivery demand
- **Storm**: -50% foot traffic, potential power outage
- **Heat Wave**: +20% cold product demand, equipment strain
- **Snow**: -40% foot traffic, supply delay risk

#### Competitor Events
- **New Competitor**: Opens nearby, -15% foot traffic for 2 weeks
- **Competitor Sale**: -10% revenue for 3 days
- **Competitor Closure**: +20% foot traffic permanent

---

## Staff System

### Universal Staff Attributes

| Attribute | Range | Effect |
|-----------|-------|--------|
| Speed | 1-10 | Task completion time = Base / (Speed * 0.2) |
| Accuracy | 1-10 | Error rate = (10 - Accuracy) * 2% |
| Charisma | 1-10 | Customer satisfaction bonus = Charisma * 3% |
| Maintenance | 1-10 | Equipment durability bonus = Maintenance * 5% |
| Stamina | 1-10 | Efficiency drop rate over shift |
| Loyalty | 1-10 | Quit chance = (10 - Loyalty) * 1% per week |

### Staff Skill Tree (Universal)

```
TIER 1 (Base)
├── Basic Training (+5% task speed)
├── Customer Service 101 (+5% satisfaction)
└── Safety Training (unlock equipment access)

TIER 2 (Requires 50 XP)
├── Efficiency I (+10% speed)
├── Quality Focus (+10% accuracy)
├── Team Player (+5% nearby staff performance)
└── Equipment Handler (-10% breakdown chance)

TIER 3 (Requires 150 XP)
├── Senior Training (+15% overall performance)
├── Specialization (role-specific bonuses)
├── Leadership (can supervise 2 junior staff)
└── Crisis Management (+20% performance during events)

TIER 4 (Requires 300 XP)
├── Expert (+25% role performance)
├── Mentor (train other staff 2x faster)
├── Department Head (supervise 5 staff)
└── Owner's Right Hand (can manage business when owner absent)
```

### Shift Scheduling

| Shift | Hours | Wage Multiplier |
|-------|-------|-----------------|
| Morning | 6 AM - 2 PM | 1.0x |
| Afternoon | 2 PM - 10 PM | 1.0x |
| Evening | 10 PM - 6 AM | 1.5x |
| Split | Custom 4-hour blocks | 1.1x |
| On-Call | As needed | 2.0x per hour |

---

## Daily Business Loop

```
1. OPEN BUSINESS
   ├── Check overnight events/deliveries
   ├── Review day's schedule
   ├── Verify staff attendance
   └── Unlock doors / activate systems

2. MORNING OPERATIONS (Peak 1)
   ├── Serve customers
   ├── Process orders/deliveries
   ├── Handle mini-games
   └── Monitor staff performance

3. MIDDAY LULL
   ├── Restock inventory
   ├── Equipment maintenance
   ├── Staff breaks
   └── Review metrics

4. AFTERNOON/EVENING OPERATIONS (Peak 2)
   ├── Serve customers
   ├── Handle rush periods
   ├── Process urgent tasks
   └── Delegate to staff

5. CLOSE BUSINESS
   ├── End-of-day inventory count
   ├── Cash register reconciliation
   ├── Cleaning tasks
   ├── Lock up / security check
   └── Review daily performance

6. EVENING (Post-Close)
   ├── Review dashboard
   ├── Plan next day
   ├── Order supplies
   ├── Schedule staff
   └── Unlock upgrades
```

---

# Business 1: Hypermarket

## Overview

The Hypermarket is a large retail supermarket where players manage inventory, staff, and customer flow across multiple departments including Grocery, Fresh Produce, Bakery, Deli, and Household goods.

---

## Core Loop

```
HYPERMARKET DAILY CYCLE

MORNING RUSH (6 AM - 10 AM)
├── Receive deliveries → Stock shelves → Open store
├── Handle early customers (senior shoppers, professionals)
├── Process special orders
└── Mini-games: Stocking Speed, Delivery Intake

MIDDAY (10 AM - 2 PM)
├── Steady customer flow
├── Inventory management
├── Fresh product rotation
├── Staff lunch breaks
└── Mini-games: Shelf Arrangement, Price Changes

AFTERNOON RUSH (2 PM - 7 PM)
├── School release → family shoppers
├── Peak checkout lines
├── Restock fast-moving items
└── Mini-games: Cashier Speed, Customer Assistance

EVENING (7 PM - 10 PM)
├── Late shoppers
├── Day-old product marking
├── Initial cleaning
└── Mini-games: Inventory Count, Closing Prep

CLOSE (10 PM)
├── Register reconciliation
├── Final inventory check
├── Deep cleaning
└── Security activation
```

---

## Progression Tiers

### Tier 1: Corner Store (Starting)
- **Floor Space**: 500 sq ft
- **Departments**: 1 (Grocery basics)
- **Max Staff**: 0 (player only)
- **Product SKUs**: 50
- **Daily Customer Cap**: 30
- **Equipment**: 1 register, basic shelving

### Tier 2: Neighborhood Market ($5,000 + 100 BP)
- **Floor Space**: 1,500 sq ft
- **Departments**: 2 (+ Fresh Produce)
- **Max Staff**: 2
- **Product SKUs**: 150
- **Daily Customer Cap**: 75
- **Equipment**: 2 registers, refrigeration unit

### Tier 3: Community Supermarket ($15,000 + 250 BP)
- **Floor Space**: 4,000 sq ft
- **Departments**: 4 (+ Bakery, Deli)
- **Max Staff**: 6
- **Product SKUs**: 400
- **Daily Customer Cap**: 200
- **Equipment**: 4 registers, full refrigeration, bakery ovens

### Tier 4: District Hypermarket ($50,000 + 500 BP)
- **Floor Space**: 10,000 sq ft
- **Departments**: 6 (+ Household, Electronics)
- **Max Staff**: 15
- **Product SKUs**: 1,000
- **Daily Customer Cap**: 500
- **Equipment**: 8 registers, self-checkout, full kitchen

### Tier 5: Regional Megamart ($150,000 + 1,000 BP)
- **Floor Space**: 25,000+ sq ft
- **Departments**: 10 (+ Pharmacy, Garden, Auto)
- **Max Staff**: 40
- **Product SKUs**: 5,000+
- **Daily Customer Cap**: 1,500
- **Equipment**: Full automation options

---

## Staff Roles

### Cashier
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Speed > Accuracy > Charisma | $12/hr | Register operation, bagging, customer checkout |

**Skill Progression**:
- Level 1: Basic checkout
- Level 2: Express lane certified
- Level 3: Returns processing
- Level 4: Self-checkout oversight
- Level 5: Cash office access

### Stocker
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Speed > Maintenance > Stamina | $11/hr | Shelf stocking, facing, price tags, inventory |

**Skill Progression**:
- Level 1: Basic stocking
- Level 2: Heavy item certified
- Level 3: Refrigeration trained
- Level 4: Warehouse access
- Level 5: Inventory management

### Fresh Department Associate
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Accuracy > Quality > Speed | $13/hr | Produce handling, bakery, deli operations |

**Skill Progression**:
- Level 1: Product handling
- Level 2: Food safety certified
- Level 3: Preparation tasks
- Level 4: Department specialty
- Level 5: Fresh department lead

### Customer Service
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Charisma > Accuracy > Speed | $13/hr | Returns, complaints, special orders |

**Skill Progression**:
- Level 1: Basic inquiries
- Level 2: Returns processing
- Level 3: Complaint resolution
- Level 4: Special orders
- Level 5: Service desk manager

### Floor Manager
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Charisma > Accuracy > Speed | $18/hr | Staff supervision, scheduling, escalations |

**Skill Progression**:
- Level 1: Shift supervision
- Level 2: Schedule creation
- Level 3: Hiring/training
- Level 4: Multi-department oversight
- Level 5: Assistant store manager

---

## Task List & Mini-Games

### Stocking Mini-Game
**Mechanic**: Tetris-style placement
- Drag products to shelves in optimal arrangement
- Bonus points for grouping categories
- Time limit creates pressure
- Accuracy affects product visibility to customers

**Scoring**:
- Perfect placement: 100 XP
- Good placement: 50 XP
- Poor placement: 25 XP
- Missed deadline: 10 XP

### Cashier Mini-Game
**Mechanic**: Rhythm/Reaction
- Items scroll on conveyor belt
- Player scans (tap/click) at right moment
- Bagging prompt requires correct bag selection
- Cash/card handling mini-sequence

**Scoring**:
- Perfect scan streak: 150 XP
- Fast checkout: 100 XP
- Accurate change: 50 XP bonus
- Customer satisfaction: Variable XP

### Inventory Count Mini-Game
**Mechanic**: Memory/Speed
- Flash display of shelf contents
- Input counted quantities
- Spot discrepancies
- Time pressure increases with tier

### Delivery Intake Mini-Game
**Mechanic**: Sorting/Verification
- Check delivered items against order
- Flag damaged goods
- Route to correct departments
- Time and accuracy scoring

### Price Change Mini-Game
**Mechanic**: Pattern Recognition
- Identify items requiring price updates
- Apply correct labels
- Verify markdown accuracy
- Avoid pricing errors (customer complaints)

---

## Mission Chain

### Starter Mission: "First Day on the Job"
**Objective**: Successfully open and operate your first day
**Steps**:
1. Accept delivery (tutorial: delivery intake)
2. Stock initial shelves (tutorial: stocking)
3. Open store
4. Serve 10 customers
5. Close store and reconcile register
**Rewards**: $200, 50 XP, 10 BP

### Sample Missions

#### Mission: "Fresh Start"
**Tier**: 2 (Unlock Fresh Produce)
**Objective**: Establish fresh produce department
**Steps**:
1. Purchase refrigeration unit ($2,000)
2. Complete food safety training (mini-game)
3. Order first produce shipment
4. Set up display (arrangement puzzle)
5. Sell 50 produce items
**Rewards**: $500, 100 XP, 25 BP, Produce Department Unlocked

#### Mission: "Community Pillar"
**Tier**: 3 (Community Recognition)
**Objective**: Achieve 4-star rating and community partnership
**Steps**:
1. Maintain 4-star rating for 7 consecutive days
2. Host community event (blood drive, food drive)
3. Hire 3 local employees
4. Donate $1,000 to local charity
**Rewards**: $1,000, 200 XP, 50 BP, "Community Partner" badge (+5% foot traffic)

#### Mission: "Supply Chain Master"
**Tier**: 4 (Warehouse Access)
**Objective**: Establish direct supplier relationships
**Steps**:
1. Purchase warehouse expansion ($25,000)
2. Complete logistics training
3. Negotiate with 3 suppliers (dialogue mini-game)
4. Successfully manage 30-day inventory cycle
**Rewards**: $2,000, 300 XP, 75 BP, -15% supply costs permanent

---

## Unlock Tables

### Equipment Unlocks

| Item | Cost | BP Required | Tier | Effect |
|------|------|-------------|------|--------|
| Basic Register | Free | 0 | 1 | Process 1 customer at a time |
| Express Register | $500 | 25 | 2 | +30% checkout speed |
| Self-Checkout Station | $3,000 | 100 | 3 | Automated checkout, requires oversight |
| Refrigeration Unit | $2,000 | 50 | 2 | Enables fresh produce |
| Bakery Oven | $5,000 | 75 | 3 | Enables in-store bakery |
| Deli Slicer | $1,500 | 50 | 3 | Enables deli department |
| Forklift | $8,000 | 100 | 4 | +50% warehouse efficiency |
| Automated Inventory | $15,000 | 200 | 4 | Real-time inventory tracking |

### Decoration Unlocks

| Item | Cost | BP Required | Effect |
|------|------|-------------|--------|
| Basic Signage | $100 | 5 | +2% visibility |
| Floor Graphics | $500 | 20 | +5% customer flow efficiency |
| Product Displays | $750 | 30 | +10% impulse purchases |
| Seasonal Decorations | $300 | 15 | +15% seasonal sales |
| Premium Lighting | $2,000 | 50 | +10% product appeal |
| Music System | $1,000 | 40 | +5% customer dwell time |

---

# Business 2: Retail Fashion Store

## Overview

A fashion retail business where players manage clothing inventory, visual merchandising, and provide personalized customer service across multiple product lines.

---

## Core Loop

```
FASHION STORE DAILY CYCLE

MORNING PREP (9 AM - 10 AM)
├── Visual merchandising updates
├── New arrival placement
├── Mannequin styling
└── Mini-games: Outfit Assembly, Display Design

MORNING (10 AM - 1 PM)
├── Customer assistance
├── Fitting room management
├── Sales transactions
└── Mini-games: Style Matching, Size Finding

AFTERNOON (1 PM - 5 PM)
├── Inventory restocking
├── Online order fulfillment
├── Staff training sessions
└── Mini-games: Inventory Organization, Order Picking

EVENING (5 PM - 9 PM)
├── Peak shopping hours
├── Personal styling appointments
├── Closing sales push
└── Mini-games: Closing Sales Dialogue, Outfit Completion

CLOSE (9 PM)
├── Register reconciliation
├── Next-day display prep
├── Cleaning and organizing
└── Sales analysis
```

---

## Progression Tiers

### Tier 1: Boutique Corner (Starting)
- **Floor Space**: 400 sq ft
- **Categories**: 1 (Casual wear)
- **Max Staff**: 0 (player only)
- **Inventory**: 100 items
- **Daily Customers**: 20
- **Fitting Rooms**: 1

### Tier 2: Fashion Boutique ($4,000 + 75 BP)
- **Floor Space**: 1,000 sq ft
- **Categories**: 2 (+ Accessories)
- **Max Staff**: 2
- **Inventory**: 300 items
- **Daily Customers**: 50
- **Fitting Rooms**: 2

### Tier 3: Style Studio ($12,000 + 200 BP)
- **Floor Space**: 2,500 sq ft
- **Categories**: 4 (+ Formal, Athletic)
- **Max Staff**: 5
- **Inventory**: 750 items
- **Daily Customers**: 120
- **Fitting Rooms**: 4

### Tier 4: Fashion House ($35,000 + 400 BP)
- **Floor Space**: 5,000 sq ft
- **Categories**: 6 (+ Designer, Seasonal)
- **Max Staff**: 12
- **Inventory**: 2,000 items
- **Daily Customers**: 300
- **Fitting Rooms**: 8 + VIP Suite

### Tier 5: Fashion Empire ($100,000 + 800 BP)
- **Floor Space**: 10,000+ sq ft
- **Categories**: All + Exclusive lines
- **Max Staff**: 30
- **Inventory**: 5,000+ items
- **Daily Customers**: 750
- **Fitting Rooms**: 15 + Personal shopping

---

## Staff Roles

### Sales Associate
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Charisma > Speed > Accuracy | $12/hr | Customer assistance, checkout, fitting room |

### Visual Merchandiser
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Accuracy > Creativity > Speed | $14/hr | Display design, mannequins, store layout |

### Personal Stylist
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Charisma > Accuracy > Speed | $16/hr | Personal consultations, wardrobe building |

### Inventory Specialist
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Accuracy > Speed > Maintenance | $13/hr | Stock management, receiving, organization |

### Store Manager
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Charisma > Accuracy > Speed | $20/hr | Staff supervision, sales targets, operations |

---

## Mini-Games

### Style Matching Mini-Game
**Mechanic**: Puzzle/Matching
- Customer describes desired look
- Select items that match style, color, occasion
- Score based on customer satisfaction
- Bonus for complete outfit suggestions

### Outfit Assembly Mini-Game
**Mechanic**: Drag-and-Drop Creativity
- Build mannequin outfits
- Match seasonal themes
- Color coordination scoring
- Time-limited challenges

### Fitting Room Management
**Mechanic**: Queue/Resource Management
- Assign rooms to customers
- Track items going in/out
- Prevent fitting room overflow
- Handle returns to floor

### Register Speed Mini-Game
**Mechanic**: Reaction/Memory
- Scan items quickly
- Apply correct discounts
- Process gift cards
- Bag appropriately

---

## Mission Chain

### Starter Mission: "Style Foundations"
**Objective**: Set up and sell your first collection
**Steps**:
1. Arrange initial inventory display
2. Style first mannequin
3. Assist 5 customers
4. Make 3 sales
5. Close store successfully
**Rewards**: $150, 40 XP, 10 BP

### Sample Missions

#### Mission: "Accessory Expansion"
**Objective**: Add accessories department
**Steps**:
1. Purchase display cases ($1,500)
2. Order accessory inventory
3. Create accessory display
4. Sell 25 accessories
**Rewards**: $400, 100 XP, 25 BP, Accessories Unlocked

#### Mission: "Personal Touch"
**Objective**: Launch personal styling service
**Steps**:
1. Complete style consultation training
2. Set up VIP fitting room ($3,000)
3. Complete 5 personal styling sessions
4. Achieve 4.5+ rating on styling
**Rewards**: $750, 150 XP, 40 BP, Personal Stylist role unlocked

#### Mission: "Fashion Week"
**Objective**: Host in-store fashion event
**Steps**:
1. Plan event (select date, theme)
2. Prepare exclusive collection
3. Invite VIP customers
4. Successfully execute runway show
5. Achieve $5,000 event sales
**Rewards**: $2,000, 300 XP, 75 BP, "Fashion Forward" badge

---

# Business 3: Restaurant

## Overview

A full-service or fast-food restaurant where players manage cooking, serving, and customer satisfaction across multiple stations.

---

## Core Loop

```
RESTAURANT DAILY CYCLE

PREP (5 AM - 10 AM)
├── Ingredient prep
├── Station setup
├── Staff briefing
└── Mini-games: Chopping Rhythm, Station Setup

BREAKFAST SERVICE (6 AM - 11 AM) [If enabled]
├── Breakfast orders
├── Quick-service focus
└── Mini-games: Order Assembly, Coffee Speed

LUNCH RUSH (11 AM - 2 PM)
├── Peak order volume
├── Kitchen coordination
├── Table management
└── Mini-games: Cooking Timing, Order Accuracy

AFTERNOON (2 PM - 5 PM)
├── Prep for dinner
├── Menu adjustments
├── Staff rotation
└── Mini-games: Inventory Check, Menu Planning

DINNER SERVICE (5 PM - 10 PM)
├── Full-service operations
├── Reservation management
├── Peak revenue period
└── Mini-games: Multi-order Cooking, Wine Pairing

CLOSE (10 PM - 12 AM)
├── Kitchen deep clean
├── Inventory count
├── Register reconciliation
└── Next-day prep
```

---

## Progression Tiers

### Tier 1: Food Cart (Starting)
- **Seating**: 0 (Takeout only)
- **Menu Items**: 5
- **Max Staff**: 0 (player only)
- **Kitchen Stations**: 1
- **Daily Covers**: 30
- **Equipment**: Basic grill, prep area

### Tier 2: Diner ($6,000 + 100 BP)
- **Seating**: 20
- **Menu Items**: 15
- **Max Staff**: 3
- **Kitchen Stations**: 2
- **Daily Covers**: 80
- **Equipment**: Full grill, deep fryer

### Tier 3: Family Restaurant ($20,000 + 250 BP)
- **Seating**: 50
- **Menu Items**: 35
- **Max Staff**: 8
- **Kitchen Stations**: 4
- **Daily Covers**: 200
- **Equipment**: Full kitchen, bar area

### Tier 4: Fine Dining ($60,000 + 500 BP)
- **Seating**: 80
- **Menu Items**: 50 (with specials)
- **Max Staff**: 20
- **Kitchen Stations**: 6
- **Daily Covers**: 150 (higher quality)
- **Equipment**: Professional kitchen, wine cellar

### Tier 5: Restaurant Group ($200,000 + 1,000 BP)
- **Seating**: 150+
- **Menu Items**: 75+
- **Max Staff**: 50
- **Kitchen Stations**: 10
- **Daily Covers**: 400
- **Equipment**: Full professional setup

---

## Staff Roles

### Line Cook
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Speed > Accuracy > Maintenance | $14/hr | Station cooking, prep, plating |

### Server
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Charisma > Speed > Accuracy | $8/hr + tips | Order taking, delivery, table service |

### Host/Hostess
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Charisma > Accuracy > Speed | $12/hr | Seating, reservations, customer greeting |

### Sous Chef
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Accuracy > Speed > Maintenance | $20/hr | Kitchen management, quality control |

### Dishwasher
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Speed > Stamina > Maintenance | $11/hr | Dish cleaning, station support |

### Restaurant Manager
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Charisma > Accuracy > Speed | $25/hr | Overall operations, staff, customers |

---

## Mini-Games

### Cooking Timing Mini-Game
**Mechanic**: Rhythm/Timing
- Multiple items on stovetop
- Flip/remove at perfect moment
- Visual/audio cues for doneness
- Burned = wasted ingredients + reputation hit

### Order Assembly Mini-Game
**Mechanic**: Memory/Matching
- View order ticket
- Select correct items
- Plate in correct arrangement
- Speed bonus for accuracy

### Table Service Mini-Game
**Mechanic**: Route Optimization
- Plan efficient table route
- Balance multiple table needs
- Respond to customer signals
- Maximize tip potential

### Ingredient Prep Mini-Game
**Mechanic**: Rhythm/Precision
- Chopping to rhythm
- Measuring ingredients
- Prep station organization
- Time pressure with accuracy

---

## Mission Chain

### Starter Mission: "Kitchen Foundations"
**Objective**: Serve your first customers
**Steps**:
1. Prep ingredients (tutorial)
2. Cook first dishes
3. Serve 5 customers
4. Maintain food safety standards
5. Close kitchen
**Rewards**: $200, 50 XP, 10 BP

### Sample Missions

#### Mission: "Dine-In Debut"
**Objective**: Establish sit-down service
**Steps**:
1. Purchase dining tables ($2,000)
2. Hire first server
3. Serve 25 dine-in customers
4. Achieve 3.5+ star rating
**Rewards**: $500, 100 XP, 25 BP, Dine-In Unlocked

#### Mission: "Critics' Choice"
**Objective**: Earn positive food critic review
**Steps**:
1. Receive critic notification
2. Prepare signature dishes
3. Provide exceptional service
4. Score 4+ stars from critic
**Rewards**: $1,500, 250 XP, 60 BP, "Critics' Choice" badge (+20% weekend traffic)

#### Mission: "Catering Expansion"
**Objective**: Launch catering services
**Steps**:
1. Purchase catering equipment ($5,000)
2. Complete catering certification
3. Successfully cater 3 events
4. Achieve 90% satisfaction rate
**Rewards**: $2,500, 350 XP, 80 BP, Catering Service Unlocked

---

# Business 4: Construction/Home Remodeling

## Overview

A construction and remodeling business where players take on projects from simple repairs to full home renovations, managing crews and timelines.

---

## Core Loop

```
CONSTRUCTION DAILY CYCLE

MORNING OFFICE (6 AM - 7 AM)
├── Review day's projects
├── Dispatch crews
├── Client check-ins
└── Material orders

ON-SITE WORK (7 AM - 4 PM)
├── Travel to job sites
├── Perform construction tasks
├── Manage subcontractors
├── Client walkthrough
└── Mini-games: Measuring, Cutting, Installing

AFTERNOON OFFICE (4 PM - 6 PM)
├── Site reports
├── Invoice processing
├── Estimate creation
├── Material pickup
└── Mini-games: Estimate Calculator, Schedule Planning

EVENING (6 PM - 8 PM)
├── Client consultations
├── Bid submissions
├── Next-day planning
└── Equipment maintenance
```

---

## Progression Tiers

### Tier 1: Handyman (Starting)
- **Project Types**: Basic repairs
- **Max Concurrent Projects**: 1
- **Max Crew**: 0 (player only)
- **Equipment**: Basic hand tools
- **Project Value Cap**: $500
- **Vehicle**: Personal vehicle

### Tier 2: Small Contractor ($5,000 + 100 BP)
- **Project Types**: + Minor renovations
- **Max Concurrent Projects**: 2
- **Max Crew**: 2
- **Equipment**: Power tools
- **Project Value Cap**: $5,000
- **Vehicle**: Work van

### Tier 3: General Contractor ($25,000 + 300 BP)
- **Project Types**: + Full room remodels
- **Max Concurrent Projects**: 4
- **Max Crew**: 8
- **Equipment**: Full tool collection
- **Project Value Cap**: $50,000
- **Vehicle**: Multiple work vehicles

### Tier 4: Construction Company ($75,000 + 600 BP)
- **Project Types**: + Whole home renovation
- **Max Concurrent Projects**: 8
- **Max Crew**: 25
- **Equipment**: Heavy equipment access
- **Project Value Cap**: $250,000
- **Vehicle**: Fleet management

### Tier 5: Development Firm ($250,000 + 1,200 BP)
- **Project Types**: + New construction
- **Max Concurrent Projects**: 15
- **Max Crew**: 100+
- **Equipment**: Full construction fleet
- **Project Value Cap**: $1,000,000+
- **Vehicle**: Full fleet

---

## Staff Roles

### Carpenter
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Accuracy > Speed > Maintenance | $22/hr | Framing, trim, cabinets, finish work |

### Electrician
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Accuracy > Maintenance > Speed | $28/hr | Wiring, panels, fixtures, code compliance |

### Plumber
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Accuracy > Maintenance > Speed | $26/hr | Pipes, fixtures, water heaters, drainage |

### Painter
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Accuracy > Speed > Maintenance | $18/hr | Prep, painting, finishing, detail work |

### Laborer
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Stamina > Speed > Maintenance | $15/hr | Demo, cleanup, material moving, support |

### Project Manager
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Accuracy > Charisma > Speed | $35/hr | Scheduling, client liaison, quality control |

---

## Mini-Games

### Measuring Mini-Game
**Mechanic**: Precision/Math
- Measure spaces accurately
- Calculate material needs
- Mark cut lines precisely
- Verify measurements against plans

### Cutting Mini-Game
**Mechanic**: Timing/Precision
- Line up material in saw
- Cut at precise moment
- Achieve clean cuts
- Minimize waste

### Installation Mini-Game
**Mechanic**: Puzzle/Assembly
- Follow installation sequence
- Level and align components
- Secure with correct fasteners
- Pass inspection checks

### Safety Check Mini-Game
**Mechanic**: Inspection/Checklist
- Identify safety hazards
- Ensure proper PPE
- Verify work area security
- Report incidents

### Estimate Mini-Game
**Mechanic**: Calculator/Strategy
- Review project scope
- Calculate material costs
- Estimate labor hours
- Set competitive price

---

## Mission Chain

### Starter Mission: "Tool Time"
**Objective**: Complete your first repair job
**Steps**:
1. Accept first job (leaky faucet)
2. Travel to job site
3. Complete repair (plumbing mini-game)
4. Get client sign-off
5. Collect payment
**Rewards**: $100, 40 XP, 10 BP

### Sample Missions

#### Mission: "Power Up"
**Objective**: Get electrician certification
**Steps**:
1. Complete electrical training course ($500)
2. Pass certification exam (mini-game)
3. Complete 3 electrical jobs
4. Pass safety inspection
**Rewards**: $750, 150 XP, 35 BP, Electrical work unlocked

#### Mission: "Kitchen Dreams"
**Objective**: Complete first kitchen remodel
**Steps**:
1. Win kitchen remodel bid
2. Demolition phase
3. Plumbing rough-in
4. Electrical work
5. Cabinet installation
6. Countertop installation
7. Final inspection
**Rewards**: $5,000, 400 XP, 100 BP, Kitchen Specialist badge

#### Mission: "Builder's License"
**Objective**: Obtain general contractor license
**Steps**:
1. Complete required training hours (40 in-game days)
2. Pass contractor exam
3. Post license bond ($10,000)
4. Complete 10 projects successfully
**Rewards**: $3,000, 500 XP, 125 BP, General Contractor License

---

# Business 5: Uber/Taxi Company

## Overview

A ride-sharing and taxi business where players start as drivers and grow to manage a fleet of vehicles and drivers.

---

## Core Loop

```
RIDE SERVICE DAILY CYCLE

MORNING RUSH (5 AM - 9 AM)
├── Airport runs
├── Commuter pickups
├── Peak pricing active
└── Mini-games: Navigation, Time Management

MIDDAY (9 AM - 4 PM)
├── Steady ride flow
├── Airport/hotel routes
├── Vehicle maintenance window
└── Mini-games: Fuel Management, Route Optimization

EVENING RUSH (4 PM - 8 PM)
├── Commuter returns
├── Dinner/entertainment runs
├── Peak pricing returns
└── Mini-games: Multi-stop Routes, Customer Service

NIGHT (8 PM - 2 AM)
├── Entertainment district
├── Bar/club pickups
├── Premium night rates
└── Mini-games: Safe Driving, Difficult Customer

LATE NIGHT/DAWN (2 AM - 5 AM)
├── Airport early flights
├── Shift workers
├── Low volume, premium rates
└── Vehicle inspection/cleaning
```

---

## Progression Tiers

### Tier 1: Solo Driver (Starting)
- **Vehicle**: Personal car
- **Fleet Size**: 1
- **Max Drivers**: 0 (player only)
- **Daily Rides**: 15-20
- **Service Area**: Single district
- **Features**: Basic navigation

### Tier 2: Premium Driver ($3,000 + 75 BP)
- **Vehicle**: Upgraded personal vehicle
- **Fleet Size**: 1
- **Max Drivers**: 0
- **Daily Rides**: 25-30
- **Service Area**: 2 districts
- **Features**: + Premium rides, better GPS

### Tier 3: Small Fleet ($15,000 + 200 BP)
- **Vehicle**: 3 vehicles
- **Fleet Size**: 3
- **Max Drivers**: 2
- **Daily Rides**: 60-80
- **Service Area**: City-wide
- **Features**: + Dispatch system

### Tier 4: Taxi Company ($50,000 + 400 BP)
- **Vehicle**: 10 vehicles
- **Fleet Size**: 10
- **Max Drivers**: 9
- **Daily Rides**: 200+
- **Service Area**: City + suburbs
- **Features**: + Corporate accounts, airport contracts

### Tier 5: Transportation Empire ($150,000 + 800 BP)
- **Vehicle**: 30+ vehicles
- **Fleet Size**: 30+
- **Max Drivers**: 29+
- **Daily Rides**: 500+
- **Service Area**: Regional
- **Features**: + Luxury vehicles, private contracts

---

## Staff Roles

### Driver
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Speed > Charisma > Maintenance | $15/hr + tips | Driving, customer service, vehicle care |

### Dispatcher
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Speed > Accuracy > Charisma | $16/hr | Ride assignments, routing, driver support |

### Fleet Mechanic
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Maintenance > Accuracy > Speed | $22/hr | Vehicle repairs, maintenance, inspections |

### Fleet Manager
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Accuracy > Charisma > Speed | $25/hr | Vehicle scheduling, driver management, compliance |

### Customer Service
| Attribute Priority | Base Wage | Tasks |
|-------------------|-----------|-------|
| Charisma > Accuracy > Speed | $14/hr | Complaints, lost items, bookings |

---

## Mini-Games

### Navigation Mini-Game
**Mechanic**: Route Selection
- View map with multiple route options
- Consider traffic, time, fuel
- Select optimal path
- Real-time adjustments for obstacles

### Driving Mini-Game
**Mechanic**: Skill/Timing
- Smooth acceleration/braking
- Lane management
- Traffic anticipation
- Customer comfort meter

### Customer Service Mini-Game
**Mechanic**: Dialogue/Choice
- Respond to passenger requests
- Handle difficult customers
- Conversation for tips
- Maintain professional demeanor

### Vehicle Maintenance Mini-Game
**Mechanic**: Inspection/Action
- Identify maintenance needs
- Perform basic checks (oil, tires, fluids)
- Schedule major repairs
- Prevent breakdown events

### Dispatch Optimization Mini-Game (Fleet)
**Mechanic**: Strategy/Logistics
- Assign drivers to rides
- Minimize wait times
- Balance driver workloads
- Maximize coverage efficiency

---

## Mission Chain

### Starter Mission: "First Fare"
**Objective**: Complete your first ride
**Steps**:
1. Accept ride request
2. Navigate to pickup location
3. Pick up passenger
4. Navigate to destination (mini-game)
5. Drop off and collect fare
**Rewards**: $50, 30 XP, 10 BP

### Sample Missions

#### Mission: "Five-Star Driver"
**Objective**: Achieve 5-star driver rating
**Steps**:
1. Complete 20 rides
2. Maintain 4.8+ average rating
3. Zero cancellations
4. Zero safety incidents
**Rewards**: $300, 100 XP, 25 BP, Premium Rides Unlocked

#### Mission: "Airport Contract"
**Objective**: Secure airport pickup rights
**Steps**:
1. Apply for airport permit ($1,000)
2. Pass background check
3. Complete airport training
4. Complete 10 airport rides without incident
**Rewards**: $500, 200 XP, 50 BP, Airport Access Badge

#### Mission: "Fleet Starter"
**Objective**: Expand to multiple vehicles
**Steps**:
1. Purchase second vehicle ($8,000)
2. Hire first driver
3. Set up dispatch system
4. Complete 50 rides with both vehicles
**Rewards**: $1,500, 300 XP, 75 BP, Fleet Management Unlocked

---

# Technical Specifications

## Save/Load JSON Structure

```json
{
  "saveVersion": "1.0",
  "timestamp": "2024-01-15T14:30:00Z",
  "playerData": {
    "name": "Player Name",
    "money": 15000,
    "businessPoints": 250,
    "experience": 5000,
    "level": 12
  },
  "businesses": [
    {
      "businessId": "hypermarket_001",
      "businessType": "Hypermarket",
      "tier": 2,
      "name": "Player's Market",
      "reputation": 3.8,
      "totalReputation": 3.8,
      "dailyStats": {
        "customersServed": 45,
        "revenue": 1250,
        "expenses": 400,
        "profit": 850
      },
      "inventory": {
        "products": [
          { "productId": "apple_001", "quantity": 50, "purchasePrice": 0.50 }
        ],
        "totalValue": 2500
      },
      "staff": [
        {
          "staffId": "staff_001",
          "roleId": "cashier",
          "name": "John Smith",
          "attributes": {
            "speed": 6,
            "accuracy": 7,
            "charisma": 5,
            "maintenance": 4,
            "stamina": 6,
            "loyalty": 7
          },
          "experience": 150,
          "level": 2,
          "skills": ["basic_training", "customer_service_101"],
          "schedule": {
            "monday": { "shift": "morning", "hours": 8 },
            "tuesday": { "shift": "morning", "hours": 8 }
          },
          "hourlyWage": 12,
          "morale": 80
        }
      ],
      "equipment": [
        {
          "equipmentId": "register_001",
          "type": "basic_register",
          "condition": 85,
          "lastMaintenance": "2024-01-10"
        }
      ],
      "unlocks": {
        "departments": ["grocery", "fresh_produce"],
        "equipment": ["basic_register", "express_register", "refrigeration"],
        "decorations": ["basic_signage", "floor_graphics"],
        "services": []
      },
      "missions": {
        "completed": ["first_day", "fresh_start"],
        "active": ["community_pillar"],
        "available": ["supply_chain_master"]
      },
      "events": {
        "active": [
          {
            "eventId": "supply_delay_001",
            "type": "supply_delay",
            "affectedCategory": "dairy",
            "startTime": "2024-01-14T08:00:00Z",
            "endTime": "2024-01-16T08:00:00Z"
          }
        ],
        "history": []
      }
    }
  ],
  "globalStats": {
    "totalRevenue": 125000,
    "totalProfit": 45000,
    "daysPlayed": 30,
    "missionsCompleted": 15,
    "eventsHandled": 8
  },
  "settings": {
    "difficulty": "normal",
    "autoSave": true,
    "notifications": true
  }
}
```

---

# UI Wireframes

## Business Dashboard (Text Wireframe)

```
╔══════════════════════════════════════════════════════════════════════╗
║  BUSINESS DASHBOARD - [Business Name]                    [X] Close   ║
╠══════════════════════════════════════════════════════════════════════╣
║                                                                      ║
║  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐      ║
║  │ TODAY'S STATS   │  │ REPUTATION      │  │ BUSINESS POINTS │      ║
║  │ Revenue: $1,250 │  │ ★★★★☆ 3.8       │  │ BP: 250         │      ║
║  │ Expenses: $400  │  │ ▲ +0.2 this week│  │ Next: Floor Exp │      ║
║  │ Profit: $850    │  │                 │  │ Need: 50 more   │      ║
║  │ Customers: 45   │  │ [View Details]  │  │ [Shop]          │      ║
║  └─────────────────┘  └─────────────────┘  └─────────────────┘      ║
║                                                                      ║
║  ┌────────────────────────────────────────────────────────────────┐ ║
║  │ TASK QUEUE                                           [+ Add]   │ ║
║  │ ──────────────────────────────────────────────────────────────│ ║
║  │ ● [URGENT] Restock Dairy Section          [Assign] [Do It]    │ ║
║  │ ● [HIGH] Process Delivery - Vendor ABC    [Assign] [Do It]    │ ║
║  │ ○ [NORMAL] Clean Floor - Aisle 3          [Assign] [Do It]    │ ║
║  │ ○ [LOW] Update Price Tags                 [Assign] [Do It]    │ ║
║  └────────────────────────────────────────────────────────────────┘ ║
║                                                                      ║
║  ┌─────────────────────────┐  ┌─────────────────────────────────┐  ║
║  │ ACTIVE EVENTS           │  │ STAFF ON DUTY                   │  ║
║  │ ⚠ Supply Delay (Dairy)  │  │ ◉ John (Cashier) - 4hrs left   │  ║
║  │   Ends in: 1d 4h        │  │ ◉ Sarah (Stocker) - 6hrs left  │  ║
║  │                         │  │ ○ Mike (Off-duty)               │  ║
║  │ ℹ Inspection in 5 days  │  │ [Manage Staff] [Schedule]       │  ║
║  └─────────────────────────┘  └─────────────────────────────────┘  ║
║                                                                      ║
║  [Inventory] [Hiring] [Upgrades] [Missions] [Reports] [Settings]    ║
║                                                                      ║
╚══════════════════════════════════════════════════════════════════════╝
```

## Task Queue Panel

```
╔════════════════════════════════════════════════════════════╗
║  TASK QUEUE                                    [Filter ▼]  ║
╠════════════════════════════════════════════════════════════╣
║                                                            ║
║  ┌──────────────────────────────────────────────────────┐ ║
║  │ ● URGENT - Restock Dairy Section                     │ ║
║  │   Est. Time: 15 min | XP: 20 | Deadline: 30 min      │ ║
║  │   [Assign to: ▼ John    ] [Do It Yourself] [Details] │ ║
║  └──────────────────────────────────────────────────────┘ ║
║                                                            ║
║  ┌──────────────────────────────────────────────────────┐ ║
║  │ ● HIGH - Process Incoming Delivery                   │ ║
║  │   Est. Time: 30 min | XP: 35 | Deadline: 1 hour      │ ║
║  │   [Assign to: ▼ Sarah   ] [Do It Yourself] [Details] │ ║
║  └──────────────────────────────────────────────────────┘ ║
║                                                            ║
║  Completed Today: 12/15 | Staff Efficiency: 85%           ║
║                                                            ║
╚════════════════════════════════════════════════════════════╝
```

## Hiring Panel

```
╔════════════════════════════════════════════════════════════╗
║  HIRING CENTER                            Slots: 2/4 Used  ║
╠════════════════════════════════════════════════════════════╣
║                                                            ║
║  AVAILABLE APPLICANTS                                      ║
║  ┌──────────────────────────────────────────────────────┐ ║
║  │ ► MARIA GONZALES                          [HIRE $12] │ ║
║  │   Role: Cashier | Experience: 2 years                │ ║
║  │   ┌──────────────────────────────────────────────┐   │ ║
║  │   │ Speed: ████████░░ 8  Charisma: ██████░░░░ 6  │   │ ║
║  │   │ Accuracy: ███████░░░ 7  Loyalty: █████░░░░░ 5│   │ ║
║  │   └──────────────────────────────────────────────┘   │ ║
║  └──────────────────────────────────────────────────────┘ ║
║                                                            ║
║  ┌──────────────────────────────────────────────────────┐ ║
║  │ ► JAMES WILSON                            [HIRE $11] │ ║
║  │   Role: Stocker | Experience: 6 months               │ ║
║  │   ┌──────────────────────────────────────────────┐   │ ║
║  │   │ Speed: █████░░░░░ 5  Maintenance: ████░░░░░░ 4│  │ ║
║  │   │ Stamina: ███████░░░ 7  Loyalty: ██████░░░░ 6 │   │ ║
║  │   └──────────────────────────────────────────────┘   │ ║
║  └──────────────────────────────────────────────────────┘ ║
║                                                            ║
║  [Post Job Listing: $50] [View Current Staff] [Schedules] ║
║                                                            ║
╚════════════════════════════════════════════════════════════╝
```

## Inventory Panel

```
╔════════════════════════════════════════════════════════════╗
║  INVENTORY MANAGEMENT                    Total: $12,450    ║
╠════════════════════════════════════════════════════════════╣
║                                                            ║
║  Categories: [All] [Grocery] [Produce] [Dairy] [Bakery]   ║
║                                                            ║
║  ┌──────────────────────────────────────────────────────┐ ║
║  │ ITEM          │ QTY  │ PRICE │ STOCK │ ACTION        │ ║
║  │───────────────┼──────┼───────┼───────┼───────────────│ ║
║  │ Apples        │  45  │ $1.29 │ ████░ │ [Order]       │ ║
║  │ Bread         │  12  │ $2.49 │ ██░░░ │ [Order] ⚠     │ ║
║  │ Milk (Gal)    │   8  │ $3.99 │ █░░░░ │ [Order] ⚠     │ ║
║  │ Eggs (Dozen)  │  30  │ $4.29 │ ███░░ │ [Order]       │ ║
║  │ Cheese        │  22  │ $5.99 │ ███░░ │ [Order]       │ ║
║  └──────────────────────────────────────────────────────┘ ║
║                                                            ║
║  Low Stock Alerts: 2 | Pending Orders: 1                   ║
║  [Quick Reorder] [Supplier Contacts] [Order History]      ║
║                                                            ║
╚════════════════════════════════════════════════════════════╝
```

## Schedule Editor

```
╔════════════════════════════════════════════════════════════╗
║  SHIFT SCHEDULE - Week of Jan 15                          ║
╠════════════════════════════════════════════════════════════╣
║                                                            ║
║  TIME  │ MON │ TUE │ WED │ THU │ FRI │ SAT │ SUN │        ║
║  ──────┼─────┼─────┼─────┼─────┼─────┼─────┼─────│        ║
║  6-2PM │ YOU │ YOU │ YOU │ YOU │ YOU │ --- │ --- │        ║
║        │John │John │ --- │John │John │John │John │        ║
║  ──────┼─────┼─────┼─────┼─────┼─────┼─────┼─────│        ║
║  2-10PM│Sarah│Sarah│Sarah│Sarah│ --- │Sarah│ --- │        ║
║        │ --- │Mike │Mike │Mike │Mike │Mike │Mike │        ║
║                                                            ║
║  Coverage Score: 92% | Labor Cost: $1,240/week            ║
║  ⚠ Gap: Wednesday 6AM-2PM (1 staff short)                 ║
║                                                            ║
║  [Auto-Schedule] [Copy Last Week] [Save] [Publish]        ║
║                                                            ║
╚════════════════════════════════════════════════════════════╝
```

## Mission Log

```
╔════════════════════════════════════════════════════════════╗
║  MISSION LOG                              Progress: 65%    ║
╠════════════════════════════════════════════════════════════╣
║                                                            ║
║  ACTIVE MISSIONS                                           ║
║  ┌──────────────────────────────────────────────────────┐ ║
║  │ ► COMMUNITY PILLAR                     [Track]       │ ║
║  │   Become a community partner                         │ ║
║  │   ┌────────────────────────────────────────────────┐ │ ║
║  │   │ ☑ Maintain 4★ rating for 7 days (5/7)         │ │ ║
║  │   │ ☐ Host community event                         │ │ ║
║  │   │ ☑ Hire 3 local employees (3/3)                │ │ ║
║  │   │ ☐ Donate $1,000 ($400/$1,000)                 │ │ ║
║  │   └────────────────────────────────────────────────┘ │ ║
║  │   Rewards: $1,000 | 200 XP | 50 BP                   │ ║
║  └──────────────────────────────────────────────────────┘ ║
║                                                            ║
║  AVAILABLE MISSIONS                                        ║
║  ● Supply Chain Master (Tier 4 required)                  ║
║  ● Grand Opening Event (Available now)                    ║
║                                                            ║
║  COMPLETED (12)                           [View All]      ║
║                                                            ║
╚════════════════════════════════════════════════════════════╝
```

---

## Document Revision History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2024-01-15 | Lead Designer | Initial GDD creation |

---

*This document is confidential and intended for internal studio use only.*
