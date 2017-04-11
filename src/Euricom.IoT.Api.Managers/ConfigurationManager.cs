using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureDeviceManager;
using Euricom.IoT.Common;
using Euricom.IoT.Common.Notifications;
using Euricom.IoT.Common.Utilities;
using Euricom.IoT.DataLayer;
using Euricom.IoT.LazyBone;
using Euricom.IoT.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers
{
    public class ConfigurationManager : IConfigurationManager
    {
        public ConfigurationManager()
        {
        }

        public Settings GetConfigSettings()
        {
            return Database.Instance.GetConfigSettings();
        }

        public void SaveConfigSettings(Settings settings)
        {
            Database.Instance.SaveConfigSettings(settings);
        }
    }
}
