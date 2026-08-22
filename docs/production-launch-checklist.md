# Final production launch checklist

## Launch readiness gate

This checklist should be completed before the server is opened to a larger player base or public launch.

### Infrastructure

- [ ] Production database is reachable and healthy.
- [ ] Backups are enabled and tested.
- [ ] Reverse proxy and TLS are configured.
- [ ] Health endpoints are passing.
- [ ] Metrics and alerting are active.
- [ ] Secrets are stored outside the repo and not committed to source control.
- [ ] Monitoring dashboards are visible to the ops team.

### Application readiness

- [ ] API boots successfully in Production mode.
- [ ] `/api/health` returns expected status.
- [ ] `/metrics` is being scraped successfully.
- [ ] Login and registration flow works.
- [ ] Character creation works.
- [ ] World movement works.
- [ ] Basic combat works.
- [ ] Dungeon encounter flow works.
- [ ] Guild creation/join flow works.
- [ ] PvP or duel flow works if enabled.
- [ ] Anti-cheat guardrails are functioning.

### Data and game integrity

- [ ] Character data can be created and retrieved correctly.
- [ ] Inventory and equipment state is persisted correctly.
- [ ] Dungeon rewards and loot logic are valid.
- [ ] Economy values are in expected ranges.
- [ ] No critical progression blockers are discovered.

### Operations

- [ ] Production runbook is available.
- [ ] Rollback process is documented and tested.
- [ ] Support and moderation workflow is prepared.
- [ ] Incident owner and escalation chain are confirmed.
- [ ] A post-incident communication process is documented.

### Commercial and fairness checks

- [ ] Monetization avoids pay-to-win.
- [ ] Cosmetic/convenience systems are clearly separated from combat power.
- [ ] Premium content does not directly speed progression or increase drop quality.
- [ ] Fairness policy is visible to players and staff.

### Final sign-off

- [ ] Technical lead approves server stability.
- [ ] Game design lead approves balance and fairness.
- [ ] Operations lead approves monitoring and recovery plan.
- [ ] Release owner approves soft launch or public launch timing.

## Launch decision rule

A production launch should be approved only if every required item above is checked, with any unresolved risk documented and accepted by the release owner.
