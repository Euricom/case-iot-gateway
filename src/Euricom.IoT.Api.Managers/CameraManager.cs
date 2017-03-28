using AzureDeviceManager;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Common;
using Euricom.IoT.Common.Notifications;
using Euricom.IoT.DataLayer;
using Euricom.IoT.FileTransfer;
using Euricom.IoT.Messaging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Manager
{
    public class CameraManager : ICameraManager
    {
        private DeviceManager _azureDeviceManager;

        public CameraManager()
        {

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

        private async Task<byte[]> GetMotionImage(string filePath)
        {
            var splitted = filePath.Split('/');
            var path = "/sdcard/Camera1/" + splitted[splitted.Length - 2] + "/" + splitted[splitted.Length - 1];
            var bytes = await FtpFileTransfer.GetFile(path);
            return bytes;
        }

        private void PublishMotionEvent(string deviceKey, CameraNotification notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            new MqttMessagePublisher("", deviceKey).Publish(json);
        }

    }
}
