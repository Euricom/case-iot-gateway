using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureBlobStorage;
using Euricom.IoT.AzureDeviceManager;
using Euricom.IoT.Common;
using Euricom.IoT.Common.Notifications;
using Euricom.IoT.DataLayer;
using Euricom.IoT.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Manager
{
    public class CameraManager : ICameraManager
    {
        private readonly IAzureBlobStorageManager _azureBlobStorage;
        private readonly IAzureDeviceManager _azureDeviceManager;

        public CameraManager()
        {
            _azureBlobStorage = new IoT.AzureBlobStorage.AzureBlobStorageManager();
            _azureDeviceManager = new AzureDeviceManager.AzureDeviceManager();
        }

        public Task<IEnumerable<Camera>> GetAll()
        {
            var cameras = Database.Instance.GetCameras();
            return Task.FromResult(cameras.AsEnumerable());
        }

        public Task<Camera> Get(string deviceId)
        {
            var json = Database.Instance.GetValue(DatabaseTableNames.DBREEZE_TABLE_CAMERAS, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Camera>(json));
        }

        public async Task<Camera> Add(Camera camera)
        {
            //Add device to Azure Device IoT
            //var deviceId = _azureDeviceManager.AddDeviceAsync(camera.Name).Result;
            camera.DeviceId = Guid.NewGuid().ToString();

            //Convert to json
            var json = JsonConvert.SerializeObject(camera);

            //Save to database
            Database.Instance.SetValue(DatabaseTableNames.DBREEZE_TABLE_CAMERAS, camera.DeviceId, json);

            return await Get(camera.DeviceId);
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

            Database.Instance.SetValue(DatabaseTableNames.DBREEZE_TABLE_CAMERAS, camera.DeviceId, json);

            return await Get(camera.DeviceId);
        }

        public async Task<bool> Remove(string deviceId)
        {
            try
            {
                // Remove device from Azure
                await _azureDeviceManager.RemoveDeviceAsync(deviceId);

                // Remove device from  database
                Database.Instance.RemoveDevice(deviceId);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Notify(string deviceId, string url, string timestamp, int frameNumber, int eventNumber)
        {
            var config = DataLayer.Database.Instance.GetCameraConfig(deviceId);

            //var imageBytes = GetMotionImage(url);
            var notification = new CameraNotification
            {
                FilePath = url,
                EventNumber = eventNumber,
                FrameNumber = frameNumber,
                Timestamp = timestamp,
                DeviceKey = config.DeviceId,
            };

            // Publish to IoT Hub
            PublishMotionEvent(config.DeviceId, notification);
        }

        public async void UploadFilesToBlobStorage(Dictionary<string, byte[]> files)
        {
            foreach (var file in files)
            {
                using (MemoryStream ms = new MemoryStream(file.Value))
                {
                    await _azureBlobStorage.PostImage(file.Key, ms);
                }
            }
        }

        public bool TestConnection(string deviceId)
        {
            throw new NotImplementedException();
        }

        private void PublishMotionEvent(string deviceKey, CameraNotification notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            new MqttMessagePublisher("", deviceKey).Publish(json);
        }


    }
}
