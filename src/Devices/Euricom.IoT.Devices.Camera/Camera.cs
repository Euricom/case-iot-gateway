using System.IO;
using System.Threading.Tasks;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Models;

namespace Euricom.IoT.Devices.Camera
{
    public class Camera : Device
    {
        // EF
        private Camera() { }

        public Camera(string deviceId, string primaryKey, string name, string motionEyeIdentifier, bool enabled, string address, string dropboxPath, int pollingTime)
            : base(deviceId, primaryKey, HardwareType.Camera)
        {
            Name = name;
            Enabled = enabled;
            Address = address;
            DropboxPath = dropboxPath;
            PollingTime = pollingTime;
            MotionEyeIdentifier = motionEyeIdentifier;
        }

        public string MotionEyeIdentifier { get; protected set; }
        public string Address { get; protected set; }
        public string DropboxPath { get; protected set; }
        public int PollingTime { get; protected set; }

        public void Update(string name,string motionEyeIdentifier, bool enabled, string address, string dropboxPath, int pollingTime)
        {
            Name = name;
            MotionEyeIdentifier = motionEyeIdentifier;
            Enabled = enabled;
            Address = address;
            DropboxPath = dropboxPath;
            PollingTime = pollingTime;
        }

        #region Functionality

        public Task<bool> TestConnection(IHttpService service)
        {
            EnforceEnabled();

            return service.TestConnection(Address, "motionEye");
        }

        public async Task<Stream> GetPicture(IHttpService httpService, string fileName)
        {
            EnforceEnabled();

            return await httpService.GetFile($"{Address}/picture/{MotionEyeIdentifier}/preview/{fileName}/");
        }

        public async Task<Stream> GetPicture(IHttpService httpService)
        {
            EnforceEnabled();
            
            return await httpService.GetFile($"{Address}/picture/{MotionEyeIdentifier}/current/");
        }

        #endregion
    }
}
