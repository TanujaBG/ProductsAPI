$ErrorActionPreference = "Stop"
$base = "http://localhost:5182"

function Test-Endpoint($label, $method, $url, $body) {
    try {
        if ($null -ne $body) {
            $json = $body | ConvertTo-Json -Compress
            $r = Invoke-WebRequest -UseBasicParsing -Uri $url -Method $method -Body $json -ContentType "application/json"
        }
        else {
            $r = Invoke-WebRequest -UseBasicParsing -Uri $url -Method $method
        }
        Write-Host "$label  ->  $($r.StatusCode)" -ForegroundColor Green
        if ($r.Content) { Write-Host "   $($r.Content)" }
    }
    catch {
        $resp = $_.Exception.Response
        $code = [int]$resp.StatusCode
        $reader = New-Object System.IO.StreamReader($resp.GetResponseStream())
        Write-Host "$label  ->  $code" -ForegroundColor Yellow
        Write-Host "   $($reader.ReadToEnd())"
    }
}

Test-Endpoint "1. GET /v1/products (list)"            "GET"  "$base/v1/products" $null
Test-Endpoint "2. GET /v1/categories"                 "GET"  "$base/v1/categories" $null
Test-Endpoint "3. GET /v1/products/1/with-category"   "GET"  "$base/v1/products/1/with-category" $null
Test-Endpoint "4. POST valid (categoryId 1)"          "POST" "$base/v1/products" @{ name = "Webcam"; price = 40; categoryId = 1 }
Test-Endpoint "5. POST bad category (categoryId 999)" "POST" "$base/v1/products" @{ name = "Ghost"; price = 10; categoryId = 999 }
Test-Endpoint "6. POST invalid (empty name)"          "POST" "$base/v1/products" @{ name = ""; price = -5; categoryId = 1 }
Test-Endpoint "7. GET page 1 size 2"                  "GET"  "$base/v1/products/page?page=1&size=2" $null
Test-Endpoint "8. GET /v1/categories (count went up?)" "GET" "$base/v1/categories" $null
