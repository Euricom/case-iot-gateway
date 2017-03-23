using Euricom.IoT.Api.Notifications;
using Euricom.IoT.Common;
using Euricom.IoT.FileTransfer;
using Euricom.IoT.Messaging;
using Newtonsoft.Json;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Controllers
{
    [RestController(InstanceCreationType.Singleton)]
    public class CameraController
    {
        public CameraController()
        {
        }

        [UriFormat("/camera/notify?device={device}&url={url}&ts={timestamp}&frame={frameNumber}&event={eventNumber}")]
        public IGetResponse Notify(string device, string url, string timestamp, int frameNumber, int eventNumber)
        {
            var config = DataLayer.Database.Instance.GetCameraConfig(device);

            //var imageBytes = GetMotionImage(url);
            var notification = new CameraNotification
            {
                FilePath = url,
                EventNumber = eventNumber,
                FrameNumber = frameNumber,
                Timestamp = timestamp,
                DeviceKey = config.DeviceKey,
            };

            // Publish to IoT Hub
            PublishMotionEvent(config.DeviceKey, notification);

            // Send response back
            return new GetResponse(GetResponse.ResponseStatus.OK);
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
            new MqttMessagePublisher(deviceKey).Publish(json);
        }
    }
}
