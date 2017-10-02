function Invoke-AuthenticatedWebRequest {
	param (
		[string] $uri,
		[string] $userName,
		[string] $password,
		[string] $method,
		[string] $contentType
	)
	
	$credentials = "$($userName):$($password)"
	$encodedCredentials = [System.Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes($credentials))
	$basicAuthValue = "Basic $encodedCredentials"
	$headers = @{
		Authorization = $basicAuthValue
	}

	Invoke-WebRequest -Uri $uri -Headers $headers -Method $method -ContentType $contentType -DisableKeepAlive
}

$deviceEndpoint = "$deviceName`:8080"
$apiRoot = "http://$deviceEndpoint/api"

Write-Host "Establishing PS Session to $deviceName..." 
$pstimeout = New-PSSessionoption -OperationTimeout (1000*60*5)
$session = New-PSSession -computer $ipAddress -Credential $cred -ErrorAction Stop -SessionOption $pstimeout

$retryLimit = 10
$retryCount = 1
$noError = $false
while ($noError -eq $false -and $retryCount -le $retryLimit)
{
#Check for a 500 error on /api/iot/iotonboarding/softapsettings call
    try{
        $responseSoftAP = Invoke-AuthenticatedWebRequest -uri "$apiRoot/iot/iotonboarding/softapsettings" -userName $username -password $password -method "GET" -contentType ""
    }
    catch [Exception]{
		echo $_.Exception|format-list -force
        Write-Host "There was an error... Retrying:" 
        Write-Host "Retry Count: $retryCount"
        
        # Restart Device
        # Write-Host "Restarting Device $deviceName" 
        Invoke-Command -Session $session -FilePath .\Reboot-Device.ps1

        # wait for reboot of the device
        Write-Host "Sleeping for 120 seconds while the device reboots..."
        Start-Sleep -Seconds 120

        # re-establish the session
        Write-Host "Done sleeping, resuming..."
        Write-Host "Re-establishing remote powershell session..."
        $session = New-PSSession -computer $ipAddress -Credential $cred -ErrorAction Stop -SessionOption $pstimeout
        
		$retryCount += 1

        Continue
    }
    $noError = $true
}
Write-Host "No More Errors!"

#This happens because we believe it provisions the SoftAP and needs a reboot for it to be set up correctly
Write-Host "Disabling SoftAP" 
$softApEnabled = [System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes("false"))
$softApSsid = [System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes("SoftAPSsid"))
$softApPassword = [System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes("p@ssw0rd"))
Invoke-AuthenticatedWebRequest -uri "$apiRoot/iot/iotonboarding/softapsettings?SoftAPEnabled=$softApEnabled&SoftApSsid=$softApSsid&SoftApPassword=$softApPassword" -userName $username -password $password -method "POST" -contentType ""
