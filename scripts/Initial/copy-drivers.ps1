Expand-Archive ..\..\drivers\Z-Stick_Gen5_Drivers.zip -DestinationPath c:\temp\Z-Stick_Gen5_Drivers\ -F

$sourcePath = "c:\temp\Z-Stick_Gen5_Drivers"
$destPath = "c:\Data\Users\$username\Documents"

Copy-Item -ToSession $session -Path $sourcePath -Destination $destPath -Recurse -Force