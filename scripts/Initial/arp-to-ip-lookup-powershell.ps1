# Gets the IP address of a computer via the MAC address. This will only work on LAN
# segments. By default we'll scan the ARP table, but then defer to an IP scan to
# as needed.
#
# Usage: $returnIpAddress = GetIPFromMAC "12-43-de-52-a9-99" "192.168.10."
# Pass in the mac address with dashes (-) as the first parameter
# Pass in an IP address with the last number missing, be sure to include the trailing "."
# Be sure to check the value if $returnIpAddress -eq $null to see if a value was actually returned
#
function GetIPFromMAC {
  Param (
      [string] $macAddress, 
      [string] $networkSegment
  )
  Write-Host "Searching for $macAddress in network segment $networkSegment..."
  $ip = arp -a | Select-String $macAddress |% { $_.ToString().Trim().Split(" ")[0] }
  if ($ip -eq $null) {
	Write-Host "$macAddress not found in ARP table, deep scan starting..."
      for ($i = 1; $i -lt 255; $i++) {
          ping "$networkSegment$i" -n 1 -l 1 -4 -w 500 | Out-Null
          $ip = arp -a "$networkSegment$i" | Select-String $macAddress |% { $_.ToString().Trim().Split(" ")[0] }
          if ($ip -ne $null) {
		      	Write-Host "Found $macAddress during deep scan, IP: $ip"
            break
          }
      } 
  }
  return $ip
}

#MacAddress Device Rename
Write-Host "Getting Ip-Address from MAC Address" 
$ipAddress = GetIPFromMac -macAddress "B8-27-EB-B3-5F-A6" -networkSegment "192.168.40."
Write-Host "Found IP Address: $ipAddress" 
$global:ipAddress=$ipAddress