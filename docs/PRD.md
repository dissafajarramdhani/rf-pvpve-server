# RF PvPvE Fair Competitive MMORPG Server — Product Requirements Document

## 1. Document Information
- Version: 0.1
- Status: Draft for development kickoff
- Owner: Project Lead / Product Owner
- Date: 2026-08-22

## 2. Product Summary
This project aims to build an RF-inspired MMORPG server focused on PvE progression, open-world PvP, PvPvE encounters, guild-based social play, and a fair economy. The server is designed to be sustainable and community-driven without relying on pay-to-win mechanics.

The project will prioritize a clean, scalable backend, strong anti-cheat protections, and sustainable monetization through non-power items such as cosmetics, convenience features, and supporter programs.

## 3. Product Vision
To create a competitive RF-style game server where players can:
- level up through PvE progression
- farm gear and materials through dungeons and bosses
- engage in fair PvP and open-field conflicts
- participate in guild wars, raid content, and PvPvE pressure zones
- enjoy a healthy economy without pay-to-win advantages

## 4. Product Mission
- Deliver a stable and fun PvPvE MMORPG experience
- Keep progression fair and transparent
- Encourage social play and guild activity
- Deliver strong live-ops potential without harming balance
- Build a sustainable revenue model based on cosmetics and convenience, not power

## 5. Target Players
### 5.1 Primary Target
- Players who enjoy MMORPG progression, grinding, boss fights, and PvP
- Guild-driven players who want team coordination and ranking systems
- Competitive players who enjoy duel, arena, and high-risk world encounters

### 5.2 Player Segments
- Casual players: leveling, quests, monster farming, resource gathering
- Mid-core players: dungeons, crafting, gear upgrades, guild growth
- Competitive players: arena, field PvP, boss rush, rankings, guild warfare

## 6. Core Game Pillars
1. Progression
   - Levels, job/class growth, skill unlocks, gear upgrading
2. PvE
   - Normal mobs, elite mobs, dungeons, bosses, world events
3. PvP
   - Duel, arena, field combat, faction testing grounds
4. PvPvE
   - danger zones, boss fights with competitive pressure, mixed combat encounters
5. Economy
   - item rarity, crafting, gold flow, market integrity, gold sinks
6. Social systems
   - party, guild, chat, alliance, guild war, territory or state
7. Fairness
   - no pay-to-win, no progression shortcuts, no direct power purchases

## 7. Product Goals
### 7.1 Functional Goals
- Login and account management
- Character creation and selection
- Map and movement system
- Combat system and skill usage
- Inventory and equipment management
- Basic monster AI and quest progression
- Dungeon and boss content
- PvP arena and duel support
- Guild system
- Economy and trading support
- Basic anti-cheat and logs

### 7.2 Non-Functional Goals
- Server uptime of 99%+
- Low latency for local or regional play
- Stable under concurrent player load
- Secure communication and protected admin access
- Data consistency and rollback safety
- Easy scaling as player base grows

## 8. Expected Gameplay Loop
1. Player creates account and character
2. Player completes early quests and learns basic combat
3. Player farms mobs and gains gear and level
4. Player enters dungeons and elite content
5. Player upgrades equipment and builds class identity
6. Player participates in PvP or PvPvE content
7. Player joins guild or raids
8. Player continues to achieve seasonal rewards and high-end progression

## 9. Functional Requirements
### 9.1 Account and Authentication
- Account registration and login
- Password hashing and secure credentials storage
- Session token management
- Character selection and world entry
- Account ban, warn, and restriction management

### 9.2 Character System
- Character creation and deletion
- Class and job selection
- Stats management
- Appearance customization
- Character progression and leveling
- Character save and load

### 9.3 World and Movement
- Open world travel
- Zone loading and transfer
- Map movement synchronization
- Safe zone and danger zone logic
- Region-based object updates
- Player and NPC position updates

### 9.4 Combat System
- Basic attack and skill cast
- Damage calculation server-side
- Buffs and debuffs
- HP/MP and death logic
- Respawn handling
- Combat logs and replay-friendly event trace

### 9.5 Inventory and Equipment
- Item pickup, stacking, and dropping
- Inventory capacity and slots
- Equipment equip/unequip logic
- Item rarity and stat validation
- Sell and trade flow
- Bank or storage support

### 9.6 PvE Content
- Monster spawn system
- Elite mobs and dungeon spawn system
- World boss and raid event logic
- Loot tables and rarity system
- Reward distribution and item logs

### 9.7 PvP Content
- Duel match support
- Arena or battleground support
- Open-world PvP conflict zones
- Honor or ranking logic
- Reward system for competitive play

### 9.8 PvPvE Content
- High-risk zones with mixed mob and player threats
- World boss contested by groups
- PvP pressure during rare mob events
- Reward structure for high-risk exploration

### 9.9 Guild and Social Systems
- Guild creation
- Member roles and permissions
- Guild chat
- Guild storage
- Guild war or territory support
- Party and team formation

### 9.10 Economic System
- Gold and item exchange
- Crafting and upgrade systems
- Market or economy management
- Gold sinks and item scarcity balancing
- Economy log tracking and analytics

### 9.11 GM/Admin Features
- Account moderation tools
- Item granting and event manipulation
- Server time and event trigger commands
- Ban and warning controls
- Player lookup and state inspection

## 10. Non-Functional Requirements
- High availability target: 99%+
- Stable under target concurrency for MVP: 200–500 concurrent players
- Secure client-server communication
- Predictable CPU and memory usage
- Database integrity and safe migration process
- Monitoring and log retention for debugging and admins
- Scalable structure for future zone expansion

## 11. Fairness Rules and Anti Pay-to-Win Policy
The project will follow strict fairness principles:
- no item directly increases stats for purchase
- no premium gear or power progression for sale
- no direct PvP advantage purchasable by money
- no direct creation of stronger build through cash purchases
- no paid skip of progression or dungeon rewards
- no premium drop chance increase

The monetization model must never compromise gameplay balance.

## 12. Monetization Policy
### Allowed monetization types
- Cosmetic shop
- Skin packs and weapon appearances
- Mount and pet cosmetic skins
- Nameplate, title, or UI cosmetic upgrades
- Supporter subscriptions with non-power rewards
- Convenience features that do not improve player power
- Guild cosmetics and housing decorations

### Disallowed monetization types
- direct stat upgrades
- premium weapons with stronger combat power
- progression skips
- paid-only items that grant PvP dominance
- premium drop chance boost
- any item that changes balance in a meaningful way

## 13. Success Metrics
- 7-day retention above target benchmark
- 30-day retention above target benchmark
- Average play session duration sustained over time
- Active guild participation rate
- Dungeon, raid, and boss clear rate
- PvP participation and leaderboard engagement
- Stable server uptime and low crash rate
- Positive player feedback on fairness and balance

## 14. Risks and Mitigations
### 14.1 Risk: Pay-to-win pressure
Mitigation: strict monetization review, no power items, gameplay fairness audit

### 14.2 Risk: Economy inflation
Mitigation: gold sinks, crafting costs, repair costs, controlled rewards

### 14.3 Risk: PvP imbalance
Mitigation: stat caps, balanced class design, server-side validation, tuning passes

### 14.4 Risk: Anti-cheat abuse
Mitigation: server-side validation, movement anomaly detection, packet validation, admin review tools

### 14.5 Risk: Performance issues
Mitigation: zone-based world architecture, modular services, load testing, monitoring metrics

## 15. Release Phases
### Phase 1 — Core Gameplay Foundation
- login
- character creation
- map movement
- basic combat
- inventory and equipment
- basic monster spawning and AI

### Phase 2 — PvE Content
- dungeons
- bosses
- loot tables
- quest progression
- crafting and economy basics

### Phase 3 — Competitive Play
- duel
- arena
- open-field PvP
- guild basics

### Phase 4 — Endgame and Live Ops
- PvPvE zones
- guild wars
- seasonal events
- anti-cheat tuning
- monetization review

## 16. Out of Scope for MVP
- full-scale housing system
- full advanced economy simulator
- large-scale territory politics
- huge cross-server wars
- deep NPC social simulation
- ultra-complex content before stable core is working

## 17. Acceptance Criteria
The project is considered successful when the following are true:
- players can create accounts and characters
- players can move and interact within world maps
- players can attack monsters and receive rewards
- players can equip and use items
- basic dungeon or boss content works
- PvP duel or arena works
- guild system is functional
- anti-cheat baseline is active
- monetization does not provide power advantage
- the server is stable enough for a live or test environment

## 18. Final Conclusion
This product should be built as a fair, sustainable, competitive PvPvE MMORPG server. The primary differentiator is not simply “RF-like gameplay,” but the combination of:
- strong PvE and PvP loops
- equitable progression
- a legal and ethical monetization model
- clean architecture and controlled scaling
- stability and anti-cheat foundations from the beginning

The server should be designed to survive long-term community retention by prioritizing fairness, compelling content, and sustainable operations.
