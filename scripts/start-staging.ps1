$ErrorActionPreference = 'Stop'

$certDir = Join-Path $PSScriptRoot '..\nginx\certs'
$certDir = [System.IO.Path]::GetFullPath($certDir)
$certPath = Join-Path $certDir 'local-dev.crt'
$keyPath = Join-Path $certDir 'local-dev.key'

New-Item -ItemType Directory -Force -Path $certDir | Out-Null

if (-not (Test-Path $certPath) -or -not (Test-Path $keyPath)) {
    $openssl = Get-Command openssl -ErrorAction SilentlyContinue
    if (-not $openssl) {
        $gitBin = Join-Path ${env:ProgramFiles} 'Git\usr\bin\openssl.exe'
        if (Test-Path $gitBin) {
            $openssl = $gitBin
        }
    }

    if (-not $openssl) {
        throw 'OpenSSL is required to generate a self-signed certificate for the staging TLS endpoint. Install OpenSSL or create the cert/key manually under nginx/certs.'
    }

    & $openssl req -x509 -nodes -newkey rsa:2048 -keyout $keyPath -out $certPath -days 365 -subj "/CN=localhost" | Out-Null
}

Write-Host 'Starting RF staging environment with HTTPS reverse proxy...'

& docker compose -f "$PSScriptRoot\..\docker-compose.staging.yml" up --build -d

Write-Host 'Staging environment is running.'
Write-Host 'Health check: https://localhost/health'
Write-Host 'API endpoint: https://localhost/api/health'
Write-Host 'TLS certificate: ' $certPath
