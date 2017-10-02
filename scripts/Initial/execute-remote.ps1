Write-Host "Establishing PS Session to $deviceName..." 
$pstimeout = New-PSSessionoption -OperationTimeout (1000*60*5)
$session = New-PSSession -computer $ipAddress -Credential $cred -ErrorAction Stop -SessionOption $pstimeout

#Rename Device to Device Name
Write-Host "Renaming Device $ipAddress to $deviceName" 
# Invoke-Command -Session $session -FilePath .\Rename-Device.ps1 -ArgumentList $deviceName

#Set screen resolution
Write-Host "Setting screen resolution" 
# Invoke-Command -Session $session -FilePath .\screen-resolution.ps1

#Set time zone
Write-Host "Setting time zone" 
# Invoke-Command -Session $session -FilePath .\set-time-zone.ps1

$f="set-time-zone.ps1"
# $c=Get-Content $f
# Invoke-Command -session $session -script {param($filename,$contents) `
     # set-content -path $filename -value $contents} -argumentlist "C:\$f",$c
	 
# Invoke-Command -Session $session -FilePath .\create-time-zone-task.ps1

& ".\copy-drivers.ps1"

Write-Host "Installing drivers" 
Invoke-Command -Session $session -FilePath .\install-drivers.ps1

#reboot
Write-Host "Rebooting" 
# Invoke-Command -Session $session -FilePath ..\reboot-device.ps1