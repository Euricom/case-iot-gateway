using Euricom.IoT.Common.Secrets;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace Euricom.IoT.AzureDeviceManager
{
    public class AzureDeviceManager : IAzureDeviceManager
    {
        private RegistryManager _registryManager;

        public AzureDeviceManager()
        {
            string connectionString = GetConnectionString();
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
            catch (Exception)
            {
                throw;
            }
        }

        private string GetConnectionString()
        {
            return Secrets.AZURE_IOT_HUB_CONNECTIONSTRING;

            //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(
            //                     new Uri(String.Format("ms-appx:///Assets/Configuration/secrets.xml")));

            //XmlDocument xmlSecrets = await XmlDocument.LoadFromFileAsync(file);

            //var children = xmlSecrets.ChildNodes;

            //return "";
        }


    }
}
