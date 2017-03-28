using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace AzureDeviceManager
{
    public class DeviceManager
    {
        private RegistryManager _registryManager;

        public DeviceManager()
        {
            string connectionString = GetConnectionString().Result;
            _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }

        public async Task<IEnumerable<Device>> GetDevicesAsync()
        {
            var devices = await _registryManager.GetDevicesAsync(int.MaxValue);
            return devices;
        }

        public async Task<string> AddDeviceAsync(string deviceName)
        {
            Device device;
            try
            {
                device = await _registryManager.AddDeviceAsync(new Device(deviceName));
                return device.Authentication.SymmetricKey.PrimaryKey;
            }
            catch (DeviceAlreadyExistsException)
            {
                throw;
            }
        }

        private async Task<string> GetConnectionString()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(
                                 new Uri(String.Format("ms-appx:///Assets/Configuration/secrets.xml")));

            XmlDocument xmlSecrets = await XmlDocument.LoadFromFileAsync(file);

            var children = xmlSecrets.ChildNodes;

            return "";
        }


    }
}
