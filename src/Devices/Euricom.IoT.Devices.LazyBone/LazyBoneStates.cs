namespace Euricom.IoT.Devices.LazyBone
{
    public static class LazyBoneStates
    {
        // Get lamp states - sends 2 bytes back to the controller: 'on/off state'+'light value'  
        // on/off state: 0-OFF, 1-ON
        // light value:  0x00-0xFF,   0x00- Brightest, 0xFF- Darkest
        public static readonly byte[] CommandGetLampStates = { 0x5B, 0x00, 0x0D };

        // 65 00 0D
        public static readonly byte[] CommandTurnOnLamp = { 0x65, 0x00, 0x0D };

        // 6F 00 0D
        public static readonly byte[] CommandTurnOffLamp = { 0x6F, 0x00, 0x0D };
    }
}