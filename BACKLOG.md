# JACAMENOVILLE - Alpha Build Backlog (Roadmap)

## Sprint Overview
**Target:** Functional Alpha Test Build  
**Timeline:** 8 Weeks  
**Team Size Assumption:** 2 Programmers, 1 Artist, 1 Designer

---

## P0 - CRITICAL (Core Flow Must Work)
*These tasks MUST be completed for any playable build*

### P0.1 - Scene Setup & Entry Point
| Task | Owner | Estimate | Dependencies |
|------|-------|----------|--------------|
| Create `MainGame.unity` scene with proper hierarchy | Programmer | 4h | None |
| Set up Manager GameObjects (GameManager, TimeSystem, etc.) | Programmer | 2h | P0.1.1 |
| Configure spawn point at TheGrid | Programmer | 1h | P0.1.2 |
| Add placeholder environment bounds | Artist | 4h | P0.1.1 |
| Configure URP settings for game (not sample) | Programmer | 2h | P0.1.1 |

**Total: ~13 hours (2 days)**

### P0.2 - Character Creation Flow
| Task | Owner | Estimate | Dependencies |
|------|-------|----------|--------------|
| Create `CharacterCreation.unity` scene | Programmer | 2h | None |
| Implement CharacterCreation UI Canvas | Programmer | 8h | P0.2.1 |
| Connect UI to `CharacterCustomization.cs` | Programmer | 4h | P0.2.2 |
| Add placeholder character model | Artist | 8h | None |
| Implement skin tone/hair selection | Programmer | 4h | P0.2.3, P0.2.4 |
| Add name input with validation | Programmer | 2h | P0.2.2 |
| Scene transition to MainGame | Programmer | 2h | P0.1, P0.2.5 |

**Total: ~30 hours (4-5 days)**

### P0.3 - TheGrid Station Implementation
| Task | Owner | Estimate | Dependencies |
|------|-------|----------|--------------|
| Create TheGrid placeholder geometry | Artist | 16h | None |
| Implement 5 train platform markers | Artist | 4h | P0.3.1 |
| Add Information Kiosk interactable | Programmer | 4h | P0.3.1 |
| Add Starter Job Board interactable | Programmer | 4h | P0.3.1 |
| Add Housing Office interactable | Programmer | 4h | P0.3.1 |
| Implement player spawn at Grid on game start | Programmer | 2h | P0.3.1 |
| Wire up `TheGrid.cs` to scene objects | Programmer | 2h | P0.3.3-6 |

**Total: ~36 hours (5 days)**

### P0.4 - Train System Integration
| Task | Owner | Estimate | Dependencies |
|------|-------|----------|--------------|
| Create Train Station UI overlay | Programmer | 8h | P0.3 |
| Implement district selection from UI | Programmer | 4h | P0.4.1 |
| Add travel time loading screen | Programmer | 4h | P0.4.2 |
| Create placeholder train car model | Artist | 8h | None |
| Implement train boarding interaction | Programmer | 4h | P0.4.1 |
| Add train arrival notification | Programmer | 2h | P0.4.3 |

**Total: ~30 hours (4 days)**

### P0.5 - JocGuide Tutorial Integration
| Task | Owner | Estimate | Dependencies |
|------|-------|----------|--------------|
| Create Guide message UI panel | Programmer | 4h | None |
| Implement message queue display | Programmer | 4h | P0.5.1 |
| Add "Take the train" initial prompt | Programmer | 2h | P0.5.2, P0.4 |
| Wire up tutorial triggers | Programmer | 4h | P0.3, P0.4 |

**Total: ~14 hours (2 days)**

### P0.6 - Save/Load Bug Fixes
| Task | Owner | Estimate | Dependencies |
|------|-------|----------|--------------|
| Fix singleton initialization order | Programmer | 4h | None |
| Fix TimeManager → TimeSystem reference | Programmer | 0.5h | None |
| Add null checks to all ISaveable implementations | Programmer | 2h | None |
| Test save/load cycle end-to-end | QA | 2h | P0.6.1-3 |

**Total: ~8.5 hours (1 day)**

---

## P1 - HIGH (Visual Polish & Performance)
*Required for acceptable Alpha quality*

### P1.1 - NFS Visual Implementation (Lighting)
| Task | Owner | Estimate | Dependencies |
|------|-------|----------|--------------|
| Upgrade to URP 14+ for better visuals | Programmer | 4h | None |
| Configure Post-Processing Volume | Programmer | 4h | P1.1.1 |
| Set up Bloom with proper thresholds | Programmer | 2h | P1.1.2 |
| Configure Color Grading (Film style) | Artist | 4h | P1.1.2 |
| Implement HDR lighting setup | Programmer | 4h | P1.1.1 |
| Add reflection probes for glossy surfaces | Programmer | 4h | P1.1.5 |
| Configure ambient occlusion | Programmer | 2h | P1.1.2 |

**Total: ~24 hours (3 days)**

### P1.2 - Performance Foundation (LODs/Culling)
| Task | Owner | Estimate | Dependencies |
|------|-------|----------|--------------|
| Set up LOD Groups for all environment assets | Artist | 16h | Art assets exist |
| Configure Occlusion Culling | Programmer | 4h | P0.3 |
| Implement Addressables for district streaming | Programmer | 16h | None |
| Set up GPU Instancing for repetitive objects | Programmer | 4h | Art assets exist |
| Add mobile-specific quality settings | Programmer | 4h | P1.2.1 |
| Create performance profiling dashboard | Programmer | 8h | None |

**Total: ~52 hours (7 days)**

### P1.3 - District Streaming (GTA V Style)
| Task | Owner | Estimate | Dependencies |
|------|-------|----------|--------------|
| Design district chunk boundaries | Designer | 8h | None |
| Implement async scene loading | Programmer | 16h | P1.2.3 |
| Create district transition triggers | Programmer | 8h | P1.3.2 |
| Add loading screen with progress | Programmer | 4h | P1.3.2 |
| Test memory management during transitions | QA | 4h | P1.3.3 |

**Total: ~40 hours (5 days)**

### P1.4 - Traffic AI Foundation
| Task | Owner | Estimate | Dependencies |
|------|-------|----------|--------------|
| Design traffic node system | Programmer | 8h | None |
| Implement vehicle spawning system | Programmer | 8h | P1.4.1 |
| Create basic vehicle AI (follow path) | Programmer | 16h | P1.4.2 |
| Add traffic light system | Programmer | 8h | P1.4.3 |
| Implement pedestrian crosswalk logic | Programmer | 8h | P1.4.4 |
| Create 3 placeholder vehicle models | Artist | 24h | None |

**Total: ~72 hours (9 days)**

### P1.5 - Pedestrian Navigation
| Task | Owner | Estimate | Dependencies |
|------|-------|----------|--------------|
| Extend NPCController for pedestrian behavior | Programmer | 8h | None |
| Add NavMesh surface per district | Programmer | 8h | P0.3 |
| Implement pedestrian spawn points | Programmer | 4h | P1.5.1 |
| Add pedestrian schedules (day/night) | Programmer | 8h | P1.5.3 |
| Create 5 pedestrian character variants | Artist | 20h | None |

**Total: ~48 hours (6 days)**

---

## P2 - POLISH (UI & Audio)
*Nice-to-have for Alpha, essential for Beta*

### P2.1 - UI System (Sims-Style)
| Task | Owner | Estimate | Dependencies |
|------|-------|----------|--------------|
| Create radial interaction menu | Programmer | 16h | P0.3 |
| Implement needs display (Sims bars) | Programmer | 8h | HUDManager exists |
| Add time/day display widget | Programmer | 4h | P2.1.2 |
| Create phone UI with apps | Programmer | 16h | PhoneUI exists |
| Implement notification system | Programmer | 8h | None |
| Add tutorial tooltip system | Programmer | 8h | JocGuide exists |
| Create inventory UI overhaul | Programmer | 12h | InventoryUI exists |

**Total: ~72 hours (9 days)**

### P2.2 - Audio Implementation
| Task | Owner | Estimate | Dependencies |
|------|-------|----------|--------------|
| Source/create ambient city soundscape | Audio | 20h | None |
| Implement district-specific music | Programmer | 8h | AmbientMusicManager exists |
| Add UI sound effects | Programmer | 4h | P2.1 |
| Implement train sounds | Audio | 8h | P0.4 |
| Add weather audio (rain, wind) | Audio | 12h | WeatherSystem exists |
| Create footstep system | Programmer | 8h | None |
| Implement audio mixing (snapshots) | Programmer | 8h | P2.2.1-6 |

**Total: ~68 hours (8-9 days)**

### P2.3 - Mini-Game Polish
| Task | Owner | Estimate | Dependencies |
|------|-------|----------|--------------|
| Complete CashierMiniGame implementation | Programmer | 16h | Stub exists |
| Complete DeliveryMiniGame implementation | Programmer | 16h | Stub exists |
| Complete WarehouseMiniGame implementation | Programmer | 12h | WarehouseJob exists |
| Add mini-game UI overlays | Programmer | 12h | P2.3.1-3 |
| Create mini-game reward animations | Programmer | 8h | P2.3.4 |

**Total: ~64 hours (8 days)**

---

## Sprint Schedule

### Sprint 1 (Week 1-2): Foundation
- [ ] P0.1 - Scene Setup ✨ Critical Path
- [ ] P0.6 - Bug Fixes
- [ ] P0.2 - Character Creation (start)
- [ ] P0.3 - TheGrid Station (start)

### Sprint 2 (Week 3-4): Core Flow
- [ ] P0.2 - Character Creation (complete)
- [ ] P0.3 - TheGrid Station (complete)
- [ ] P0.4 - Train System
- [ ] P0.5 - JocGuide Tutorial

### Sprint 3 (Week 5-6): Visual & Performance
- [ ] P1.1 - NFS Visual Implementation
- [ ] P1.2 - Performance Foundation
- [ ] P1.4 - Traffic AI (start)

### Sprint 4 (Week 7-8): Polish & Testing
- [ ] P1.4 - Traffic AI (complete)
- [ ] P1.5 - Pedestrian Navigation
- [ ] P2.1 - UI System (critical elements only)
- [ ] Full QA pass
- [ ] Alpha build preparation

---

## Resource Requirements

### Art Assets Needed
| Asset Type | Quantity | Priority |
|------------|----------|----------|
| Character Base Model | 1 | P0 |
| Train Car Model | 1 | P0 |
| TheGrid Station Environment | 1 set | P0 |
| Vehicle Models | 3-5 | P1 |
| Pedestrian Variants | 5-10 | P1 |
| Building Exteriors | 10-20 | P1 |
| UI Icons & Elements | 50+ | P2 |

### Technical Debt to Address
- [ ] Add Assembly Definition files
- [ ] Create unit test project
- [ ] Document all public APIs
- [ ] Standardize coding conventions
- [ ] Set up proper logging system

---

## Success Criteria for Alpha

✅ **Must Have:**
1. Player can create a character and enter the game
2. Player spawns at TheGrid station (not teleported home)
3. Player receives JocGuide notification about the train
4. Player can board train and travel to at least 1 district
5. Game saves and loads without crashing
6. Runs at 30+ FPS on target hardware

⚠️ **Should Have:**
1. Day/night cycle visible
2. Weather effects working
3. At least 5 NPCs wandering
4. Basic HUD displaying needs
5. One functional mini-game

❌ **Nice to Have:**
1. Full district art pass
2. Complete audio implementation
3. All 5 districts accessible
4. Traffic AI with vehicles

---

## Risk Mitigation

| Risk | Mitigation |
|------|------------|
| Art bottleneck | Use Unity Asset Store placeholders, contract external artist |
| Scope creep | Focus ONLY on Fame district for Alpha |
| Integration bugs | Daily builds, automated testing |
| Performance issues | Profile every sprint, set hard budgets |
| Team availability | Document everything, provide cross-training on systems |

---

*Backlog generated by Technical Director audit system*
*Last Updated: 2025-11-30*
