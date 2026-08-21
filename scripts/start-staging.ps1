$ErrorActionPreference = 'Stop'

Write-Host 'Starting RF staging environment with reverse proxy...'

& docker compose -f "$PSScriptRoot\..\docker-compose.staging.yml" up --build -d

Write-Host 'Staging environment is running.'
Write-Host 'Health check: http://localhost/health'
Write-Host 'API endpoint: http://localhost/api/health'
