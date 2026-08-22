[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$BackupPath,
    [string]$TargetDatabase = $env:DB_NAME,
    [string]$Host = $env:DB_HOST,
    [int]$Port = 5432,
    [string]$Username = $env:DB_USER,
    [string]$Password = $env:DB_PASSWORD,
    [switch]$UseDocker,
    [string]$DockerContainer = 'rf-postgres'
)

if ($null -ne $env:DB_PORT) { $Port = [int]$env:DB_PORT }
if (-not (Test-Path $BackupPath)) { throw "Backup file not found: $BackupPath" }
if (-not $Host) { throw 'DB_HOST is required. Set it via environment or parameter.' }
if (-not $TargetDatabase) { throw 'DB_NAME is required. Set it via environment or parameter.' }
if (-not $Username) { throw 'DB_USER is required. Set it via environment or parameter.' }
if (-not $Password) { throw 'DB_PASSWORD is required. Set it via environment or parameter.' }

$extension = [System.IO.Path]::GetExtension($BackupPath).ToLowerInvariant()

if ($UseDocker.IsPresent) {
    $dockerExists = Get-Command docker -ErrorAction SilentlyContinue
    if (-not $dockerExists) { throw 'Docker is required when -UseDocker is set.' }

    $destination = "/tmp/restore$(Get-Random)"
    $copyCommand = @('cp', $BackupPath, "${DockerContainer}:${destination}")
    & docker @copyCommand
    if ($LASTEXITCODE -ne 0) { throw "Failed to copy backup into the database container: $BackupPath" }

    if ($extension -eq '.dump') {
        $restoreCommand = @(
            'exec', '-i', $DockerContainer,
            'bash', '-lc', "export PGPASSWORD='$Password'; pg_restore --host $Host --port $Port --username $Username --clean --if-exists --dbname $TargetDatabase $destination"
        )
        & docker @restoreCommand
        if ($LASTEXITCODE -ne 0) { throw "pg_restore failed for $BackupPath." }
    }
    else {
        $restoreCommand = @(
            'exec', '-i', $DockerContainer,
            'bash', '-lc', "export PGPASSWORD='$Password'; psql --host $Host --port $Port --username $Username --dbname $TargetDatabase --file $destination"
        )
        & docker @restoreCommand
        if ($LASTEXITCODE -ne 0) { throw "psql failed for $BackupPath." }
    }

    Write-Host "Restore completed from $BackupPath into database '$TargetDatabase'."
    return
}

$pgRestore = Get-Command pg_restore -ErrorAction SilentlyContinue
$psql = Get-Command psql -ErrorAction SilentlyContinue

if ($extension -eq '.dump') {
    if (-not $pgRestore) { throw 'pg_restore is required to restore a custom format archive (.dump). Install PostgreSQL client tools and try again.' }
    $env:PGPASSWORD = $Password
    & $pgRestore --host $Host --port $Port --username $Username --clean --if-exists --dbname $TargetDatabase $BackupPath
    if ($LASTEXITCODE -ne 0) { throw "pg_restore failed for $BackupPath." }
}
else {
    if (-not $psql) { throw 'psql is required to restore a plain SQL dump (.sql). Install PostgreSQL client tools and try again.' }
    $env:PGPASSWORD = $Password
    & $psql --host $Host --port $Port --username $Username --dbname $TargetDatabase --file $BackupPath
    if ($LASTEXITCODE -ne 0) { throw "psql failed for $BackupPath." }
}

Write-Host "Restore completed from $BackupPath into database '$TargetDatabase'."
