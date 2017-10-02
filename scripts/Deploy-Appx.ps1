param (
    [string][Parameter(Mandatory = $false)] $DeviceName,
    [string][Parameter(Mandatory = $false)] $TargetAppName,
    [string][Parameter(Mandatory = $false)] $TargetAppPath,
    [string][Parameter(Mandatory = $false)] $DeviceUsername,
    [string][Parameter(Mandatory = $false)] $DevicePassword
)

$ErrorActionPreference = "Stop"
Add-Type -Path "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.Net.Http.dll"

function Upload-File {
    param(
        [System.IO.FileSystemInfo]$Path,
        [string]$Uri
    )

    $retryLimit = 10
    $retryCount = 0
    $sleepTime = 1
    $webRequestSuccessful = $false

    while ($webRequestSuccessful -eq $false -and $retryCount -lt $retryLimit) {
        try {
            Write-Host "Sending $Path.Name to $Uri..."

            $boundaryId = [System.Guid]::NewGuid().ToString()
            $wc = New-Object System.Net.Http.HttpClient($null)
            $mfdc = New-Object System.Net.Http.MultipartFormDataContent($boundaryId)
            $fileBytes = [byte[]][System.IO.File]::ReadAllBytes($Path.FullName)
    
            $wc.DefaultRequestHeaders.Authorization = New-Object System.Net.Http.Headers.AuthenticationHeaderValue("Basic", [System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes("$DeviceUsername`:$DevicePassword")))
            $bytes = New-Object System.Net.Http.ByteArrayContent -ArgumentList @(,$fileBytes)
            $mfdc.Add($bytes, "`"$Path`"", "`"$Path`"")

            $mfdc.Headers.Remove("Content-Type") | Out-Null
            $mfdc.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=$boundaryId") | Out-Null

            $requestUrl = "$uriDeploymentRoot/package?package=$($Path.Name)"
            $response = $wc.PostAsync($requestUrl, $mfdc).Result 

            if ($response.StatusCode -eq "Accepted") {
                $webRequestSuccessful = $true
            }
            else {
                throw "$response.StatusCode: $response.ReasonPhrase"
            }
        }
        catch {
            Write-Host "Error uploading file $Path to $Uri, retrying in $sleepTime seconds"
            $webRequestSuccessful = $false
            Start-Sleep -s $sleepTime
            $sleepTime = $sleepTime * 2
            $retryCount += 1
        }
    }

    if ($webRequestSuccessful -eq $false) {
        Write-Error "Could not upload file to $Uri"
    }
}

function Get-PiCredentials {
    $securePass = ConvertTo-SecureString $DevicePassword -AsPlainText -Force
    return New-Object System.Management.Automation.PSCredential($DeviceUsername, $securePass)
}

function Invoke-AuthenticatedWebRequestWithRetries {
    param (
        [string][Parameter(Mandatory=$true)] $Uri,
        [string][Parameter(Mandatory=$true)] $Method,
        [bool][Parameter(Mandatory=$true)] $ClearContentType
    )

    $retryLimit = 10
    $retryCount = 0
    $sleepTime = 1
    $webRequestSuccessful = $false
    $response = $null

    while ($webRequestSuccessful -eq $false -and $retryCount -lt $retryLimit) {
        try {
            $response = Invoke-AuthenticatedWebRequest -Uri $Uri -Method $Method -ClearContentType $ClearContentType
            $webRequestSuccessful = $true
        }
        catch {
            Write-Host "Error encounted sending web request to $Uri, retrying in $sleepTime seconds"
            $webRequestSuccessful = $false
            Start-Sleep -s $sleepTime
            $sleepTime = $sleepTime * 2
            $retryCount += 1
        }
    }

    if ($webRequestSuccessful -eq $false) {
        Write-Error "Could not send successful web request to $Uri"
    }

    return $response
}

function Invoke-AuthenticatedWebRequest {
    param (
        [string][Parameter(Mandatory=$true)] $Uri,
        [string][Parameter(Mandatory=$true)] $Method,
        [bool][Parameter(Mandatory=$true)] $ClearContentType
    )

    #$creds = Get-PiCredentials
	$credentials = "$($DeviceUsername):$($DevicePassword)"
	$encodedCredentials = [System.Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes($credentials))
	$basicAuthValue = "Basic $encodedCredentials"
	$headers = @{
		Authorization = $basicAuthValue
	}

    $response = $null

    if($Method -eq ("Post" -or "Delete") -and $ClearContentType -eq $true){ #post with cleared content type
        $response = Invoke-WebRequest -Uri $Uri -Headers $headers -Method $Method -ContentType "" -DisableKeepAlive
    } 
	elseif ($Method -eq "Post") 
	{ 
        $response = Invoke-WebRequest -Uri $Uri -Headers $headers -Method $Method -DisableKeepAlive
    } 
	else 
	{ 
        $response = Invoke-WebRequest -Uri $Uri -Headers $headers -Method $Method -DisableKeepAlive
	}

    return $response
}

function Remove-Dependency {
    param( 
        [string] $PackageFullName 
    )

    if($PackageFullName -eq "") { 
        Write-Host "Dependency not found, moving on..."
        return
    }
    Write-Host "Removing $PackageFullName..."
    Write-Host "POSTing to $uriRoot/packagemanager/package?package=$PackageFullName"
    Invoke-AuthenticatedWebRequestWithRetries -Uri "$uriRoot/packagemanager/package?package=$PackageFullName" -Method Delete -ClearContentType $true
}

function FindAndRemove-Package {
    param(
        [string]$Package
    )
    if($Package -eq $null) { return }
    Write-Host "Looking for package named $Package..."
    $target = $apps.InstalledPackages | where { $_.PackageFullName -contains $Package }
    
    if($target -ne $null) 
    {
        Write-Host "Package found. Removing..."
        Remove-Dependency -PackageFullName $target.PackageFullName
    } else {
        Write-Host not found.
    }
}

function Get-InstalledApps {
    Write-Host Getting installed packages...
    $request = Invoke-AuthenticatedWebRequestWithRetries -Uri "$uriRoot/packagemanager/packages" -Method Get -ClearContentType $false

    try { 
        $json = ConvertFrom-Json $request -Verbose
        return $json
    }
    catch {
        $ErrorMessage = $_.Exception.Message
        Write-Error "$ErrorMessage"
        Write-Error "Could not get installed packages..."
    }
}

function Poll-SuccessfulDeployment {
    $status = Invoke-AuthenticatedWebRequestWithRetries -Uri "$uriDeploymentRoot/state" -Method Get -ClearContentType $false
    $statusCode = $status.StatusCode
    $timeoutLimit = 120
    $timer = 0
    while ($statusCode -eq "204" -and $timer -lt $timeoutLimit) {
        $timer += 1
        Start-Sleep -s 1
        $status = Invoke-AuthenticatedWebRequestWithRetries -Uri "$uriDeploymentRoot/state" -Method Get -ClearContentType $false
        $statusCode = $status.StatusCode
    }

    if ($timer -eq $timeoutLimit) {
        Write-Host "Timeout exceeded after $timeoutLimit seconds"
        return $false
    }
    Write-Host "Deployment successful after $timer second(s)"
    return $true
}


function Get-AppByName {
    param ([string]$Name)
    return $apps.InstalledPackages | where { $_.Name -match $Name }
}


$creds = Get-PiCredentials

$deviceEndpoint = "$DeviceName`:8080"
$apiRoot = "http://$deviceEndpoint/api"
$uriRoot = "http://$deviceEndpoint/api/appx"
$privateUriRoot = "http://$deviceEndpoint/api/iot/appx"
$uriDeploymentRoot = "$uriRoot/packagemanager" #deployment

#get package list
$apps = Get-InstalledApps
$targetApp = Get-AppByName -Name $TargetAppName

#uninstall target app, if exists
FindAndRemove-Package -Package $targetApp.PackageFullName

#install target + dependencies
$appxFile = Get-ChildItem -Path $TargetAppPath -Filter *.appx
Write-Host Found $appxFile.Name 

#dependencies
$dependencyFolder = Join-Path -Path $TargetAppPath -ChildPath "Dependencies/ARM"
Write-Host Checking $dependencyFolder for dependencies...
$dependencies = Get-ChildItem -Path $dependencyFolder
Write-Host Found $dependencies.Length dependencies. 

foreach($dependency in $dependencies) {
    Upload-File -Uri "$uriDeploymentRoot/dependency" -Path $dependency
    $successfulDeployment = Poll-SuccessfulDeployment
    if ($successfulDeployment -eq $false) { 
        Write-Error "Could not upload $dependency..." 
    }
}

# Upload Package
Upload-File -Uri "$uriDeploymentRoot/package" -Path $appxFile
$successfulDeployment = Poll-SuccessfulDeployment
if ($successfulDeployment -eq $false) { 
    Write-Error "Could not upload $appxFile..." 
}