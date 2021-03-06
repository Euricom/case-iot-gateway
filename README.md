# Euricom IoT Gateway

## Summary

This gateway is a raspberry pi(3) device which has [Win10 IoT Core](
https://developer.microsoft.com/en-us/windows/iot) installed.

The **purpose of the gateway** is:

- to manage and control IoT devices in the Euricom network.
- to monitor devices (example: what is current state of doorlock, light on/off, etc)
  and send those notifications to Azure IoT Hub
- to have a easy to use web interface
	- to make it easy to add more devices of the same type (by configuration , no code changes), edit devices, remove devices
	- test by simple click button to see whether device works	
	- show logging of events (device state changes, errors, debugging, ..)

**Important business requirement: the user must be at Euricom WIFI network to be able to send commands.** For example, it is not possible to switch on Euricom lights at home or open doorlock at another location than Euricom.**

We use **Azure IoT Hub**:

- for sending notifications to Iot Hub as device state changes happen
- receive messages from IoT Hub to process device changes

The currently used device hardware are:

- [DanaLock](https://danalock.com/)
- [LazyBone Wifi Switch](http://www.tinyosshop.com/index.php?route=product/product&product_id=657)
- [LazyBone Wifi Dimmer](http://www.tinyosshop.com/index.php?route=product/product&product_id=582)
- [TKB Switch TZ36S](http://products.z-wavealliance.org/products/1411)
- [MotionEyeOs](https://github.com/ccrisan/motioneyeos) Camera which is a raspberry pi device with motionEye


The gateway includes a website (Angular) to set configuration parameters and to add/edit/remove/test devices. It also shows logging.

Other sort of devices can be added/programmed if you can build it in UWP because Windows 10 IoT only supports UWP development.


## Gateway


**Important**

1) Every time raspberry pi loses power/reboots, set date and time  in raspberry using powershell. You can do this with the IoT core dashboard. Right click on the device and connect with powershell. Then type in username/password. Then **copy paste and change** date and time:

```
 Set-Date "Thursday, April 06, 2017 10:34:26 AM"
```
(after every reboot of raspberry)


2) If password is lost or is forgotten, there is the possibility to login with another code.


## IP addresses / Node IDs

See [Readme environment](README_Env.md)

## DanaLock

The DanaLock is controlled using the open source library called [OpenZWave](http://openzwave.com/). The Gateway uses the [Z-Stick GEn5 from Aeotec](https://aeotec.freshdesk.com/support/solutions/folders/6000146720)

**The following steps must be done ONLY ONCE on the Gateway (again if gateway is reinstalled or sd card corrupt):**

**Step 1**

1) Download the drivers here: http://aeotec.com/z-wave-usb-stick/1358-z-wave-drivers.html
2) Connect to your raspberry e.g. \\10.0.1.31\c$\Data\Users\Administrator\Documents
and unzip and copy the drivers
3) Connect using powershell
4) PS C:\Data\Users\Administrator\Documents>
5) PS C:\Data\Users\Administrator\Documents> dir


Directory: C:\Data\Users\Administrator\Documents

Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        8/24/2015   8:34 PM           8424 uzb.cat
-a----        8/24/2015   8:34 PM            710 uzb.inf

6) PS C:\Data\Users\Administrator\Documents> devcon dp_add .\uzb.inf

Driver package 'oem0.inf' added.
PS C:\Data\Users\Administrator\Documents>
PS C:\Data\Users\Administrator\Documents> devcon status usb*


USB\VID_0658&PID_0200\5&3753427A&0&4
    Name: USB Serial Device
    Driver is running.

...

[Installing Z-Wave Stick Gen5](http://jellebens.blogspot.be/2016/02/installing-z-wave-stick-gen-5-on.html)


**Step 2**

Add Z-Wave device (**add secure device** instead of **add device**).
See [Readme adding devices / removing devices from Z-Wave](README_ZWave.md)

If you have problems adding the DanaLock as secure device, try resetting DanaLock first by holding user button for 10 beeps. 

## LazyBone Switch

The LazyBone is controlled using a tcp telnet connection. See [product manual](http://www.tinyosshop.com/datasheet/LazyBone%20User%20Manual.pdf) 

The LazyBone switch cannot handle more than 1 connection at same time, so locking must be used to avoid connection exceptions.

Example code: see doc\LazyBone.zip


## Wallmount Switch


Add Z-Wave device (**add device**).
See [Readme adding devices / removing devices from Z-Wave](README_ZWave.md)


## Camera (MotionEyeOs)

The camera is not really controlled by the gateway. The camera only sends notifications to the gateway when motion is detected. It also uploads those motion frames/movies to the Euricom dropbox account (via the settings of motionEyeOS)
The camera must be linked with the Dropbox account via a access token.

1) Go to the camera configuration page

	![Configuring camera dropbox account](/doc/euricomseccamera_dropbox_accesstoken.png)

2) Click obtain key

3) Login with your dropbox account and allow this camera access to dropbox

	![Configuring camera dropbox account obtain key](/doc/dropbox_token.png)

4) Enter correct dropbox location: /Apps/EuricomIoT/camera1

### Settings

Open the camera settings (go to http://ip of your camera)

1) Under file storage: enter /Apps/EuricomIoT/camera1 for Dropbox
2) Generate a Dropbox token in MotionEye and also set it in gateway settings.


## REST API

The gateway has a REST inteface for controlling the IoT devices. This REST interface is not really used (only used when developing/testing if devices behave correctly).

## Technical architecture

**UWP**

Windows 10 IoT core uses UWP for development. So the whole solution uses UWP projects.
The startup project is the UI (Euricom.IoT.UI). 

See [Writing apps for Iot Core](https://developer.microsoft.com/en-us/windows/iot/docs/buildingappsforiotcore)

**Restup (webserver)**

The UI launches a webserver called Restup and currently listens for requests on /api/...
It also serves static content (the Gateway Administration Website).

See [Restup](https://github.com/tomkuijsten/restup)

**Gateway Administration Website**

The administration site is a basic Angular-CLI website with basic bootstrap layout/menu.

See [Angular CLI](https://github.com/angular/angular-cli)

**Database**

The settings are stored on the Raspberry PI SD card using DBreeze which is a C# NoSQL embedded database system.

See [Dbreeze](https://github.com/hhblaze/DBreeze)

**OpenZWave and UWP**

I used the UWP .NET wrapper of https://github.com/OpenZWave/openzwave-dotnet-uwp (the OpenZWave library is a C++ library). I included the full source code of OpenZWave and UWP wrapper in this repository. This makes it easy to debug if there are problems with initializing OpenZWave or to track bugs.

See [OpenZWave UWP wrapper](https://github.com/OpenZWave/openzwave-dotnet-uwp) and [OpenZWave](https://github.com/OpenZWave/open-zwave)

**Logging**

The OpenZWave is also enabled for logging. This means that a OZW_Log.txt will be created in the package where the app wil run. For example
\\10.0.1.31\c$\Data\Users\DefaultAccount\AppData\Local\Packages\12636daa-e12e-413c-a6c6-037e08210458_kjj12ry09zpaj\LocalState\
This OZW_Log will be regenerated/overwritten every time the app restarts. This can be changed in the options.xml of OpenZWave.

Logging is done with [Serilog](https://serilog.net/) and [Rolling file sink](https://github.com/serilog/serilog-sinks-rollingfile). The sink is configured to write a log once per day in the application package folder. Example:

\\10.0.1.31\c$\Data\Users\DefaultAccount\AppData\Local\Packages\12636daa-e12e-413c-a6c6-037e08210458_kjj12ry09zpaj\LocalState\logs\

**The sink also has a default setting that limits the file size to 1GB.**


**Azure Iot Hub**

When device state changes happen, notifications are sent to the Azure IoT hub. And monitoring / loop happens to monitor devices.
These notifications/messages are also stored in a Azure SQL database.

Technical diagram:

Page 1:

![Technical diagram page 1](/doc/GW-1.jpg)

Page 2:

![Technical diagram page 2](/doc/GW-2.jpg)


The diagram explained in words:

Definition of

* AccessToken: JWT token of AD (after authentication)
* CommandToken: JWT token for sending commands to the gateway

Page1:
The App (front end app, which is a mobile app) authenticates with Active Directory (requests an AccessToken). The AccessToken is sent back to the app if successfull.

Page2:
The APP sends a request for a CommandToken by supplying the AccessToken. The gateway generates the CommandToken and sends it back. This CommandToken has a very short expiry because the business requirement was to only allow commands on the Euricom WIFI network. This means the App must send regular requests to the Gateway directly to be able to send commands. So the gateway checks the expiry! (This is also the reason why it is important the time is set correctly in the gateway, otherwise JWT expiry errors will happen)

Page 1 & page2:
The gateway implements a monitoring system / loop that continuously polls devices at a specified interval (can be changed via the device configuration). If the polling of the device was succesfull it sends ACKNOWLEDGEMENT , else a REJECT to Azure IoT Hub. 


## Development: (Windows and Visual Studio)

There are 2 development solutions

- Development of the API (must use Visual Studio) 
   => The solution can be found under **src\Euricom.IoT.sln**

- Development of the Gateway Administration Website (any editor, I used VS Code)
   => The website can be found under **src\UI\iot-gateway-app**


### API development

For API development: the method I used is basically every time API changed, deploy to the gateway over WIFI/LAN using Visual Studio.

Install Windows 10 IoT core Dashboard watcher from

[Windows 10 IoT Core Dashboard](https://developer.microsoft.com/en-us/windows/iot/docs/iotdashboard)

Use Visual Studio (2017) for debugging/deployment. Don't forget to set the IP address of the remote machine (gateway) in project properties of both UI and API project!

Set startup project to Euricom.IoT.UI (Euricom.IoT.API will be deployed too)

*Other possible development environment could be 
- setting up Windows 10 IoT Core inside a Virtual Machine - but I could not get this VM to work correctly on my laptop* (bridged network not working, could not connect from host to vm).. If you want to try this: see [Setting up Windows 10 IoT Core VM](https://www.newventuresoftware.com/blog/running-windows-10-iot-core-in-a-virtual-machine)

### Gateway Administration website development

Change directory to src\UI\iot-gateway-app, then install all dependencies with command

**Install all dependencies (node_modules)**

```
yarn
```

**Starting website**

```
yarn start
```


**Build website for deployment**

```
yarn build
```

Then you can use the generated dist folder (which contains the index.html and js) and copy to visual studio project.


## Deployment:

To deploy the gateway API , use Visual Studio.

To deploy the web configuration site, you must manually copy the files from
src\UI\iot-gateway-app to src\Euricom.IoT.UI.WebAdministration, then deploy the gateway API

To make a permanent deploy (without VS), set the app as default startup app via the Windows 10 IoT Core Dashboard.

Open gateway administration website at http://ip:8800

For example: [http://10.0.1.31:8800](http://10.0.1.31:8800)
  

**Important**

Every time raspberry pi loses power/reboots, set date and time  in raspberry using powershell. You can do this with the IoT core dashboard. Right click on the device and connect with powershell. Then type in username/password. Then **copy paste and change** date and time:

```
 Set-Date "Thursday, April 06, 2017 10:34:26 AM"
```
(after every reboot of raspberry)



## IoT messages

See [Readme IoT Message format](README_IoT.md)

## References

https://developer.microsoft.com/en-us/windows/iot/docs/buildingappsforiotcore

http://jellebens.blogspot.be/2016/02/installing-z-wave-stick-gen-5-on.html

https://developer.microsoft.com/en-us/windows/iot/docs/iotdashboard

https://github.com/OpenZWave/open-zwave

http://wiki.micasaverde.com/index.php/ZWave_Command_Classes

https://github.com/OpenZWave/openzwave-dotnet-uwp

https://github.com/tomkuijsten/restup

https://github.com/angular/angular-cli

https://github.com/hhblaze/DBreeze

https://github.com/serilog/serilog-sinks-rollingfile

https://github.com/Euricom/case-iot-hub (for examples of Azure IoT)

https://www.newventuresoftware.com/blog/running-windows-10-iot-core-in-a-virtual-machine
