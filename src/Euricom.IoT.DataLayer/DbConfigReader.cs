using Euricom.IoT.Common;

namespace Euricom.IoT.DataLayer
{
    public class DbConfigReader
    {
        public DbConfigReader()
        {

        }

        private LazyBoneConfig _lazyBoneConfig { get; set; }
        private DoorLockConfig _doorlockConfig { get; set; }
        private CameraConfig _cameraConfig { get; set; }

        // TODO read from database
        public DeviceConfigurations ReadConfiguration()
        {
            //Database.Instance.ReadConfig();
            var db = Database.Instance;

            _lazyBoneConfig = new LazyBoneConfig();
            _doorlockConfig = new DoorLockConfig();
            _cameraConfig = new CameraConfig();

            return new DeviceConfigurations()
            {
                LazyBoneConfig = _lazyBoneConfig,
                CameraConfig = _cameraConfig,
                DoorConfig = _doorlockConfig
            };
        }

        public class DeviceConfigurations
        {
            public LazyBoneConfig LazyBoneConfig { get; set; }
            public DoorLockConfig DoorConfig { get; set; }
            public CameraConfig CameraConfig { get; set; }

        }
    }
}
