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
    public string Device { get; set; }
    public string MessageType { get; set; }
}
```

- MessageType must be one of the following strings:
	-  "danalock"
	-  "camera"
	-  "lazybone_switch"
	-  "lazybone_dimmer"
	-  "wallmount_switch"

```csharp
public class CommandMessage : GatewayMessage
{
    public string CommandToken { get; set; }
}
```

- CommandToken: JWT (expires after 1 min), App must request new CommandToken every 1 minute or else the following commands do not succeed!



### DanaLock

Parameters: 

- Locked: true / false (if true: door will be locked, else unlocked)


```csharp
public class DanaLockMessage : CommandMessage
{
    public bool Locked { get; set; }
}
```


Examples:


**Lock door** DanaLock with name "danalock_front_door"


```json
{
	"Locked":true,
	"CommandToken":"secret_jwt",
	"Device":"danalock_front_door",
	"MessageType":"danalock"
}
```

**Unlock door** DanaLock with name "danalock_front_door"

```json
{
	"Locked":false,
	"CommandToken":"secret_jwt",
	"Device":"danalock_front_door",
	"MessageType":"danalock"
}
```

### LazyBone switch

Parameters: 

State: true / false (If true: sets lazybone to ON, else OFF)


```csharp
public class LazyBoneSwitchMessage : CommandMessage
{
    public bool State { get; set; }
}
```


Examples:


**Set lazybone ON** LazyBone with name "lazybone1"


```json
{
	"State":true,
	"CommandToken":"secret_jwt",
	"Device":"lazybone1",
	"MessageType":"lazybone_switch"
}
```

**Set lazybone OFF** LazyBone with name "lazybone1"

```json
{
	"State":false,
	"CommandToken":"secret_jwt",
	"Device":"lazybone1",
	"MessageType":"lazybone_switch"
}
```

### LazyBone Dimmer

Parameters:

- State: true / false. If true: switches dimmer ON, else OFF

- LightIntensity: **null** or **value between 0 and 255 (0 and 255 inclusive)**. If null, light intensity isn't changed. **0 is brightest, 255 is darkest**


```csharp
public class LazyBoneDimmerMessage : CommandMessage
{
    public bool State { get; set; }
    public short? LightIntensity { get; set; }
}
```


Examples:


**Set dimmer ON** LazyBone with name "lazybone2"


```json
{
	"State":true,
	"CommandToken":"secret_jwt",
	"Device":"lazybone2",
	"MessageType":"lazybone_dimmer"
}
```

**Set dimmer OFF** LazyBone with name "lazybone2"

```json
{
	"State":false,
	"CommandToken":"secret_jwt",
	"Device":"lazybone2",
	"MessageType":"lazybone_dimmer"
}
```

**Set dimmer ON and change light value**


```json
{
	"State":false,
	"LightIntensity":"150",
	"CommandToken":"secret_jwt",
	"Device":"lazybone2",
	"MessageType":"lazybone_dimmer"
}
```


### Wallmount switch (TZ36S)

Parameters: 

State: true / false (If true: sets wallmount to ON, else OFF)


```csharp
public class WallmountSwitchMessage : CommandMessage
{
    public bool State { get; set; }
}
```


Examples:


**Set wallmount ON** Wallmount switch with name "wallmount1"


```json
{
	"State":false,
	"CommandToken":"secret_jwt",
	"Device":"wallmount1",
	"MessageType":"wallmount_switch"
}
```

**Set wallmount OFF** Wallmount switch with name "wallmount1"

```json
{
	"State":false,
	"CommandToken":"secret_jwt",
	"Device":"wallmount1",
	"MessageType":"wallmount_switch"
}
```