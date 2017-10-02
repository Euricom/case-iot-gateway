# Trust the remote pi and connect to it
Write-Host "Configuring WinRM TrustedHosts... " 
$trustedHosts = Get-Item WSMan:\localhost\Client\TrustedHosts
if ($trustedHosts.Value -eq "") {
	Set-Item WSMan:\localhost\Client\TrustedHosts -Value "$ipAddress,$($deviceName)" -Force
}
else {
	if (-not $trustedHosts.Value.Contains($ipAddress)) {
		Set-Item WSMan:\localhost\Client\TrustedHosts -Value "$($trustedHosts.Value),$($ipAddress)" -Force
	}
	
	if (-not $trustedHosts.Value.Contains($deviceName)) {
		Set-Item WSMan:\localhost\Client\TrustedHosts -Value "$($trustedHosts.Value),$($deviceName)" -Force
	}
}
