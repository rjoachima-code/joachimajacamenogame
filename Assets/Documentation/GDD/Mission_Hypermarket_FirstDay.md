# Hypermarket Starter Mission: "First Day on the Job"

## Mission Overview

| Property | Value |
|----------|-------|
| Mission ID | `mission_hypermarket_first_day` |
| Business Type | Hypermarket |
| Required Tier | 1 |
| Prerequisites | None (Starter Mission) |
| Estimated Duration | 20-30 minutes |
| Rewards | $200, 50 XP, 10 BP |

---

## Narrative Introduction

### Scene: Early Morning - Your New Corner Store

*The sun rises over the city as you stand in front of your newly acquired corner store. The previous owner left you the keys last night, and today marks the beginning of your journey as a business owner. The store is modest - a few aisles of basic groceries, a single checkout counter, and a small stockroom in the back. But everyone has to start somewhere.*

**Tutorial Pop-up:**
> "Welcome to your first day as a business owner! Today you'll learn the basics of running your hypermarket. Complete each task to open your store and serve your first customers."

---

## Mission Flow

### Phase 1: Morning Preparation (6:00 AM - 8:00 AM)

#### Objective 1.1: Receive the Morning Delivery

**Narrative:**
*A delivery truck pulls up outside. Your first shipment has arrived - boxes of essential goods to stock your shelves. Time to learn how receiving works.*

**Game Steps:**
1. Walk to the **delivery dock** (rear of store)
2. Interact with the **delivery driver NPC**
3. Dialog: "Morning! Got your order here. 15 boxes of assorted goods. Sign here?"
4. Player selects: [Accept Delivery] / [Check Manifest First]
5. If checking manifest - mini-tutorial shows item verification
6. **MINI-GAME: Delivery Intake**
   - Check 5 boxes against delivery manifest
   - Mark any discrepancies
   - Target: 80% accuracy within 3 minutes
7. Sign digital clipboard
8. Boxes appear in stockroom

**Completion Criteria:**
- Accept delivery
- Score 60%+ on intake mini-game (tutorial difficulty)

**Rewards:**
- 5 XP
- Progress: 1/5 objectives

---

#### Objective 1.2: Stock the Shelves

**Narrative:**
*With the delivery received, it's time to get products on the shelves. Customers won't buy what they can't see!*

**Game Steps:**
1. Enter the **stockroom**
2. Pick up a box of products (interact with box stack)
3. Tutorial prompt: "Carry products to the sales floor and place them on shelves"
4. Walk to appropriate **aisle** based on product type
5. **MINI-GAME: Stocking Efficiency**
   - Tetris-style shelf arrangement
   - Place 10 products in optimal positions
   - Group similar items for bonus
   - Time limit: 5 minutes
6. Complete stocking for at least 3 product types

**Completion Criteria:**
- Stock minimum 15 items across 3 categories
- Score 50%+ on stocking mini-game

**Rewards:**
- 10 XP
- Progress: 2/5 objectives

**Tutorial Tips:**
> "Pro tip: Grouping similar products together makes it easier for customers to find what they need and increases your 'Organization' score!"

---

### Phase 2: Opening the Store (8:00 AM)

#### Objective 1.3: Open for Business

**Narrative:**
*The shelves are stocked, and it's time to welcome your first customers. But first, you need to prepare the checkout area and officially open the doors.*

**Game Steps:**
1. Walk to the **checkout counter**
2. Interact with the **cash register**
3. Tutorial: "Open the register drawer and verify starting cash"
4. Input starting cash amount ($200 starting float)
5. Walk to **front door**
6. Interact with door to unlock
7. Flip **Open/Closed sign** to "Open"
8. Notification: "Your store is now open for business!"

**Completion Criteria:**
- Initialize cash register
- Open front door
- Change sign to Open

**Rewards:**
- 5 XP
- Store officially opens
- Progress: 3/5 objectives

---

### Phase 3: Customer Service (8:00 AM - 12:00 PM)

#### Objective 1.4: Serve 10 Customers

**Narrative:**
*Customers begin trickling in - some early-morning commuters grabbing coffee and snacks, a few parents picking up last-minute groceries. Each transaction teaches you more about the rhythm of retail.*

**Game Steps:**
1. Wait for first customer to approach checkout
2. Tutorial: "Stand behind the register to begin serving customers"
3. Position player behind checkout counter
4. Customer places items on counter
5. **MINI-GAME: Cashier Speed (Repeated for each customer)**
   - Items scroll on conveyor
   - Press [Scan] at correct moment
   - Match item to barcode
   - Calculate change (optional accuracy bonus)
6. Complete transaction
7. Customer rates experience (1-5 stars)
8. Repeat for 10 customers

**Special Customers (Tutorial Variety):**
- Customer 1: Simple 3-item purchase (tutorial)
- Customer 3: Asks for item location (basic customer service)
- Customer 5: Uses coupon (coupon processing tutorial)
- Customer 7: Returns an item (returns tutorial)
- Customer 10: Large purchase (10+ items, speed test)

**Completion Criteria:**
- Serve exactly 10 customers
- Maintain average 3-star satisfaction

**Rewards:**
- 20 XP
- $50-150 based on transaction totals
- Progress: 4/5 objectives

**Customer Dialogue Examples:**

*Customer 1 (Tutorial):*
> "Just these three items today."

*Customer 3:*
> "Excuse me, where can I find the bread?"
> Player: [Point to Aisle 2] / [Escort customer personally]
> (Personal escort gives +satisfaction bonus)

*Customer 7:*
> "I need to return this. It was expired when I bought it yesterday."
> Player: [Process return] / [Argue] / [Offer exchange]
> Tutorial: "Returns are part of business. Always process them politely for better reputation."

---

### Phase 4: Closing (5:00 PM)

#### Objective 1.5: Close Store and Reconcile Register

**Narrative:**
*The afternoon sun begins to set as your first day winds down. The store is quiet now - time to close up properly and see how you did.*

**Game Steps:**
1. Walk to front door
2. Flip sign to "Closed"
3. Lock door
4. Return to checkout counter
5. Open cash register
6. **MINI-GAME: Cash Count**
   - Count bills and coins
   - Compare to system total
   - Identify any discrepancies
   - Target: Within $5 variance
7. Generate end-of-day report
8. Store cash in safe
9. Walk through store for security check
10. Exit through back door

**Completion Criteria:**
- Successfully close store
- Reconcile register within $5
- Complete security check

**Rewards:**
- 10 XP
- Final daily summary displayed

---

## Mission Completion

### Summary Screen

```
╔══════════════════════════════════════════════════════════════╗
║          FIRST DAY ON THE JOB - COMPLETE!                   ║
╠══════════════════════════════════════════════════════════════╣
║                                                              ║
║  "Congratulations! You've successfully completed your       ║
║   first day as a business owner. The road ahead is long,    ║
║   but every empire starts with a single customer."          ║
║                                                              ║
║  ┌────────────────────────────────────────────────────────┐ ║
║  │ DAY 1 STATISTICS                                       │ ║
║  │ ──────────────────────────────────────────────────────│ ║
║  │ Customers Served:     10                               │ ║
║  │ Total Revenue:        $127.45                          │ ║
║  │ Average Satisfaction: 3.8 ★                            │ ║
║  │ Tasks Completed:      5/5                              │ ║
║  │ Register Variance:    -$0.25 (Good!)                   │ ║
║  └────────────────────────────────────────────────────────┘ ║
║                                                              ║
║  REWARDS EARNED:                                             ║
║  ├── $200 Cash Bonus                                        ║
║  ├── 50 Experience Points                                    ║
║  ├── 10 Business Points                                      ║
║  └── Unlocked: "Store Owner" Achievement                     ║
║                                                              ║
║  NEXT MISSION AVAILABLE:                                     ║
║  → "Fresh Start" - Unlock the Fresh Produce department      ║
║                                                              ║
║                    [Continue]                                ║
║                                                              ║
╚══════════════════════════════════════════════════════════════╝
```

### Narrative Outro

*As you lock up for the night, you take one last look at your little store. It's not much yet, but it's yours. Tomorrow brings new opportunities - maybe it's time to think about expanding. You've heard there's good money in fresh produce...*

---

## Technical Implementation Notes

### Mission Data Structure

```csharp
BusinessMission firstDayMission = new BusinessMission
{
    missionId = "mission_hypermarket_first_day",
    title = "First Day on the Job",
    description = "Successfully open and operate your hypermarket for the first time.",
    businessType = BusinessType.Hypermarket,
    requiredTier = 1,
    
    objectives = new List<MissionObjective>
    {
        new MissionObjective
        {
            objectiveId = "obj_receive_delivery",
            description = "Receive the morning delivery",
            type = ObjectiveType.CompleteTask,
            targetProgress = 1
        },
        new MissionObjective
        {
            objectiveId = "obj_stock_shelves",
            description = "Stock shelves with at least 15 items",
            type = ObjectiveType.CompleteTask,
            targetProgress = 15
        },
        new MissionObjective
        {
            objectiveId = "obj_open_store",
            description = "Open your store for business",
            type = ObjectiveType.CompleteTask,
            targetProgress = 1
        },
        new MissionObjective
        {
            objectiveId = "obj_serve_customers",
            description = "Serve 10 customers",
            type = ObjectiveType.ServeCustomers,
            targetProgress = 10
        },
        new MissionObjective
        {
            objectiveId = "obj_close_store",
            description = "Close store and reconcile register",
            type = ObjectiveType.CompleteTask,
            targetProgress = 1
        }
    },
    
    rewards = new MissionRewards
    {
        money = 200f,
        experience = 50,
        businessPoints = 10,
        badge = "Store Owner"
    },
    
    unlocks = new List<string> { "mission_hypermarket_fresh_start" }
};
```

### Trigger Conditions

| Trigger | Condition | Action |
|---------|-----------|--------|
| Mission Start | Player enters store for first time after purchase | Begin Phase 1 |
| Objective Update | Delivery mini-game complete | Mark 1.1 complete |
| Objective Update | 15 items stocked | Mark 1.2 complete |
| Objective Update | Sign flipped to Open | Mark 1.3 complete |
| Objective Update | Customer count reaches 10 | Mark 1.4 complete |
| Objective Update | Register reconciled | Mark 1.5 complete |
| Mission Complete | All 5 objectives complete | Show completion screen |

### NPC Spawn Configuration

| NPC Type | Spawn Rate | Behavior |
|----------|------------|----------|
| Delivery Driver | Once at 6:00 AM | Wait at dock, deliver, leave |
| Tutorial Customers | 1 per 10 min game time | Simple shopping patterns |
| Asking Customer | 30% of customers | Request help finding items |
| Return Customer | 10% of customers | Request return/exchange |

---

## Localization Keys

| Key | English Text |
|-----|--------------|
| MISSION_FIRST_DAY_TITLE | First Day on the Job |
| MISSION_FIRST_DAY_DESC | Successfully open and operate your hypermarket for the first time. |
| OBJ_RECEIVE_DELIVERY | Receive the morning delivery |
| OBJ_STOCK_SHELVES | Stock shelves with at least 15 items |
| OBJ_OPEN_STORE | Open your store for business |
| OBJ_SERVE_CUSTOMERS | Serve 10 customers |
| OBJ_CLOSE_STORE | Close store and reconcile register |
| DIALOG_DELIVERY_GREETING | Morning! Got your order here. 15 boxes of assorted goods. Sign here? |
| TUTORIAL_STOCKING_TIP | Pro tip: Grouping similar products together makes it easier for customers to find what they need! |
| COMPLETION_CONGRATS | Congratulations! You've successfully completed your first day as a business owner. |
