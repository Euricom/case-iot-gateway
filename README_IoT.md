# Euricom IoT Gateway IoT

## Requesting command token

```json
{
  "AccesToken": "..."
}
```


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


**Lock** DanaLock with name "danalock_front_door"


```json
{  
   "CommandToken": "command_token_jwt", //**Replace with the correct jwt**
   "DeviceType":"danalock",
   "Message": { "Name": "danalock_front_door", "Locked": "true" }
}
```

**Unlock** DanaLock with name "danalock_front_door"

```json
{  
   "CommandToken": "jwt", //**Replace with the correct jwt**
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


**On** LazyBone with name "lazybone1"


```json
{  
   "CommandToken":"jwt", //**Replace with the correct jwt**
   "DeviceType":"lazybone_switch",
   "Message": { "Name": "lazybone1", "State": "true" }
}
```

**Off** LazyBone with name "lazybone1"

```json
{  
   "CommandToken": "jwt", //**Replace with the correct jwt**
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


**Set dimmer on** LazyBone with name "lazybone2"


```json
{  
   "CommandToken": "jwt", //**Replace with the correct jwt**
   "DeviceType":"lazybone_dimmer",
   "Message": { "Name": "lazybone1", "State": "" }
}
```

**Set dimmer off** LazyBone with name "lazybone2"

```json
{  
   "CommandToken": "jwt", //**Replace with the correct jwt**
   "DeviceType":"lazybone_dimmer",
   "Message": { "Name": "lazybone1", "State": "false" }
}
```

**Set dimmer on and change light value**


```json
{  
   "CommandToken": "jwt", //**Replace with the correct jwt**
   "DeviceType":"lazybone_dimmer",
   "Message": { "Name": "lazybone1", "State": "true", "LightIntensity": "180" }
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


**On** Wallmount switch with name "wallmount1"


```json
{  
   "CommandToken":"jwt", //**Replace with the correct jwt**
   "DeviceType":"lazybone_switch",
   "Message": { "Name": "wallmount1", "State": "true" }
}
```

**Off** Wallmount switch with name "lazybone1"

```json
{  
   "CommandToken": "jwt", //**Replace with the correct jwt**
   "DeviceType": "lazybone_switch",
   "Message": { "Name": "wallmount1", "State": "false" }
}
```