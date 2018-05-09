using System.Threading.Tasks;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Models;

namespace Euricom.IoT.Devices.Camera
{
    public class Camera : Device
    {
        // EF
        private Camera() { }

        public Camera(string deviceId, string primaryKey, string name, bool enabled, string address, string dropboxPath, int pollingTime, int maximumDaysDropbox, double maximumStorageDropbox, int maximumDaysAzureBlobStorage)
            : base(deviceId, primaryKey, HardwareType.Camera)
        {
            Name = name;
            Enabled = enabled;
            Address = address;
            DropboxPath = dropboxPath;
            PollingTime = pollingTime;
            MaximumDaysDropbox = maximumDaysDropbox;
            MaximumStorageDropbox = maximumStorageDropbox;
            MaximumDaysAzureBlobStorage = maximumDaysAzureBlobStorage;
        }

        public string Address { get; protected set; }
        public string DropboxPath { get; protected set; }
        public int PollingTime { get; protected set; }
        public int MaximumDaysDropbox { get; protected set; }
        public double MaximumStorageDropbox { get; protected set; }
        public int MaximumDaysAzureBlobStorage { get; protected set; }

        public void Update(string name, bool enabled, string address, string dropboxPath, int pollingTime,
            int maximumDaysDropbox, double maximumStorageDropbox, int maximumDaysAzureBlobStorage)
        {
            Name = name;
            Enabled = enabled;
            Address = address;
            DropboxPath = dropboxPath;
            PollingTime = pollingTime;
            MaximumDaysDropbox = maximumDaysDropbox;
            MaximumStorageDropbox = maximumStorageDropbox;
            MaximumDaysAzureBlobStorage = maximumDaysAzureBlobStorage;
        }

        #region Functionality

        public Task<bool> TestConnection(IHttpService service)
        {
            EnforceEnabled();

            return service.TestConnection(Address, "motionEye");
        }

        #endregion
    }
}
