# PostgreSQL backup and restore strategy

## Goals

This project needs a simple, production-safe PostgreSQL backup process that can be used for both staging and production. The strategy below is designed to be:

- low complexity
- easy to automate
- safe for frequent disaster recovery testing
- suitable for a small-to-mid-size game backend

## Recommended backup pattern

Use a two-layer approach:

1. Daily full dumps in custom archive format (`.dump`)
2. Optional plain SQL dumps (`.sql`) for quick review or partial restore

The custom archive format (`pg_dump --format=c`) is the most robust choice for restore reliability. It preserves schema and data together and is easier to restore cleanly than a raw SQL file when the database grows.

## Backup script

The repository includes a script at `scripts/backup-postgres.ps1`.

Example:

```powershell
$env:DB_HOST = 'prod-db.internal'
$env:DB_PORT = '5432'
$env:DB_NAME = 'rf_server'
$env:DB_USER = 'rf_app'
$env:DB_PASSWORD = 'super-secret'

pwsh ./scripts/backup-postgres.ps1 -RetentionDays 7
```

This writes backups into the repository-local `backups/` directory by default and keeps only files younger than the configured retention window.

If PostgreSQL client tools are not installed on the local machine, use the Docker-backed path:

```powershell
pwsh ./scripts/backup-postgres.ps1 -UseDocker -DockerContainer rf-postgres
```

## Restore script

The repository includes a restore script at `scripts/restore-postgres.ps1`.

Example restore from a custom archive:

```powershell
$env:DB_HOST = 'prod-db.internal'
$env:DB_PORT = '5432'
$env:DB_NAME = 'rf_server_restore'
$env:DB_USER = 'rf_app'
$env:DB_PASSWORD = 'super-secret'

pwsh ./scripts/restore-postgres.ps1 -BackupPath .\backups\rf_server-20260822-120000.dump
```

Example restore from a plain SQL dump:

```powershell
pwsh ./scripts/restore-postgres.ps1 -BackupPath .\backups\rf_server-20260822-120000.sql
```

## Backup schedule

Recommended automation:

- Every 6 hours: small incremental or full dump depending on database size
- Daily: full dump retained for 7–30 days
- Weekly: longer-retention cold storage copy to object storage or external drive

For Windows Server or a VM host, schedule the script with Task Scheduler. For Linux hosts, use cron or systemd timers.

Example cron job:

```bash
0 */6 * * * /usr/bin/pwsh -File /opt/rf-server/scripts/backup-postgres.ps1 -RetentionDays 14
```

## Restore playbook

1. Stop writes to the target API or place the application into maintenance mode.
2. Verify the backup file integrity.
3. Create a temporary target database name if you want a side-by-side restore.
4. Run `restore-postgres.ps1` against the backup.
5. Validate account, character, guild, and dungeon data counts.
6. Resume traffic only after verification passes.

## Verification checklist

After any restore, verify:

- login and registration still work
- player characters are present
- guild membership data is intact
- dungeon or arena progress is readable
- app health endpoint returns `healthy`

## Production policy

- Never restore over the live production database without a written rollback plan.
- Store backup files outside the repository when possible.
- Encrypt backups if they leave the host or are stored in shared storage.
- Keep at least one backup copy off-site or in object storage.
- Test restores at least once per release cycle.
