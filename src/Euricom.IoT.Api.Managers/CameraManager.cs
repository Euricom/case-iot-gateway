using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureBlobStorage;
using Euricom.IoT.AzureDeviceManager;
using Euricom.IoT.Common;
using Euricom.IoT.DataLayer;
using Euricom.IoT.Messaging;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Manager
{
    public class CameraManager : ICameraManager
    {
        private readonly IAzureBlobStorageManager _azureBlobStorage;
        private readonly IAzureDeviceManager _azureDeviceManager;

        public CameraManager()
        {
            var settings = Database.Instance.GetConfigSettings();
            _azureBlobStorage = new IoT.AzureBlobStorage.AzureBlobStorageManager();
            _azureDeviceManager = new AzureDeviceManager.AzureDeviceManager(settings);
        }

        public Task<IEnumerable<Camera>> GetAll()
        {
            var cameras = Database.Instance.GetCameras();
            return Task.FromResult(cameras.AsEnumerable());
        }

        public Task<Camera> GetByDeviceId(string deviceId)
        {
            var json = Database.Instance.GetValue(Constants.DBREEZE_TABLE_CAMERAS, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Camera>(json));
        }

        public Task<Camera> GetByDeviceName(string deviceName)
        {
            var deviceId = new HardwareManager().GetDeviceId(deviceName);
            var json = Database.Instance.GetValue(Constants.DBREEZE_TABLE_CAMERAS, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Camera>(json));
        }

        public async Task<Camera> Add(Camera camera)
        {
            // Generate Device Id
            camera.DeviceId = Guid.NewGuid().ToString();

            //Convert to json
            var json = JsonConvert.SerializeObject(camera);

            //Save to database
            Database.Instance.SetValue(Constants.DBREEZE_TABLE_CAMERAS, camera.DeviceId, json);

            return await GetByDeviceId(camera.DeviceId);
        }

        public async Task<Camera> Edit(Camera camera)
        {
            if (camera == null)
            {
                throw new ArgumentNullException("camera");
            }
            else if (String.IsNullOrEmpty(camera.DeviceId))
            {
                throw new ArgumentException("camera.DeviceId");
            }

            var json = JsonConvert.SerializeObject(camera);

            Database.Instance.SetValue(Constants.DBREEZE_TABLE_CAMERAS, camera.DeviceId, json);

            return await GetByDeviceId(camera.DeviceId);
        }

        public async Task Remove(string deviceName)
        {
            // Remove device from  database
            var deviceId = new HardwareManager().GetDeviceId(deviceName);
            Database.Instance.RemoveDevice(deviceId);
        }

        public void Notify(string deviceId, string url, string timestamp, int frameNumber, int eventNumber)
        {
            var settings = Database.Instance.GetConfigSettings();
            var config = Database.Instance.GetCameraConfig(deviceId);
            if (config.Enabled)
            {
                var notification = new CameraNotification
                {
                    FilePath = url,
                    EventNumber = eventNumber,
                    FrameNumber = frameNumber,
                    Timestamp = timestamp,
                    DeviceKey = config.DeviceId,
                };

                // Publish to IoT Hub
                // PublishMotionEvent(settings, config.Name, config.DeviceId, notification);
            }
        }

        public async void UploadFilesToBlobStorage(string path, Dictionary<string, byte[]> files)
        {
            foreach (var file in files)
            {
                using (MemoryStream ms = new MemoryStream(file.Value))
                {
                    await _azureBlobStorage.PostImage(path, file.Key, ms);
                }
            }
        }

        public async Task<bool> TestConnection(string deviceId)
        {
            try
            {
                var config = DataLayer.Database.Instance.GetCameraConfig(deviceId);

                HttpWebRequest request = (HttpWebRequest)GetAddress(config);
                request.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Headers["server"].Contains("motionEye"))
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception("A request to the server succeeded, but couldn't determine if server is motionEye");
                    }
                }
                else
                {
                    throw new Exception($"Could not get a valid response from {request.RequestUri}");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static WebRequest GetAddress(Camera config)
        {
            if (String.IsNullOrEmpty(config.Address))
            {
                throw new ArgumentNullException("config.Address");
            }
            else
            {
                if (config.Address.Contains("http://"))
                {
                    return WebRequest.Create(config.Address);
                }
                return WebRequest.Create("http://" + config.Address);
            }
        }

        private void PublishMotionEvent(Settings settings, string deviceName, string deviceKey, CameraNotification notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            new MqttMessagePublisher(settings, deviceName, deviceKey).Publish(json);
        }
    }
}
