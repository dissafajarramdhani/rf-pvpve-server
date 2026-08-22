$ErrorActionPreference = 'Stop'

param(
    [string]$BaseUrl = 'https://localhost'
)

$previousCallback = [System.Net.ServicePointManager]::ServerCertificateValidationCallback
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }

try {
    $paths = @('/health', '/api/health', '/api/ops/status')
    $failed = $false

    foreach ($path in $paths) {
        $uri = "$BaseUrl$path"
        Write-Host "Checking $uri"

        try {
            $response = Invoke-WebRequest -Uri $uri -UseBasicParsing
            Write-Host "Status: $($response.StatusCode)"
            Write-Host $response.Content
        }
        catch {
            $failed = $true
            Write-Error "Smoke check failed for $uri: $($_.Exception.Message)"
        }
    }

    if ($failed) {
        throw 'One or more staging smoke checks failed.'
    }

    Write-Host 'All staging smoke checks passed.'
}
finally {
    [System.Net.ServicePointManager]::ServerCertificateValidationCallback = $previousCallback
}
