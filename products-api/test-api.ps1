$ErrorActionPreference = "Stop"
$base = "http://localhost:5199"

Write-Host "=== 1. Valid POST (expect 201 + Location) ===" -ForegroundColor Cyan
$body = @{ name = "Headset"; price = 59.99 } | ConvertTo-Json
$r = Invoke-WebRequest -UseBasicParsing -Uri "$base/products" -Method Post -Body $body -ContentType "application/json"
Write-Host "Status:   $($r.StatusCode)"
Write-Host "Location: $($r.Headers.Location)"
Write-Host "Body:     $($r.Content)"

Write-Host ""
Write-Host "=== 2. Invalid POST (empty name + negative price -> expect 400 ProblemDetails) ===" -ForegroundColor Cyan
try {
    $bad = @{ name = ""; price = -5 } | ConvertTo-Json
    Invoke-WebRequest -UseBasicParsing -Uri "$base/products" -Method Post -Body $bad -ContentType "application/json" | Out-Null
    Write-Host "Unexpected success!"
}
catch {
    $resp = $_.Exception.Response
    Write-Host "Status: $([int]$resp.StatusCode)"
    $reader = New-Object System.IO.StreamReader($resp.GetResponseStream())
    Write-Host "Body:   $($reader.ReadToEnd())"
}

Write-Host ""
Write-Host "=== 3. GET all (should now include Headset) ===" -ForegroundColor Cyan
Invoke-RestMethod -Uri "$base/products" | ConvertTo-Json -Compress
