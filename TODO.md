# TODO List for Jacameno: Adult Life Prototype

## 1. Project Structure Setup
- [x] Create Assets/Scripts subfolders: Player, World, UI, Systems, NPCs, MiniGames, Data

## 2. Player Scripts
- [x] PlayerController.cs (third-person movement, sprint, jump)
- [x] PlayerStats.cs (hunger, energy, money, stress, experience)
- [x] PlayerSkills.cs (cooking, work, fitness, charm leveling)
- [x] PlayerInteraction.cs (raycast/proximity triggers)

## 3. World Scripts
- [ ] DayNightCycle.cs (lighting, skybox, time passage)
- [ ] TimeManager.cs (game time, even during mini-games)

## 4. UI Scripts
- [x] HUDManager.cs (clock, hunger, energy, money)
- [x] InventoryUI.cs (inventory menu)
- [x] PhoneUI.cs (quests, apps, messages)
- [x] ShopUI.cs (shops interface)
- [x] JobResultsUI.cs (job results screen)

## 5. Systems Scripts
- [x] NeedsManager.cs (stat decay)
- [x] MoneyManager.cs
- [x] BillsManager.cs (rent, utilities)
- [x] QuestManager.cs (daily/weekly tasks)
- [x] DialogueManager.cs
- [x] SaveManager.cs (JSON save/load)

## 6. NPC Scripts
- [x] NPCController.cs (behavior states, navmesh)
- [x] NPCDialogue.cs (dialogue trigger)

## 7. Mini-Games Scripts
- [x] CashierMiniGame.cs (scanning items)
- [x] DeliveryMiniGame.cs (drive to markers)
- [x] WarehouseMiniGame.cs (timed sorting)

## 8. Data Classes
- [x] Item.cs, Quest.cs, NPCData.cs, etc.

## 9. Scene and Prefab Setup
- [x] Document scene structure
- [x] List prefabs
- [x] UI hierarchy
- [x] Example JSON save file

## 10. Integration and Testing
- [x] Assign scripts to objects
- [x] Test compilation
- [x] Verify game loop
