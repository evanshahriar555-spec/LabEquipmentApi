param(
    [string]$BaseUrl = 'https://localhost:7222',
    [switch]$SkipCertificateCheck
)
$ErrorActionPreference = 'Stop'
if ($SkipCertificateCheck) {
    # PowerShell 7 supports this parameter. On Windows PowerShell 5.1,
    # rely on the trusted ASP.NET development certificate instead.
    if ($PSVersionTable.PSVersion.Major -ge 7) {
        $requestExtra = @{ SkipCertificateCheck = $true }
    } else {
        Write-Warning 'Windows PowerShell 5.1 ignores -SkipCertificateCheck. Trust the .NET developer HTTPS certificate, or use PowerShell 7.'
        $requestExtra = @{}
    }
} else { $requestExtra = @{} }

function Call-Api([string]$Method, [string]$Path, [string]$Json) {
    $args = @{Uri=($BaseUrl.TrimEnd('/') + $Path);Method=$Method;ErrorAction='Stop'}
    if ($PSVersionTable.PSVersion.Major -lt 6) { $args['UseBasicParsing'] = $true }
    foreach($key in $requestExtra.Keys) { $args[$key] = $requestExtra[$key] }
    if ($Json) { $args['Body']=$Json; $args['ContentType']='application/json' }
    return Invoke-WebRequest @args
}
function Assert([bool]$ok, [string]$message) {
    if (-not $ok) {throw "FAILED: $message"}
    Write-Host "PASS: $message" -ForegroundColor Green
}
Write-Host 'Testing read-only endpoints before any mutation...'
$all = Call-Api 'GET' '/api/equipment' ''
$items = @($all.Content | ConvertFrom-Json)
Assert ($all.StatusCode -eq 200 -and $items.Count -ge 3) 'GET all returns records'
$one = Call-Api 'GET' '/api/equipment/1' ''
Assert ($one.StatusCode -eq 200 -and ($one.Content | ConvertFrom-Json).name -eq 'Projector') 'GET /1 is Projector'
$available = @( (Call-Api 'GET' '/api/equipment/available' '').Content | ConvertFrom-Json )
Assert (($available | Where-Object {$_.name -eq 'Monitor'}).Count -ge 1) 'Monitor appears in available'
Assert (($available | Where-Object {$_.name -eq 'Laptop'}).Count -eq 0) 'Unavailable Laptop is omitted'
$summary = @( (Call-Api 'GET' '/api/equipment/room-summary' '').Content | ConvertFrom-Json )
Assert (($summary | Where-Object {$_.room -eq 'Lab 10'}).Count -ge 1) 'Optional room-summary endpoint works'

function Expected-HttpError([string]$Method, [string]$Path, [string]$Json, [int]$Code) {
    try {
        $null = Call-Api $Method $Path $Json
        throw "Expected HTTP $Code from $Method $Path, but it succeeded."
    } catch {
        if (-not $_.Exception.Response) { throw }
        $actual = [int]$_.Exception.Response.StatusCode
        Assert ($actual -eq $Code) "$Method $Path returned $Code"
    }
}
Expected-HttpError 'GET' '/api/equipment/999' '' 404
Expected-HttpError 'POST' '/api/equipment' '{"name":"","room":"Lab 10"}' 400
Expected-HttpError 'POST' '/api/equipment' '{"name":"Mouse","room":""}' 400

Write-Host 'Creating a temporary test item (will be deleted)...'
$new = Call-Api 'POST' '/api/equipment' '{"name":"Test Keyboard","room":"Lab 99"}'
$newItem = $new.Content | ConvertFrom-Json
Assert ($new.StatusCode -eq 201 -and $newItem.isAvailable -eq $true) 'POST creates available equipment (201)'
try {
    $got = Call-Api 'GET' ("/api/equipment/" + $newItem.id) ''
    Assert ($got.StatusCode -eq 200) 'GET by created ID'
} finally {
    $deleted = Call-Api 'DELETE' ("/api/equipment/" + $newItem.id) ''
    Assert ($deleted.StatusCode -eq 204) 'DELETE removes temporary test item'
}
Write-Host 'All smoke checks passed.' -ForegroundColor Green
