$ErrorActionPreference = 'Stop'
Set-Location -LiteralPath $PSScriptRoot
$repo = 'evanshahriar555-spec/LabEquipmentApi'
$url = "https://github.com/$repo.git"

function Run([string]$name, [string[]]$argv) {
    & $name @argv
    if ($LASTEXITCODE -ne 0) { throw "$name failed with exit code $LASTEXITCODE" }
}

Write-Host 'Checking complete project...' -ForegroundColor Cyan
Run 'git' @('status','--short')
Run 'dotnet' @('build','LabEquipmentApi.slnx')

$remote = $null
$remotes = @(& git remote)
if ($remotes -contains 'origin') {
    $remote = (& git remote get-url origin).Trim()
    if ($remote -ne $url) {
        throw "Origin already points to $remote. Expected $url. No changes made."
    }
}

# GitHub CLI can create the repository automatically when installed and authenticated.
$gh = Get-Command gh -ErrorAction SilentlyContinue
if ($gh) {
    & gh auth status 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host 'Sign into GitHub CLI in the browser...' -ForegroundColor Yellow
        Run 'gh' @('auth','login','--web')
    }
    & gh repo view $repo --json name 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Creating public GitHub repository $repo..." -ForegroundColor Cyan
        Run 'gh' @('repo','create',$repo,'--public','--description','Laboratory Equipment Inventory - Server Side Applications')
    }
} else {
    Write-Host ''
    Write-Host 'GitHub CLI is not installed. Please create this empty PUBLIC repository in the browser:' -ForegroundColor Yellow
    Write-Host 'https://github.com/new'
    Write-Host 'Owner: evanshahriar555-spec' -ForegroundColor Yellow
    Write-Host 'Repository name: LabEquipmentApi' -ForegroundColor Yellow
    Write-Host 'Do NOT add a README, .gitignore, or license.' -ForegroundColor Yellow
    Start-Process 'https://github.com/new'
    [void](Read-Host 'Press ENTER after clicking Create repository on GitHub')
}
if (-not $remote) { Run 'git' @('remote','add','origin',$url) }
Run 'git' @('push','-u','origin','main')
Write-Host ''
Write-Host "SUCCESS: https://github.com/$repo" -ForegroundColor Green
Write-Host 'Submit this URL in your Teams assignment.'
Run 'git' @('log','--oneline','-8')
