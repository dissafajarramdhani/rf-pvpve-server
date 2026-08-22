[CmdletBinding()]
param(
    [string]$Host = $env:DB_HOST,
    [int]$Port = 5432,
    [string]$Database = $env:DB_NAME,
    [string]$Username = $env:DB_USER,
    [string]$Password = $env:DB_PASSWORD,
    [string]$OutputDirectory = (Join-Path (Resolve-Path (Join-Path $PSScriptRoot '..')).Path 'backups'),
    [int]$RetentionDays = 7,
    [switch]$UseDocker,
    [string]$DockerContainer = 'rf-postgres'
)

if ($null -ne $env:DB_PORT) { $Port = [int]$env:DB_PORT }
if (-not $Host) { throw 'DB_HOST is required. Set it via environment or parameter.' }
if (-not $Database) { throw 'DB_NAME is required. Set it via environment or parameter.' }
if (-not $Username) { throw 'DB_USER is required. Set it via environment or parameter.' }
if (-not $Password) { throw 'DB_PASSWORD is required. Set it via environment or parameter.' }

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null

$timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$archivePath = Join-Path $OutputDirectory "$Database-$timestamp.dump"
$plainSqlPath = Join-Path $OutputDirectory "$Database-$timestamp.sql"

$pgDump = Get-Command pg_dump -ErrorAction SilentlyContinue
if (-not $pgDump -and $UseDocker.IsPresent) {
    $dockerExists = Get-Command docker -ErrorAction SilentlyContinue
    if (-not $dockerExists) { throw 'Docker is required when pg_dump is unavailable and -UseDocker is set.' }
    $pgDump = $null
}

if ($pgDump) {
    $env:PGPASSWORD = $Password
    & $pgDump.Path --host $Host --port $Port --username $Username --format=c --file $archivePath $Database
    if ($LASTEXITCODE -ne 0) { throw "pg_dump failed for database '$Database'." }

    & $pgDump.Path --host $Host --port $Port --username $Username --file $plainSqlPath --clean --if-exists $Database
    if ($LASTEXITCODE -ne 0) { throw "Plain SQL dump failed for database '$Database'." }
}
elseif ($UseDocker.IsPresent) {
    $dockerExists = Get-Command docker -ErrorAction SilentlyContinue
    if (-not $dockerExists) { throw 'Docker is required when pg_dump is unavailable.' }

    $dockerEnv = "PGPASSWORD=$Password"
    $archiveCommand = @(
        'exec', '-i', $DockerContainer,
        'bash', '-lc', "export PGPASSWORD='$Password'; pg_dump --host $Host --port $Port --username $Username --format=c --file /tmp/$Database-$timestamp.dump $Database"
    )
    & docker @archiveCommand
    if ($LASTEXITCODE -ne 0) { throw "Docker-based pg_dump failed for database '$Database'." }

    $copyArchive = @('cp', "${DockerContainer}:/tmp/${Database}-${timestamp}.dump", $archivePath)
    & docker @copyArchive
    if ($LASTEXITCODE -ne 0) { throw "Docker backup copy failed for database '$Database'." }

    $plainCommand = @(
        'exec', '-i', $DockerContainer,
        'bash', '-lc', "export PGPASSWORD='$Password'; pg_dump --host $Host --port $Port --username $Username --file /tmp/$Database-$timestamp.sql --clean --if-exists $Database"
    )
    & docker @plainCommand
    if ($LASTEXITCODE -ne 0) { throw "Docker plain SQL dump failed for database '$Database'." }

    $copyPlain = @('cp', "${DockerContainer}:/tmp/${Database}-${timestamp}.sql", $plainSqlPath)
    & docker @copyPlain
    if ($LASTEXITCODE -ne 0) { throw "Docker plain SQL copy failed for database '$Database'." }
}
else {
    throw "pg_dump was not found on PATH. Install PostgreSQL client tools or rerun with -UseDocker to dump from the database container."
}

Get-ChildItem $OutputDirectory | Where-Object { $_.CreationTime -lt (Get-Date).AddDays(-$RetentionDays) } | Remove-Item -Force -ErrorAction SilentlyContinue

Write-Host "Backup created successfully."
Write-Host "Archive: $archivePath"
Write-Host "Plain SQL: $plainSqlPath"
