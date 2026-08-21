# RF PvPvE Server Architecture Blueprint

## 1. Overview
This document defines the technical architecture for a PvPvE MMORPG server inspired by RF-style gameplay. The architecture is designed to support:
- account and character management
- adventure and PvE progression
- PvP and PvPvE combat
- guild and social systems
- stable economy
- anti-cheat baseline
- eventual scaling to a live environment

The design follows a server-authoritative model: the server is the source of truth for combat, progression, inventory, and world state. The client is treated as an input device and must not be trusted for critical logic.

## 2. Architectural Principles
1. Server authoritative gameplay
2. Strict separation of modules and responsibilities
3. Zone-based world processing
4. Persistent data stored securely in database
5. Cache for hot data and session optimization
6. Never trust client values for combat or progression
7. Build with scaling and patching in mind
8. Use migration-based DB updates
9. Logs for everything important
10. Fairness is a design constraint, not a feature request

## 3. High-Level System Layout
- Client
- Gateway / Proxy
- Auth Service
- Character Service
- World / Zone Service
- Combat Service
- PvP Service
- PvPvE Service
- Inventory / Equipment Service
- Economy Service
- Guild Service
- Event Service
- Chat Service
- Database Layer
- Cache Layer
- Admin Tools
- Monitoring and Logging

## 4. Major Components

### 4.1 Client
The client handles:
- login UI
- character selection
- movement input
- skill casting intent
- combat input
- inventory UI and trade actions
- map rendering and display

The client should never decide final combat results.

### 4.2 Gateway / Proxy
The gateway acts as the front door for client connections.

Responsibilities:
- accept TCP sockets from clients
- validate sessions
- route packets to proper service
- handle connection limits and anti-abuse rules
- hide internal topology from clients

### 4.3 Auth Service
Responsibilities:
- register and login accounts
- verify credentials using secure hashing
- issue access tokens or sessions
- validate ban status
- route players to the right world/zone

Primary tables/data:
- account
- login_log
- session
- banned_account

### 4.4 Character Service
Responsibilities:
- load and save character data
- create, rename, and delete character
- load equipment and appearance
- maintain progression state
- expose character summary for login flow

Core data:
- character
- character_stats
- skill_tree
- appearance
- equipment

### 4.5 World / Zone Service
This is the service that owns the live world state.

Responsibilities:
- maintain player objects in a zone
- maintain NPC and monster states
- process movement updates and region changes
- handle map transfer, instance creation, and spawn logic
- broadcast nearby updates to players
- manage area events and dynamic world states

Core concepts:
- world map
- zone region
- map instance
- spawn points
- nearby player visibility

### 4.6 Combat Service
Responsibilities:
- resolve skill and attack requests
- verify validity and range
- calculate damage and defense
- apply buffs/debuffs and critical hits
- manage death and revive logic
- log all combat outcomes

Rules:
- Attack and damage are authoritative on the server
- Client sends input only; server resolves the result
- Combat logs must be stored for debugging and anti-cheat review

### 4.7 PvP Service
Responsibilities:
- duel flow
- arena match management
- open-field PvP rules
- honor or rank update
- reward distribution
- match log storage

Important design note:
- PvP rewards should be fair and non-power-heavy
- PvP trophy systems can be cosmetic or honor-based without giving direct stat advantage

### 4.8 PvPvE Service
Responsibilities:
- danger zone logic
- world boss with PvP pressure
- mixed combat scenarios
- raid or event pressure while players compete or cooperate
- reward evaluation for risk and participation

### 4.9 Inventory and Equipment Service
Responsibilities:
- item pickup and drop logic
- inventory slot validation
- equip and unequip flow
- item stat management
- storage and bank adjustments
- item ownership and trade validation

Important rules:
- no item duplication allowed
- item ownership must be tracked uniquely
- item instance state must be stored and logged

### 4.10 Economy Service
Responsibilities:
- manage gold, crafting, item selling, and market economy
- track economy transactions
- apply repair costs, upgrade costs, and gold sinks
- apply supply and demand tuning across item families

Examples of gold sinks:
- repair costs
- weapon or armor upgrades
- crafting materials
- storage expansion costs
- event participation fees or consumables

### 4.11 Guild Service
Responsibilities:
- guild creation
- role assignment
- member management
- guild chat and announcements
- guild storage and funds
- guild combat or territorial participation

### 4.12 Event Service
Responsibilities:
- schedule event timers
- trigger world boss events
- manage seasonal activities
- distribute rewards
- handle event states and logs

### 4.13 Chat Service
Responsibilities:
- direct chat
- global chat
- guild chat
- party chat
- system announcements

### 4.14 Anti-Cheat Service
Responsibilities:
- detect abnormal packet behavior
- detect impossible movement
- detect invalid combat actions
- detect duplicated item or resource actions
- detect speed or teleport anomalies
- flag suspicious players for review

This service is critical to server fairness and security.

## 5. Runtime Flow
### 5.1 Login and Character Selection
1. Client connects to Gateway
2. Gateway routes login request to Auth Service
3. Auth verifies credentials and ban status
4. Session token is issued
5. Client requests character list
6. Character Service loads and returns active characters
7. Client chooses a character
8. World Service spawns player into the map

### 5.2 Movement and World Update
1. Client sends movement packet
2. Gateway routes packet to Zone Service
3. Zone Service validates actor state and map bounds
4. Server updates player position in world state
5. Nearby clients receive position update
6. DB and cache updates only when necessary

### 5.3 Combat Flow
1. Client sends skill or attack intent
2. Zone Service forwards to Combat Service
3. Combat Service validates range, cooldown, target, and stats
4. Damage and state changes are resolved server-side
5. Buffs, HP changes, death, and respawn are processed
6. Logs are written
7. Nearby players receive combat update

### 5.4 PvP and PvPvE Flow
1. System or player triggers PvP or PvPvE context
2. PvP or PvPvE service validates arena or zone rules
3. combat and result resolution handled by server
4. reward and rank updates allocated by service
5. logs saved and event data persisted

## 6. Data Architecture
The architecture is separated into persistent model and runtime model.

### 6.1 Persistent Data
Stored in relational database:
- account
- character
- inventory
- equipment
- item_template
- item_instance
- guild
- guild_member
- quest_progress
- skill_tree
- combat_log
- economy_log
- event_log
- anti_cheat_log

### 6.2 Runtime Data
Stored in memory / temporary caches, not all persisted:
- active players
- current zone objects
- active monster states
- buff states
- combat state
- temporary event states
- recent packet state

## 7. Database Strategy
### 7.1 Recommended Databases
- PostgreSQL or MariaDB as primary database
- Redis for hot data access and session cache

### 7.2 Storage Responsibilities
- PostgreSQL/MariaDB: account, character, item, guild, economy, combat logs, anti-cheat logs
- Redis: player sessions, active world state caches, templates, event timers, guild hot data

### 7.3 Data Integrity Rules
- use transactions for critical persistence
- use migration scripts for schema updates
- always validate item ownership before operations
- never save a client-provided state as authoritative
- store game actions in logs for debugging and review

## 8. Cache Strategy
Recommended Redis usage:
- account session cache
- active character state summary
- item templates and static game data
- guild metadata and roster summary
- map or zone metadata
- event timers and spawn cache

Do not store all world state in Redis permanently. Use it for hot, frequently accessed, and transient values.

## 9. Zone and World Scaling Strategy
### 9.1 Initial Structure
- 1 Auth Service
- 1 Gateway
- 2–4 Zone Servers depending on player count
- 1 Chat Service
- 1 DB Primary
- 1 Redis
- 1 Monitoring stack

### 9.2 Scaling Principles
- Separate map regions by zone server
- Use instance servers for dungeons and PvP arenas
- Load-balance by active player count and region
- Split high-traffic maps when needed
- Isolate event or boss nodes if they become hotspots

## 10. Communication Layer
### 10.1 Network Protocol
- TCP-based socket architecture
- custom packet framing or efficient binary protocol
- opcodes for packet routing
- packet validation and throttling

### 10.2 Message Types
- auth packets
- movement packets
- combat packets
- inventory packets
- chat packets
- event packets
- admin packets

## 11. Security Model
### 11.1 Security Requirements
- secure login transmission
- session validation
- packet rate limiting
- crypto or secure channels for sensitive traffic
- server-side state validation
- restricted admin endpoints
- protected database access

### 11.2 Anti-Cheat Baseline
- movement validation
- impossible distance detection
- invalid packet detection
- fight range validation
- duplicate item or reward checks
- skill cooldown validation
- speed/teleport anomaly detection
- suspicious packet spike detection

## 12. Monitoring and Operations
### 12.1 Tools
- Prometheus
- Grafana
- centralized logging
- health checks
- crash logs
- DB query monitoring

### 12.2 Metrics to Track
- active players
- world tick latency
- map occupancy
- login success/failure
- combat actions per second
- DB query latency
- economy anomalies
- crash and restart counts
- anti-cheat warnings

## 13. Admin and GM Tools
Admin tooling should include:
- account lookup
- ban/unban management
- item granting
- character inspection
- time and event triggers
- player state editing (limited and logged)
- map teleport/relocate
- event management dashboards
- audit and log review

## 14. Deployment Architecture
### 14.1 Recommended Setup
- Linux environment for services
- Docker for service isolation
- CI/CD pipeline for build and deploy
- DB backup and restore testing
- staging environment before production

### 14.2 Recommended Production Topology
- gateway
- auth service
- character service
- 2+ zone servers
- chat service
- DB master
- Redis cache
- monitoring and logging stack
- admin console

## 15. Recommended Stack
### Languages / Runtime
- C++ or C# for game server core
- SQL for DB schema
- Redis for cache
- shell or Python for deployment scripts and automation

### Tools
- GitHub for source control
- GitHub Actions for CI/CD
- Docker for deployment
- Prometheus + Grafana for observability

## 16. Development Milestones
### Milestone 1: Foundation
- auth system
- character creation
- world map loading
- movement
- basic combat
- inventory system

### Milestone 2: Content
- monster AI
- dungeon system
- boss event
- loot tables
- craftingeconomy basics

### Milestone 3: Competitive Play
- duel support
- arena or PvP zone
- guild system
- PvPvE pressure zones

### Milestone 4: Live Ops and Stability
- event scheduling
- anti-cheat tuning
- performance and load testing
- production-ready deployment

## 17. Final Architectural Summary
The server architecture should follow a modular, server-authoritative, zone-based model. The key to success is balancing gameplay fairness, maintainability, scalability, and operational visibility.

This backend design supports:
- clean code organization
- future expansion into more zones and content
- robust economy and progression logic
- secure and fair PvP
- robust anti-cheat foundations
- sustainable live service operations

The architecture is designed to evolve from MVP to a live, community-run MMORPG environment without requiring a full rewrite.
