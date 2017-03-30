using AzureDeviceManager;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureBlobStorage;
using Euricom.IoT.Common;
using Euricom.IoT.Common.Notifications;
using Euricom.IoT.DataLayer;
using Euricom.IoT.Messaging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Euricom.IoT.Api.Manager
{
    public class CameraManager : ICameraManager
    {
        private AzureBlobStorageManager _azureBlobStorage;
        private DeviceManager _azureDeviceManager;

        public CameraManager()
        {
            _azureBlobStorage = new IoT.AzureBlobStorage.AzureBlobStorageManager();
        }

        public Camera Add(Camera camera)
        {
            //Add device to Azure Device IoT
            var deviceId = _azureDeviceManager.AddDeviceAsync(camera.Name).Result;
            camera.DeviceId = deviceId;

            //Convert to json
            var json = JsonConvert.SerializeObject(camera);

            //Save to database
            Database.Instance.SetValue("Cameras", camera.DeviceId, json);

            return camera;
        }

        public void Notify(string device, string url, string timestamp, int frameNumber, int eventNumber)
        {
            var config = DataLayer.Database.Instance.GetCameraConfig(device);

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

        private void PublishMotionEvent(string deviceKey, CameraNotification notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            new MqttMessagePublisher("", deviceKey).Publish(json);
        }


    }
}
