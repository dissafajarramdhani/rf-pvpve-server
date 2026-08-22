# Production crash recovery runbook and launch gate checklist

## Purpose

This runbook is meant to reduce the risk of a bad production launch and give the team a clear operating procedure when the RF server experiences downtime, data issues, or deployment failures.

## Scope

This document covers:

- production startup and shutdown
- dependency checks (database, reverse proxy, API)
- restore procedures after crash or partial deployment
- launch gate validation before opening access to players
- rollback steps for failed releases

## Production startup checklist

Before starting the production service:

1. Confirm the branch is the approved release branch.
2. Confirm the runtime environment variables are set correctly.
3. Confirm the database connection is valid and reachable.
4. Confirm the production certificate or TLS path is valid.
5. Confirm the API can bind to the desired port.
6. Confirm the health endpoints return success.
7. Confirm metrics and logs are flowing.
8. Confirm backup automation is enabled and recent backups exist.

Example command:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Production'
$env:DB_HOST = 'prod-db.internal'
$env:DB_NAME = 'rf_server'
$env:DB_USER = 'rf_app'
$env:DB_PASSWORD = 'secret'

pwsh ./scripts/start-prod.ps1
```

## Health checks and smoke tests

Run the following checks before launch:

- `http://localhost:8080/health`
- `http://localhost:8080/api/health`
- `http://localhost:8080/api/security/rules`
- `http://localhost:9091/metrics`

For the staging or reverse proxy path:

- `https://localhost/health`
- `https://localhost/api/health`

The API should respond within a normal SLA and the database should be marked as configured.

## Crash recovery procedure

### 1. Detect the issue

Signs of failure:

- health endpoints return 5xx or are unreachable
- API process exits unexpectedly
- database connection errors appear in logs
- Prometheus shows API downtime or error spikes

### 2. Contain the impact

- stop traffic if the issue is severe or data-corrupting
- place the service in maintenance mode if the proxy supports it
- confirm whether the failure is db, API, or reverse proxy related

### 3. Recover by priority

Priority order:

1. Confirm database is reachable and healthy.
2. Restart the API service if it crashed.
3. Validate health endpoints again.
4. Check reverse proxy routing and TLS layers.
5. Restore from the latest known-good backup if data corruption is suspected.

### 4. Verify before reopening

- health endpoints return success
- login/register flow works
- at least one core gameplay flow works (character creation or dungeon run)
- metrics/monitoring are healthy
- no new DB errors are appearing

## Rollback procedure

If the new release fails:

1. Stop the newly deployed version.
2. Restore the previous release build or image tag.
3. Restore the latest backup if the database schema or data is inconsistent.
4. Verify the previous release health endpoints and tasks.
5. Reopen only after success validation.

## Launch gate checklist

The release should only be considered ready when all checks below pass:

- [ ] Production config is set and secrets are not exposed in source control.
- [ ] Database connection is verified.
- [ ] API health endpoint returns success.
- [ ] `/api/health` shows expected environment metadata.
- [ ] `/metrics` is reachable from the monitoring stack.
- [ ] Reverse proxy is routing correctly and TLS terminates properly.
- [ ] Login and register endpoints respond successfully.
- [ ] Character creation and movement endpoints respond successfully.
- [ ] A dungeon or combat flow smoke test passes.
- [ ] Backups have been taken and a restore test has been completed.
- [ ] Monitoring alerts are configured and tested.
- [ ] Rollback plan is documented and ready.
- [ ] On-call contact and escalation chain are known.

## Incident communication

When a production incident occurs, update the following:

- internal team channel
- incident owner
- current status
- scope of impact
- expected recovery time

Do not communicate partial truths or speculate without evidence.

## Post-incident follow-up

After recovery:

1. document the root cause
2. record timestamps and affected systems
3. correct the relevant config, code, or infrastructure gap
4. add monitoring, a guardrail, or automation to prevent recurrence
5. keep the incident record in the project ticket or release notes

## Production policy

- never start a production release without the launch gate checklist passing
- never restore directly over live data without validating the backup
- always keep a rollback path available until the next release is proven stable
