# Euricom IoT Gateway IoT

## Requesting command token

Must be on WIFI:

POST http://10.0.1.31:8800/api/security/requestcommandtoken

Headers {"Content-Type":"application/json"}

Body: include the JWT accesstoken

```json
{
  "AccesToken": "..."
}
```

Returns a JWT (example)
```json
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWV9.TJVA95OrM7E2cBab30RMHrHDcEfxjoYZgeFONFh7HgQ
```
(this jwt is just a sample jwt from https://jwt.io/)


## Euricom IoT Gateway IoT Hub messages

**Important:** 
**Publish these message to Device ID: IoTGateway**

### Gateway message

```csharp
public class GatewayMessage
{
    public string CommandToken { get; set; }
    public string DeviceType { get; set; }
    public string Message { get; set; }
}
```

- CommandToken: JWT (expires after 1 min), App must request new CommandToken every 1 minute or else the following commands do not succeed!

- DeviceType must be one of the following strings:
	-  "danalock"
	-  "camera"
	-  "lazybone_switch"
	-  "lazybone_dimmer"
	-  "wallmount_swith"

- Message contains the device command message (open door, close door, switch lazybone on, etc)

### DanaLock

Parameters: 

- Locked: true / false (if true: door will be locked, else unlocked)


```csharp
public class DanaLockMessage
{
    public string Name { get; set; }
    public bool Locked { get; set; }
}
```


Examples:


**Lock door** DanaLock with name "danalock_front_door"


```json
{  
   "CommandToken": "command_token_jwt", //Replace with the correct jwt
   "DeviceType":"danalock",
   "Message": { "Name": "danalock_front_door", "Locked": "true" }
}
```

**Unlock door** DanaLock with name "danalock_front_door"

```json
{  
   "CommandToken": "jwt", //Replace with the correct jwt
   "DeviceType": "danalock",
   "Message": { "Name": "danalock_front_door", "Locked": "false" }
}
```

### LazyBone switch

Parameters: 

State: true / false (If true: sets lazybone to ON, else OFF)


```csharp
public class LazyBoneSwitchMessage
{
    public string Name { get; set; }
    public bool State { get; set; }
}
```


Examples:


**Set lazybone ON** LazyBone with name "lazybone1"


```json
{  
   "CommandToken":"jwt", //Replace with the correct jwt
   "DeviceType":"lazybone_switch",
   "Message": { "Name": "lazybone1", "State": "true" }
}
```

**Set lazybone OFF** LazyBone with name "lazybone1"

```json
{  
   "CommandToken": "jwt", //Replace with the correct jwt
   "DeviceType": "lazybone_switch",
   "Message": { "Name": "lazybone1", "State": "false" }
}
```

### LazyBone Dimmer

Parameters:

- State: true / false. If true: switches dimmer ON, else OFF

- LightIntensity: **null** or **value between 0 and 255 (0 and 255 inclusive)**. If null, light intensity isn't changed. **0 is brightest, 255 is darkest**


```csharp
public class LazyBoneDimmerMessage
{
    public string Name { get; set; }
    public bool State { get; set; }
    public short? LightIntensity { get; set; }
}
```


Examples:


**Set dimmer ON** LazyBone with name "lazybone2"


```json
{  
   "CommandToken": "jwt", //Replace with the correct jwt
   "DeviceType":"lazybone_dimmer",
   "Message": { "Name": "lazybone1", "State": "" }
}
```

**Set dimmer OFF** LazyBone with name "lazybone2"

```json
{  
   "CommandToken": "jwt", //Replace with the correct jwt
   "DeviceType":"lazybone_dimmer",
   "Message": { "Name": "lazybone2", "State": "false" }
}
```

**Set dimmer ON and change light value**


```json
{  
   "CommandToken": "jwt", //**Replace with the correct jwt**
   "DeviceType":"lazybone_dimmer",
   "Message": { "Name": "lazybone2", "State": "true", "LightIntensity": "180" }
}
```


### Wallmount switch (TZ36S)

Parameters: 

State: true / false (If true: sets lazybone to ON, else OFF)


```csharp
public class WallmountSwitchMessage
{
    public string Name { get; set; }
    public bool State { get; set; }
}
```


Examples:


**Set wallmount ON** Wallmount switch with name "wallmount1"


```json
{  
   "CommandToken":"jwt", //**Replace with the correct jwt**
   "DeviceType":"lazybone_switch",
   "Message": { "Name": "wallmount1", "State": "true" }
}
```

**Set wallmount OFF** Wallmount switch with name "wallmount1"

```json
{  
   "CommandToken": "jwt", //**Replace with the correct jwt**
   "DeviceType": "lazybone_switch",
   "Message": { "Name": "wallmount1", "State": "false" }
}
```