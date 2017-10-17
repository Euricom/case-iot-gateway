using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureBlobStorage;
using Euricom.IoT.Common;
using Euricom.IoT.DataLayer;
using Euricom.IoT.Messaging;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Messages;
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
        private readonly Database _database;
        private readonly IAzureBlobStorageManager _azureBlobStorageManager;

        public CameraManager(Database database, IAzureBlobStorageManager azureBlobStorageManager)
        {
            _database = database;
            _azureBlobStorageManager = azureBlobStorageManager;
        }

        public Task<IEnumerable<Camera>> GetAll()
        {
            var cameras = _database.GetCameras();
            return Task.FromResult(cameras.AsEnumerable());
        }

        public Task<Camera> GetByDeviceId(string deviceId)
        {
            var json = _database.GetValue(Constants.DBREEZE_TABLE_CAMERAS, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Camera>(json));
        }

        private string GetDeviceId(string deviceName)
        {
            var device = _database.GetCameras().FirstOrDefault(x => x.Name == deviceName);
            if (device == null)
            {
                throw new Exception($"Could not find deviceName: {deviceName}");
            }
            return device.DeviceId;
        }

        public Task<Camera> GetByDeviceName(string deviceName)
        {
            var deviceId = GetDeviceId(deviceName);
            var json = _database.GetValue(Constants.DBREEZE_TABLE_CAMERAS, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Camera>(json));
        }

        public async Task<Camera> Add(Camera camera)
        {
            // Generate Device Id
            camera.DeviceId = Guid.NewGuid().ToString();

            //Convert to json
            var json = JsonConvert.SerializeObject(camera);

            //Save to database
            _database.SetValue(Constants.DBREEZE_TABLE_CAMERAS, camera.DeviceId, json);

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

            _database.SetValue(Constants.DBREEZE_TABLE_CAMERAS, camera.DeviceId, json);

            return await GetByDeviceId(camera.DeviceId);
        }

        public Task Remove(string deviceName)
        {
            // Remove device from  database
            var deviceId = GetDeviceId(deviceName);
            _database.RemoveDevice(deviceId);

            return Task.FromResult(0);
        }

        public void Notify(string deviceId, string url, string timestamp, int frameNumber, int eventNumber)
        {
            var settings = _database.GetConfigSettings();
            var config = _database.GetCameraConfig(deviceId);
            if (config.Enabled)
            {
                var notification = new CameraMotionMessage
                {
                    Gateway = "IoTGateway",
                    Device = config.Name,
                    CommandToken = null,
                    MessageType = MessageTypes.Camera,
                    FilePath = url,
                    EventNumber = eventNumber,
                    FrameNumber = frameNumber,
                };

                // Publish to IoT Hub
                PublishMotionEvent(settings, config.Name, config.DeviceId, notification);
            }
        }

        public async void UploadFilesToBlobStorage(string path, Dictionary<string, byte[]> files)
        {
            foreach (var file in files)
            {
                using (MemoryStream ms = new MemoryStream(file.Value))
                {
                    await _azureBlobStorageManager.PostImage(path, file.Key, ms);
                }
            }
        }

        public async Task<bool> TestConnection(string deviceId)
        {
            try
            {
                var config = _database.GetCameraConfig(deviceId);

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

        private void PublishMotionEvent(Settings settings, string deviceName, string deviceKey, CameraMotionMessage notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            new MqttMessagePublisher(settings, deviceName, deviceKey).Publish(json);
        }
    }
}
