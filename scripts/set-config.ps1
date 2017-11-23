$global:deviceName ="euricom-iot"
$global:macAddress = "B8-27-EB-B3-5F-A6" 
$global:networkSegment = "192.168.1.2"

$global:username = "administrator"
$global:password = "p@ssw0rd"
$securePassword = ConvertTo-SecureString $password -AsPlainText -Force

$global:cred = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $username, $securePassword
