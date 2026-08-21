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

## Important policy
This project is designed for fair progression and no pay-to-win mechanics. Any paid feature must be cosmetic or convenience-based and must not directly enhance combat power, item quality, or progression speed.

## License
Add a suitable license before public release. Common choices include MIT or GPL depending on your project goals.
