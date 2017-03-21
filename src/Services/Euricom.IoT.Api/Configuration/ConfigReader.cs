using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Euricom.IoT.Api.Configuration
{
    public class ConfigReader
    {
        private LazyBoneConfig _lazyBoneConfig { get; set; }
        private DoorLockConfig _doorlockConfig { get; set; }
        private CameraConfig _cameraConfig { get; set; }

        public DeviceConfigurations ReadConfiguration()
        {
            //ReadXml();

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

        private void ReadXml()
        {
            //Load xml
            foreach (XElement settingsElement in XElement.Load("configuration.xml").Elements("settings"))
            {
                // Lazybone
                foreach (XElement lazyBoneSettings in settingsElement.Elements("lazybone"))
                {
                    _lazyBoneConfig.Host = lazyBoneSettings.Element("host").Value;
                    _lazyBoneConfig.Port = lazyBoneSettings.Element("port").Value;
                }

                //// Camera
                //foreach (XElement lazyBoneSettings in settingsElement.Elements("camera"))
                //{
                //    _lazyBoneConfig.Host = lazyBoneSettings.Element("host").Value;
                //    _lazyBoneConfig.Port = lazyBoneSettings.Element("port").Value;
                //}

                //// DanaLock
                //foreach (XElement lazyBoneSettings in settingsElement.Elements("danalock"))
                //{
                //    _lazyBoneConfig.Host = lazyBoneSettings.Element("host").Value;
                //    _lazyBoneConfig.Port = lazyBoneSettings.Element("port").Value;
                //}

            }
        }
    }
}
