# JACAMENOVILLE - Alpha Test Plan

## Document Overview
**Version:** 1.0  
**Target Build:** Alpha Test  
**Test Lead:** TBD  
**Last Updated:** 2025-11-30

---

## I. Alpha Build Acceptance Criteria

### Critical Success Criteria (Must Pass All)
| ID | Criteria | Priority |
|----|----------|----------|
| AC-001 | Player can launch game without crash | P0 |
| AC-002 | Character creation flow completes successfully | P0 |
| AC-003 | Player spawns at TheGrid station after character creation | P0 |
| AC-004 | JocGuide displays intro message on spawn | P0 |
| AC-005 | Player can navigate to train station UI | P0 |
| AC-006 | Player can select destination district | P0 |
| AC-007 | Train travel completes and player arrives at destination | P0 |
| AC-008 | Game saves player progress | P0 |
| AC-009 | Game loads saved progress correctly | P0 |
| AC-010 | Game runs at 30+ FPS on minimum spec hardware | P0 |

### Secondary Criteria (Should Pass 80%+)
| ID | Criteria | Priority |
|----|----------|----------|
| AC-011 | Day/night cycle visible and transitions | P1 |
| AC-012 | Weather system changes states | P1 |
| AC-013 | NPCs wander without getting stuck | P1 |
| AC-014 | HUD displays time, needs, and money | P1 |
| AC-015 | At least one mini-game is playable | P1 |
| AC-016 | Audio plays without errors | P2 |
| AC-017 | UI elements are readable and interactive | P2 |
| AC-018 | No visual artifacts or z-fighting | P2 |

---

## II. Manual Test Cases (20 Total)

### Smoke Tests (Quick Validation)

#### ST-001: Game Launch
| Field | Value |
|-------|-------|
| **Preconditions** | Fresh install, no save data |
| **Steps** | 1. Launch game executable<br>2. Wait for main menu |
| **Expected** | Main menu loads within 30 seconds, no crashes |
| **Pass Criteria** | Menu visible, all buttons responsive |

#### ST-002: Character Creation Entry
| Field | Value |
|-------|-------|
| **Preconditions** | At main menu |
| **Steps** | 1. Click "New Game"<br>2. Wait for character creation scene |
| **Expected** | Character creation UI visible |
| **Pass Criteria** | UI loads, character model visible |

#### ST-003: Name Input
| Field | Value |
|-------|-------|
| **Preconditions** | In character creation |
| **Steps** | 1. Click name field<br>2. Type "TestPlayer"<br>3. Click outside field |
| **Expected** | Name accepted and displayed |
| **Pass Criteria** | Name shows in field without truncation |

#### ST-004: Character Customization Options
| Field | Value |
|-------|-------|
| **Preconditions** | In character creation |
| **Steps** | 1. Cycle through skin tones<br>2. Cycle through hair styles<br>3. Cycle through outfits |
| **Expected** | Each option visually changes character |
| **Pass Criteria** | All options cycle correctly, character updates |

#### ST-005: Spawn Location
| Field | Value |
|-------|-------|
| **Preconditions** | Completed character creation |
| **Steps** | 1. Click "Start Game"<br>2. Observe spawn location |
| **Expected** | Player spawns at TheGrid station, NOT at home |
| **Pass Criteria** | Spawn point is TheGrid central area |

---

### Core Flow Tests

#### CF-001: JocGuide Introduction
| Field | Value |
|-------|-------|
| **Preconditions** | Just spawned at TheGrid |
| **Steps** | 1. Wait 3 seconds after spawn<br>2. Observe guide message |
| **Expected** | JocGuide welcome message appears |
| **Pass Criteria** | Message UI visible, text readable |

#### CF-002: Train Station Interaction
| Field | Value |
|-------|-------|
| **Preconditions** | At TheGrid |
| **Steps** | 1. Walk to train platform<br>2. Press interact button (E) |
| **Expected** | Train station UI opens |
| **Pass Criteria** | UI displays 5 district destinations |

#### CF-003: District Selection
| Field | Value |
|-------|-------|
| **Preconditions** | Train station UI open |
| **Steps** | 1. Click on "Fame District"<br>2. Click "Confirm" |
| **Expected** | Travel sequence initiates |
| **Pass Criteria** | Loading/travel screen appears |

#### CF-004: Train Travel Completion
| Field | Value |
|-------|-------|
| **Preconditions** | Train travel initiated |
| **Steps** | 1. Wait for travel to complete<br>2. Observe arrival |
| **Expected** | Player appears at Fame district station |
| **Pass Criteria** | Player position changed, correct district loaded |

#### CF-005: Save Game
| Field | Value |
|-------|-------|
| **Preconditions** | Player in any district |
| **Steps** | 1. Open pause menu<br>2. Click "Save Game" |
| **Expected** | Save confirmation appears |
| **Pass Criteria** | save.json file created in persistent data |

#### CF-006: Load Game
| Field | Value |
|-------|-------|
| **Preconditions** | Save file exists |
| **Steps** | 1. Quit to main menu<br>2. Click "Load Game" |
| **Expected** | Game state restored |
| **Pass Criteria** | Player at saved location, stats restored |

---

### System Tests

#### SYS-001: Time Progression
| Field | Value |
|-------|-------|
| **Preconditions** | In game world |
| **Steps** | 1. Note current time<br>2. Wait 60 real seconds<br>3. Check time again |
| **Expected** | In-game time has advanced |
| **Pass Criteria** | HUD clock shows progression |

#### SYS-002: Day/Night Cycle
| Field | Value |
|-------|-------|
| **Preconditions** | In game world |
| **Steps** | 1. Set game time to 5:00 AM<br>2. Fast forward to 8:00 AM<br>3. Observe lighting |
| **Expected** | Lighting transitions from dark to bright |
| **Pass Criteria** | Directional light intensity increases |

#### SYS-003: Weather Change
| Field | Value |
|-------|-------|
| **Preconditions** | In game world |
| **Steps** | 1. Wait for weather check interval<br>2. Observe weather |
| **Expected** | Weather may change (random) |
| **Pass Criteria** | No crashes, effects visible if changed |

#### SYS-004: Needs Decay
| Field | Value |
|-------|-------|
| **Preconditions** | Full hunger/energy |
| **Steps** | 1. Note hunger level<br>2. Wait 5 in-game hours<br>3. Check hunger |
| **Expected** | Hunger has decreased |
| **Pass Criteria** | NeedsManager correctly decays stats |

#### SYS-005: NPC Wandering
| Field | Value |
|-------|-------|
| **Preconditions** | NPCs spawned in district |
| **Steps** | 1. Observe NPC movement for 2 minutes |
| **Expected** | NPCs move around naturally |
| **Pass Criteria** | NPCs don't get stuck, use NavMesh correctly |

---

### UI/UX Tests

#### UI-001: HUD Visibility
| Field | Value |
|-------|-------|
| **Preconditions** | In game world |
| **Steps** | 1. Observe HUD elements |
| **Expected** | All elements visible and readable |
| **Pass Criteria** | Clock, needs bars, money display all present |

#### UI-002: Phone Access
| Field | Value |
|-------|-------|
| **Preconditions** | In game world |
| **Steps** | 1. Press phone key (P)<br>2. Observe phone UI |
| **Expected** | Phone UI opens |
| **Pass Criteria** | Phone panel visible, tabs interactive |

#### UI-003: Inventory Access
| Field | Value |
|-------|-------|
| **Preconditions** | In game world |
| **Steps** | 1. Press inventory key (I)<br>2. Observe inventory UI |
| **Expected** | Inventory UI opens |
| **Pass Criteria** | Inventory panel visible, items displayed |

#### UI-004: Pause Menu
| Field | Value |
|-------|-------|
| **Preconditions** | In game world |
| **Steps** | 1. Press ESC<br>2. Observe pause menu |
| **Expected** | Pause menu opens, game pauses |
| **Pass Criteria** | Menu visible, time stops, buttons work |

---

## III. Automated Unit Tests (5 Planned)

### Test 1: TimeSystem Progression
```csharp
[Test]
public void TimeSystem_TickMinute_AdvancesTimeCorrectly()
{
    // Arrange
    var timeSystem = new GameObject().AddComponent<TimeSystem>();
    int initialHour = timeSystem.Hour;
    int initialMinute = timeSystem.Minute;
    
    // Act - Simulate 60 minute ticks
    for (int i = 0; i < 60; i++)
    {
        // Call private TickMinute via reflection or make internal
    }
    
    // Assert
    Assert.AreEqual(initialHour + 1, timeSystem.Hour);
    Assert.AreEqual(initialMinute, timeSystem.Minute);
}
```

### Test 2: MoneyManager Transactions
```csharp
[Test]
public void MoneyManager_AddMoney_IncreasesBalance()
{
    // Arrange
    var moneyManager = new GameObject().AddComponent<MoneyManager>();
    float initialBalance = moneyManager.balance;
    
    // Act
    moneyManager.AddMoney(100f, "Test");
    
    // Assert
    Assert.AreEqual(initialBalance + 100f, moneyManager.balance);
    Assert.AreEqual(1, moneyManager.transactions.Count);
}

[Test]
public void MoneyManager_Withdraw_DecreasesBalance()
{
    // Arrange
    var moneyManager = new GameObject().AddComponent<MoneyManager>();
    moneyManager.balance = 500f;
    
    // Act
    bool result = moneyManager.Withdraw(200f, "Test");
    
    // Assert
    Assert.IsTrue(result);
    Assert.AreEqual(300f, moneyManager.balance);
}
```

### Test 3: PlayerStats Save/Load
```csharp
[Test]
public void PlayerStats_SaveLoad_PreservesData()
{
    // Arrange
    var playerStats = new GameObject().AddComponent<PlayerStats>();
    playerStats.ModifyHunger(-50f);
    playerStats.ModifyMoney(200);
    
    // Act
    string savedData = playerStats.SaveData();
    playerStats.ModifyHunger(50f); // Change state
    playerStats.LoadData(savedData);
    
    // Assert
    Assert.AreEqual(50f, playerStats.GetHunger());
}
```

### Test 4: DistrictManager Switch
```csharp
[Test]
public void DistrictManager_ChangeDistrict_UpdatesCurrentDistrict()
{
    // Arrange
    var districtManager = new GameObject().AddComponent<DistrictManager>();
    bool eventFired = false;
    districtManager.OnDistrictChanged += (d) => eventFired = true;
    
    // Act
    districtManager.ChangeDistrict(DistrictType.Fame);
    
    // Assert
    Assert.AreEqual(DistrictType.Fame, districtManager.GetCurrentDistrict());
    Assert.IsTrue(eventFired);
}
```

### Test 5: TrainSystem Travel
```csharp
[Test]
public void TrainSystem_TravelToDistrict_InitiatesTravel()
{
    // Arrange
    var trainSystem = new GameObject().AddComponent<TrainSystem>();
    // Setup mock station data
    
    // Act
    bool result = trainSystem.TravelToDistrict(DistrictType.Fame);
    
    // Assert
    Assert.IsTrue(result);
    // Verify travel coroutine started
}
```

---

## IV. Test Environment Requirements

### Minimum Spec (Mobile Equivalent)
- CPU: Intel i3 or equivalent
- RAM: 4GB
- GPU: Integrated graphics (Intel HD 4000+)
- Storage: 2GB free
- OS: Windows 10

### Recommended Spec (PC Target)
- CPU: Intel i5 or equivalent
- RAM: 8GB
- GPU: GTX 1060 or equivalent
- Storage: 4GB free SSD
- OS: Windows 10/11

### Test Data Requirements
- Fresh save data for smoke tests
- Pre-configured save files for load tests
- District JSON configurations verified

---

## V. Bug Severity Definitions

| Severity | Definition | Examples |
|----------|------------|----------|
| **S0 - Critical** | Game unplayable, crash | Crash on launch, save corruption |
| **S1 - Major** | Feature broken, no workaround | Cannot board train, UI doesn't open |
| **S2 - Minor** | Feature impaired, workaround exists | Wrong text displayed, minor visual glitch |
| **S3 - Trivial** | Cosmetic or enhancement | Typo in text, animation jank |

---

## VI. Test Schedule

| Phase | Duration | Focus |
|-------|----------|-------|
| Smoke Testing | Day 1 | ST-001 to ST-005 |
| Core Flow Testing | Day 2-3 | CF-001 to CF-006 |
| System Testing | Day 4-5 | SYS-001 to SYS-005 |
| UI/UX Testing | Day 6 | UI-001 to UI-004 |
| Regression | Day 7 | All previously failed tests |
| Automated Tests | Continuous | Run on every build |

---

## VII. Exit Criteria for Alpha

### Pass Conditions
- All P0 acceptance criteria pass
- No S0 (Critical) bugs open
- No more than 3 S1 (Major) bugs open
- 80%+ of manual test cases pass
- All automated unit tests pass
- Performance target met (30+ FPS)

### Documentation Required
- Test execution report
- Known issues list
- Performance profiling results
- Build notes

---

*Test Plan generated by Technical Director audit system*
