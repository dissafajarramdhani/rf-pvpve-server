$ErrorActionPreference = 'Stop'

$env:ASPNETCORE_ENVIRONMENT = 'Production'
$env:ASPNETCORE_URLS = if ($env:ASPNETCORE_URLS) { $env:ASPNETCORE_URLS } else { 'http://0.0.0.0:8080' }

if (-not $env:DB_HOST) { throw 'DB_HOST is required. Set it before running production mode.' }
if (-not $env:DB_NAME) { throw 'DB_NAME is required. Set it before running production mode.' }
if (-not $env:DB_USER) { throw 'DB_USER is required. Set it before running production mode.' }
if (-not $env:DB_PASSWORD) { throw 'DB_PASSWORD is required. Set it before running production mode.' }

$connectionString = "Host=$($env:DB_HOST);Port=$($env:DB_PORT ?? '5432');Database=$($env:DB_NAME);Username=$($env:DB_USER);Password=$($env:DB_PASSWORD)"
$env:ConnectionStrings__DefaultConnection = $connectionString

Write-Host "Starting RF Server in Production mode..."
Write-Host "Connection: $connectionString"

& dotnet run --project "$PSScriptRoot\..\src\RF.Server.Api\RF.Server.Api.csproj" --urls $env:ASPNETCORE_URLS
