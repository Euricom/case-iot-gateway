# case-iot-gateway

DanaLock

https://github.com/OpenZWave/open-zwave/wiki/Adding-Security-Devices-to-OZW

Step 1
Download the drivers here: http://aeotec.com/z-wave-usb-stick/1358-z-wave-drivers.html

step 2
Connect to tou you raspberry e.g. \\freyr\c$\Data\Users\Administrator\Documents
and unzip and copy the drivers

step 3
connect using powershell

[freyr]: PS C:\Data\Users\Administrator\Documents>

freyr]: PS C:\Data\Users\Administrator\Documents> dir


    Directory: C:\Data\Users\Administrator\Documents


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        8/24/2015   8:34 PM           8424 uzb.cat
-a----        8/24/2015   8:34 PM            710 uzb.inf

[freyr]: PS C:\Data\Users\Administrator\Documents> devcon dp_add .\uzb.inf

....

Driver package 'oem0.inf' added.
[freyr]: PS C:\Data\Users\Administrator\Documents>

[freyr]: PS C:\Data\Users\Administrator\Documents> devcon status usb*

...

USB\VID_0658&PID_0200\5&3753427A&0&4
    Name: USB Serial Device
    Driver is running.

...

http://jellebens.blogspot.be/2016/02/installing-z-wave-stick-gen-5-on.html