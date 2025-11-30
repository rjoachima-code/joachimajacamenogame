# Jacameno: Adult Life - Unity 3D Platformer Prototype

A 3D Platformer game focused on surviving adult responsibilities through exploration, collection, and mini-games. Experience the challenges of adult life in an engaging 3D world where you manage work, bills, and daily needs.

## Game Description

**Jacameno: Adult Life** is a 3D Platformer that combines life simulation elements with classic platformer mechanics. Navigate through a vibrant world, collect items, complete quests, and manage your character's daily responsibilities including work, bills, and personal needs.

## Controls

| Action | Input |
|--------|-------|
| Move | WASD / Arrow Keys |
| Jump | Space |
| Sprint | Shift |
| Look Around | Mouse |
| Interact | E |
| Inventory | I |
| Phone | P |
| Pause Menu | ESC |

## Installation Instructions

1. Clone this repository:
   ```bash
   git clone https://github.com/rjoachima-code/joachimajacamenogame.git
   ```
2. Open the project in Unity 2022+
3. Ensure URP and New Input System packages are installed
4. Open the MainMenu scene and press Play

## Credits

**Created and Maintained by Joa'Chima Ross Jr**

---

## Engine Requirements
- Unity 2022+
- URP (Universal Render Pipeline)
- New Input System
- C# only

## Project Structure

### Scripts Folder Structure
```
Assets/Scripts/
├── Player/
│   ├── PlayerController.cs
│   ├── PlayerStats.cs
│   ├── PlayerSkills.cs
│   └── PlayerInteraction.cs
├── World/
│   └── DayNightCycle.cs
├── UI/
│   ├── HUDManager.cs
│   ├── InventoryUI.cs
│   ├── PhoneUI.cs
│   ├── ShopUI.cs
│   └── JobResultsUI.cs
├── Systems/
│   ├── TimeManager.cs
│   ├── MoneyManager.cs
│   ├── NeedsManager.cs
│   ├── QuestManager.cs
│   ├── DialogueManager.cs
│   └── SaveManager.cs
├── NPCs/
│   ├── NPCController.cs
│   └── NPCDialogue.cs
├── MiniGames/
│   ├── CashierMiniGame.cs
│   ├── DeliveryMiniGame.cs
│   └── WarehouseMiniGame.cs
├── Data/
│   ├── Quest.cs
│   ├── NPCData.cs
│   └── PlayerSaveData.cs
└── GameManager.cs
```

## Inspector Setup Notes

### GameManager
- Attach to an empty GameObject in the main scene
- No inspector fields required

### TimeManager
- Attach to an empty GameObject
- Configure timeSpeed, dayLengthInMinutes
- Assign event listeners for day/night transitions

### PlayerController
- Attach to Player GameObject with CharacterController
- Assign cameraTransform (Main Camera)
- Configure movement settings (walkSpeed, sprintSpeed, jumpHeight)
- Set up New Input System actions: Move, Look, Sprint, Jump, Interact

### DayNightCycle
- Attach to an empty GameObject
- Assign directionalLight (Directional Light in scene)
- Assign skyboxMaterial (Skybox material)
- Configure lightColorGradient and lightIntensityCurve

### HUDManager
- Attach to HUD Canvas
- Assign UI elements: clockText, hungerBar, energyBar, moneyText

### Other Managers
- All managers are singletons - attach to empty GameObjects
- No specific inspector setup required beyond event assignments

## Scene Structure

### Main Scene Hierarchy
```
Main Scene
├── Managers
│   ├── GameManager
│   ├── TimeManager
│   ├── MoneyManager
│   ├── NeedsManager
│   ├── QuestManager
│   ├── DialogueManager
│   └── SaveManager
├── Player
│   ├── PlayerController
│   ├── PlayerStats
│   ├── PlayerSkills
│   └── PlayerInteraction
├── World
│   ├── Terrain
│   ├── Buildings
│   │   ├── Apartment
│   │   ├── Store
│   │   └── JobLocations
│   └── DayNightCycle
├── NPCs
│   ├── NPC1 (NPCController + NPCDialogue)
│   └── NPC2 (NPCController + NPCDialogue)
├── UI
│   ├── HUD Canvas (HUDManager)
│   ├── Inventory Canvas (InventoryUI)
│   ├── Phone Canvas (PhoneUI)
│   ├── Shop Canvas (ShopUI)
│   └── JobResults Canvas (JobResultsUI)
└── MiniGames
    ├── CashierMiniGame
    ├── DeliveryMiniGame
    └── WarehouseMiniGame
```

## Prefab List

### Core Prefabs
- Player.prefab (CharacterController, PlayerController, etc.)
- NPC.prefab (NPCController, NPCDialogue, NavMeshAgent)
- InteractableObject.prefab (Interactable script)

### UI Prefabs
- HUD.prefab
- InventoryPanel.prefab
- PhonePanel.prefab
- ShopPanel.prefab
- JobResultsPanel.prefab

### Mini-Game Prefabs
- CashierStation.prefab
- DeliveryVehicle.prefab
- Warehouse.prefab

## UI Hierarchy

### HUD Canvas
```
HUD Canvas
├── Clock Text
├── Hunger Bar
│   ├── Fill
│   └── Background
├── Energy Bar
│   ├── Fill
│   └── Background
└── Money Text
```

### Phone Canvas
```
Phone Canvas
├── Phone Panel
│   ├── Quest Tab
│   │   └── Quest Container
│   ├── Apps Tab
│   └── Messages Tab
└── Continue Button
```

### Shop Canvas
```
Shop Canvas
├── Shop Panel
│   ├── Item Container
│   └── Money Text
└── Close Button
```

## Example Quests

```csharp
// Daily Quests
new Quest("daily_eat", "Eat Breakfast", "Start your day with a meal", Quest.QuestType.Daily, 10, 5);
new Quest("daily_work", "Go to Work", "Earn some money", Quest.QuestType.Daily, 50, 10);

// Weekly Quests
new Quest("weekly_pay_bills", "Pay Bills", "Pay rent and utilities", Quest.QuestType.Weekly, 0, 20);
new Quest("weekly_socialize", "Socialize", "Talk to 3 NPCs", Quest.QuestType.Weekly, 25, 15);
```

## Example Items

```csharp
// Food items
"Apple" // Restores 10 hunger
"Bread" // Restores 15 hunger
"Water" // Restores 5 energy

// Other items
"Phone" // Allows access to phone menu
"Wallet" // Stores money
```

## Example NPCs

```csharp
// NPC Data
new NPCData("landlord", "Landlord", new string[] { "Pay your rent!", "Rent is due." }, new string[] { "weekly_pay_bills" });
new NPCData("boss", "Boss", new string[] { "Good work today!", "Keep it up." }, new string[] { "daily_work" });
new NPCData("friend", "Friend", new string[] { "Hey, want to hang out?", "How's life treating you?" }, new string[] { "weekly_socialize" });
```

## Example JSON Save File

```json
{
  "hunger": 85.0,
  "energy": 70.0,
  "money": 150,
  "stress": 20.0,
  "experience": 250,
  "skills": {
    "Cooking": 2,
    "Work": 3,
    "Fitness": 1,
    "Charm": 2
  },
  "inventory": ["Apple", "Phone", "Wallet"],
  "completedQuests": ["daily_eat", "daily_work"],
  "gameTime": 12.5
}
```

## Game Loop

1. **Wake Up**: Player starts in apartment, needs decay over time
2. **Eat**: Interact with fridge/kitchen to restore hunger
3. **Work**: Go to job location, play mini-games to earn money
4. **Pay Bills**: Use phone to pay rent/utilities
5. **Sleep**: Go to bed to restore energy, advance to next day
6. **Repeat**: Daily cycle with weekly objectives

## Mini-Games

### Cashier Mini-Game
- Scan items by pressing correct keys
- Time limit, accuracy affects pay

### Delivery Mini-Game
- Drive to marked locations
- Complete deliveries within time limit

### Warehouse Mini-Game
- Sort items into correct bins
- Timed challenge with increasing difficulty

## Controls

- WASD: Move
- Shift: Sprint
- Space: Jump
- Mouse: Look around
- E: Interact
- I: Inventory
- P: Phone
- ESC: Pause menu

## Build Instructions

1. Open project in Unity 2022+
2. Ensure URP and New Input System packages are installed
3. Set up scene as described above
4. Configure input actions in Project Settings > Input System Package
5. Build for target platform

## Notes

- All scripts use singleton pattern for easy access
- Event system used for loose coupling between systems
- JSON serialization for save/load functionality
- Modular design allows for easy expansion
- Time passes even during mini-games for realism
