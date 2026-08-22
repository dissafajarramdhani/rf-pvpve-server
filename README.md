# RF PvPvE Server

A fair, PvPvE-focused MMORPG server project inspired by RF-style gameplay. The project is designed around a no-pay-to-win model, sustainable operations, and a clear path from core gameplay to live server deployment.

## Project goals
- Fair PvE progression with meaningful rewards
- PvP and PvPvE encounters with balanced incentives
- Guild and social systems that encourage community growth
- Monetization based on cosmetics, convenience, and supporter value without power advantages
- Clean modular architecture from day one

## Repository structure
```text
rf-pvpve-server/
├─ .github/
│  ├─ ISSUE_TEMPLATE/
│  └─ workflows/
├─ db/
│  ├─ migrations/
│  └─ schema/
├─ docs/
├─ scripts/
├─ src/
│  ├─ auth/
│  ├─ character/
│  ├─ combat/
│  ├─ economy/
│  ├─ guild/
│  ├─ pvp/
│  ├─ shared/
│  └─ world/
├─ tests/
│  └─ unit/
├─ .gitignore
├─ README.md
└─ LICENSE
```

## Development direction
This repository is intended for the source code, database schema, tooling, and documentation for the server project. Runtime binaries, client packages, and installer archives are intentionally kept outside of the versioned codebase to avoid bloating the repository and to keep source control focused on maintainable development assets.

## Recommended branch model
- main: production-stable branch
- develop: active development
- feature/*: new feature work
- hotfix/*: urgent fixes

## Getting started
1. Clone the repository.
2. Review the docs in the `docs/` folder.
3. Set up the database schema in `db/schema/`.
4. Implement the foundation modules in `src/`.
5. Verify gameplay loop progression and fairness rules before live deployment.

## Release readiness and hardening
- Health check endpoint: `/health`
- Runtime summary endpoint: `/api/health`
- Security rules endpoint: `/api/security/rules`
- Docker packaging available via `Dockerfile` and `docker-compose.yml`
- Local production-like deployment can be started with `docker compose up --build`

## Production deployment configuration
- Copy `.env.example` to a private `.env` file and fill in actual database and host values.
- Use the production appsettings file at `src/RF.Server.Api/appsettings.Production.json` for environment-specific settings.
- Launch production mode with:
  - `pwsh ./scripts/start-prod.ps1`
  - or `docker compose up --build`
- Do not commit real production secrets or credentials to the repository.

## Staging environment and reverse proxy
- A staging deployment is defined in `docker-compose.staging.yml`.
- Nginx reverse proxy config is in `nginx/default.conf`.
- Launch staging stack with:
  - `pwsh ./scripts/start-staging.ps1`
  - or `docker compose -f docker-compose.staging.yml up --build -d`
- Health checks are exposed through the reverse proxy on `https://localhost/health` and `https://localhost/api/health`.
- The staging stack includes TLS termination at Nginx and redirects plain HTTP traffic to HTTPS.
- A local self-signed certificate is generated automatically under `nginx/certs/` by the staging bootstrap script for non-production validation.

## Staging smoke checks and observability
- For a basic smoke test, call the health endpoints after the stack is running.
- Supported baseline endpoints:
  - `https://localhost/health`
  - `https://localhost/api/health`
  - `https://localhost/api/ops/status`
- Request logging is enabled via ASP.NET Core `HttpLogging` to capture method, path, response status, and duration.
- Production-readiness baseline includes health checks, runtime uptime metadata, and structured request logging for incident investigation.

## Production monitoring stack
- Monitoring stack is defined in `docker-compose.monitoring.yml` and uses Prometheus, Grafana, and Alertmanager.
- Start it with:
  - `docker compose -f docker-compose.monitoring.yml up --build -d`
- Access points:
  - Prometheus: `http://localhost:9090`
  - Grafana: `http://localhost:3000` (admin/admin)
  - Alertmanager: `http://localhost:9093`
  - API metrics: `http://localhost:9091/metrics`
  - API health: `http://localhost:8080/health`
- The application exposes Prometheus metrics via the `prometheus-net` package and `/metrics` endpoint for scraping.
- Grafana is pre-provisioned with a default RF dashboard and Prometheus data source.

## PostgreSQL backup and restore strategy
- Backup strategy and restore playbook are documented in `docs/postgres-backup-restore.md`.
- Backup automation script: `scripts/backup-postgres.ps1`
- Restore automation script: `scripts/restore-postgres.ps1`
- Recommended workflow:
  - create a `pg_dump` archive backup daily
  - keep a 7–30 day retention window
  - test restores at least once per release cycle
  - never restore directly over the live production database without a rollback plan
- Backup files are written to the `backups/` directory by default and are excluded from source control.

## Production crash recovery runbook and launch gate checklist
- Full operational recovery steps and rollout checks are in `docs/production-runbook.md`.
- The launch gate requires validation of:
  - production config
  - API health and metrics
  - DB connectivity
  - login and character flows
  - anti-cheat sanity checks
  - backup readiness
  - monitoring coverage
  - rollback plan

## Final production launch checklist
- Final release approval checklist: `docs/production-launch-checklist.md`
- The launch gate should confirm technical, data integrity, fairness, and operations readiness before public release.

## Soft launch strategy
- Soft launch plan and controlled rollout guidance: `docs/soft-launch-strategy.md`
- Recommended soft launch flow: invite-only or region-limited access, followed by gradual expansion only after stability metrics are verified.

## Production release approval narrative
- Final go-live decision memo: `docs/go-live-decision-memo.md`
- This memo captures the rationale, risk assessment, and approval basis for moving from internal validation to a controlled production release.

## Release planning and post-launch roadmap
- Post-launch roadmap: `docs/post-launch-roadmap.md`
- This plan defines the soft launch, controlled growth, and long-term operations strategy after the initial production release.

## Final project summary and next actions
- Full project summary and team action plan: `docs/final-project-summary.md`
- This document captures the project status, what has been achieved, and the recommended next actions for the team.

## Important policy
This project is designed for fair progression and no pay-to-win mechanics. Any paid feature must be cosmetic or convenience-based and must not directly enhance combat power, item quality, or progression speed.

## License
Add a suitable license before public release. Common choices include MIT or GPL depending on your project goals.
