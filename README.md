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

## Important policy
This project is designed for fair progression and no pay-to-win mechanics. Any paid feature must be cosmetic or convenience-based and must not directly enhance combat power, item quality, or progression speed.

## License
Add a suitable license before public release. Common choices include MIT or GPL depending on your project goals.
