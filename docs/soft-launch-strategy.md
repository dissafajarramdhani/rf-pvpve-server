# Soft launch strategy for RF PvPvE server

## Goal

The goal of the soft launch is to validate the live server under realistic traffic conditions without exposing the product to full public risk. This creates a controlled environment for collecting feedback, testing server stability, and validating the economy, fairness rules, and operational workflows.

## Soft launch principles

- Keep the first release narrow and controlled.
- Limit access to trusted players, community testers, or a small region.
- Prioritize server stability over broad reach.
- Validate balance, anti-cheat, and operational workflows before scaling.
- Track data quality and incident reliability before wider roll-out.

## Recommended soft launch scope

### Phase 1: Closed beta

- 50–200 invited players
- trusted community / internal testers
- only selected maps, dungeons, and PvP modes enabled
- restricted progression or content unlocks if needed
- collect crash reports, latency, and gameplay feedback

### Phase 2: Controlled public access

- limited public registration or invite-only access
- one region or one world shard only
- server monetization active only for cosmetics or convenience features
- limited event schedule to reduce operational risk

### Phase 3: Expansion phase

- increase world capacity gradually
- enable additional dungeons and PvP features after stability data is confirmed
- monitor economy and player churn before further scaling

## Launch gates before soft launch

Do not start soft launch until all of the following are true:

- production environment is configured
- database is live and backup-tested
- API health and metrics pass checks
- login, account, character, and movement flows pass smoke tests
- combat, dungeon, PvP, and guild flows are validated
- anti-cheat checks are in place and monitored
- payment or monetization system is restricted to non-power advantages
- support and moderation workflow is ready
- rollback procedure is tested once

## Player access model

Recommended access model for the first 4–6 weeks:

- invite codes or limited registration window
- first-come / whitelist-based access
- limited regions or server shards
- support email or Discord channel linked to server status

This makes it easier to control load and monitor the first waves of player behavior.

## Monitoring goals during soft launch

Watch closely for:

- API error rate
- database latency and connection failures
- request spikes or abnormal resource usage
- combat imbalance reports
- player reports of unfair PvP interactions
- progression blockers or quest/dungeon dead-ends
- suspicious account activity or cheating patterns

Each issue should be captured and mapped to a quick fix or operational response.

## Operational cadence during soft launch

Recommended cadence:

- hourly check of health and error dashboards
- daily review of player feedback and reported bugs
- twice-per-day review of economy and progression trends
- weekly stability review and patch release if necessary

## Soft launch success criteria

The soft launch is successful when:

- uptime remains above target threshold
- no major data loss or restore events occur
- server remains playable under community load
- combat and progression remain fair
- players can complete core loops without critical blockers
- support and moderation can respond quickly to issues

## Expansion decision rule

Do not expand to public launch until:

- all major critical bugs are resolved
- monitoring is stable and alerts are proven
- rollback plan is rehearsed
- support flow is working
- economy and fairness feedback is acceptable for scale

## Public launch trigger

Public launch should happen only after soft launch results are reviewed and the team decides that technical, operational, and game balance foundations are stable.

## Important policy

The soft launch is not a full marketing campaign. It is a controlled operational validation stage. The purpose is to collect evidence and stabilize the server before broader public exposure.
