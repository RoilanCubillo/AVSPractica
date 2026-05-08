$ErrorActionPreference = "Stop"

$root = "C:\Users\developer01\Downloads\UltraManagerAdmin (1)"
$repo = Join-Path $root "UltraManagerAdmin"
$msbuild = "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"
$iisExpress = "C:\Program Files\IIS Express\iisexpress.exe"

$securityProject = Join-Path $repo "Security.Services\Security.WebServices.csproj"
$erpProject = Join-Path $repo "WebApplication1\UltraERP.WebServices.csproj"
$centralProject = Join-Path $repo "CentralAdmin\CentralAdmin.csproj"
$ultraErpProject = Join-Path $repo "UltraERP\UltraERP.csproj"
$solution = Join-Path $repo "UltraERP\UltraERP.sln"

$securitySite = Join-Path $repo "Security.Services"
$erpSite = Join-Path $repo "WebApplication1"
$centralSite = Join-Path $repo "CentralAdmin"
$ultraErpSite = Join-Path $repo "UltraERP"

$retailHeroTargetDir = Join-Path $repo "UltraERP.BusinessDataAccess\libs"
$retailHeroTarget = Join-Path $retailHeroTargetDir "RetailHero.POS.Core.Shared.dll"
$retailHeroFallback = "C:\Users\developer01\Downloads\UltraManagerAdmin\UltraERP.BusinessDataAccess\libs\RetailHero.POS.Core.Shared.dll"

function Invoke-Step {
    param(
        [string]$Title,
        [scriptblock]$Action
    )

    Write-Host ""
    Write-Host "== $Title ==" -ForegroundColor Cyan
    & $Action
}

function Start-IISSite {
    param(
        [string]$Path,
        [int]$Port,
        [string]$Name
    )

    $listening = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue
    if ($listening) {
        Write-Host "$Name ya esta escuchando en el puerto $Port." -ForegroundColor Yellow
        return
    }

    Start-Process $iisExpress -ArgumentList "`"/path:$Path`" /port:$Port" -WindowStyle Hidden | Out-Null
    Start-Sleep -Seconds 3

    $listening = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue
    if (-not $listening) {
        throw "No se pudo iniciar $Name en el puerto $Port."
    }

    Write-Host "$Name iniciado en http://localhost:$Port/" -ForegroundColor Green
}

Set-Location $root

Invoke-Step "Preparando dependencia RetailHero" {
    if (-not (Test-Path $retailHeroTarget)) {
        if (Test-Path $retailHeroFallback) {
            New-Item -ItemType Directory -Force -Path $retailHeroTargetDir | Out-Null
            Copy-Item $retailHeroFallback $retailHeroTarget -Force
            Write-Host "DLL RetailHero copiada." -ForegroundColor Green
        }
        else {
            Write-Host "No se encontro la DLL RetailHero en la ruta de respaldo." -ForegroundColor Yellow
        }
    }
    else {
        Write-Host "DLL RetailHero ya existe." -ForegroundColor Yellow
    }
}

Invoke-Step "Restaurando paquetes NuGet" {
    & $msbuild $solution /t:Restore /p:RestorePackagesConfig=true
}

Invoke-Step "Compilando Security.Services" {
    & $msbuild $securityProject /t:Build /p:Configuration=Debug
}

Invoke-Step "Compilando WebApplication1" {
    & $msbuild $erpProject /t:Build /p:Configuration=Debug
}

Invoke-Step "Compilando CentralAdmin" {
    & $msbuild $centralProject /t:Build /p:Configuration=Debug
}

Invoke-Step "Compilando UltraERP Inventario" {
    & $msbuild $ultraErpProject /t:Build /p:Configuration=Debug
}

Invoke-Step "Levantando sitios" {
    Start-IISSite -Path $securitySite -Port 49273 -Name "Security.Services"
    Start-IISSite -Path $erpSite -Port 49272 -Name "UltraERPService"
    Start-IISSite -Path $centralSite -Port 51086 -Name "CentralAdmin"
    Start-IISSite -Path $ultraErpSite -Port 51234 -Name "UltraERP Inventario"
}

Write-Host ""
Write-Host "Listo." -ForegroundColor Green
Write-Host "Security.Services: http://localhost:49273/SecurityServices.svc?wsdl"
Write-Host "UltraERPService:  http://localhost:49272/UltraERPService.svc?wsdl"
Write-Host "CentralAdmin:     http://localhost:51086/Inicio/Login"
Write-Host "UltraERP Inv.:    http://localhost:51234/Account/Login"
