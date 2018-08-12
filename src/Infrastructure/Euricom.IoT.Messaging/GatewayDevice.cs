using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Logging;
using Euricom.IoT.Messaging.Interfaces;
using Euricom.IoT.Models.Messages;
using Euricom.IoT.Security;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Euricom.IoT.Messaging
{
    public class GatewayDevice : IGatewayDevice
    {
        private const string ConnectionStringTemplate = "HostName={0};DeviceId={1};SharedAccessKey={2}";

        private readonly IMessageHandler _messageHandler;
        private readonly string _primaryKey;
        private readonly string _host;

        private readonly object _lock = new object();
        private readonly ConcurrentQueue<Dictionary<string, object>> _stateQueue = new ConcurrentQueue<Dictionary<string, object>>();
        private readonly ConcurrentQueue<DeviceMessage> _messageQueue = new ConcurrentQueue<DeviceMessage>();

        private CancellationTokenSource _cancellation = new CancellationTokenSource();

        private Task _receiver;
        private Task _stateSender;
        private Task _messageSender;
        private DeviceClient _deviceClient;

        public GatewayDevice(IMessageHandler messageHandler, string host, string deviceId, string primaryKey)
        {
            _host = host;
            DeviceId = deviceId;
            _primaryKey = primaryKey;
            _messageHandler = messageHandler;
        }

        public string DeviceId { get; private set; }

        public bool IsRunning()
        {
            return _receiver?.IsCompleted == false && _stateSender?.IsCompleted == false && _messageSender?.IsCompleted == false;
        }

        public void Start()
        {
            if (_cancellation.IsCancellationRequested || _receiver != null || _stateSender != null)
            {
                throw new Exception("This worker is already running, or is currently stopping...");
            }

            lock (_lock)
            {
                if (_cancellation.IsCancellationRequested || _receiver != null || _stateSender != null)
                {
                    throw new Exception("This worker is already running, or is currently stopping...");
                }

                _deviceClient = DeviceClient.CreateFromConnectionString(string.Format(ConnectionStringTemplate, _host, DeviceId, _primaryKey), TransportType.Mqtt);

                _receiver = Task.Run(async () => await ReceiveAsync());
                _stateSender = Task.Run(async () => await UpdateStateAsync());
                _messageSender = Task.Run(async () => await SendMessageAsync());
            }
        }

        private async Task ReceiveAsync()
        {
            while (_cancellation.IsCancellationRequested == false)
            {
                try
                {
                    var @event = await _deviceClient.ReceiveAsync(TimeSpan.FromMilliseconds(2000));
                    if (@event == null)
                    {
                        await Task.Delay(50, _cancellation.Token);
                    }
                    else
                    {
                        var data = Encoding.UTF8.GetString(@event.GetBytes());

                        // Handle message
                        bool handled = await HandleMessage(data);
                        if (handled)
                        {
                            await _deviceClient.CompleteAsync(@event);
                        }
                        else
                        {
                            await _deviceClient.RejectAsync(@event);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error(ex);
                }
            }
        }

        private async Task<bool> HandleMessage(string data)
        {
            try
            {
                Logger.Instance.Information($"Handling message: {data}");

                var message = JsonConvert.DeserializeObject<CommandMessage>(data);
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                if (message.MessageType == null)
                {
                    throw new ArgumentNullException(nameof(message.MessageType));
                }

                // Verify JWT token
                var valid = JwtSecurity.VerifyJwt(message.CommandToken, out var payload);
                if (valid == false)
                {
                    Logger.Instance.Warning($"Invalid jwt: {JsonConvert.SerializeObject(payload)}");
                }

                switch (message.MessageType)
                {
                    case "danalock":
                        await _messageHandler.HandleDanaLockMessage(message.Device, JsonConvert.DeserializeObject<DanaLockMessage>(data));
                        break;
                    case "wallmount_switch":
                        await _messageHandler.HandleWallMountSwitchMessage(message.Device, JsonConvert.DeserializeObject<WallmountSwitchMessage>(data));
                        break;
                    case "camera_snapshot":
                        var response = await _messageHandler.HandleCameraMessage(message.Device, JsonConvert.DeserializeObject<CameraSnapshotMessage>(data));

                        SendMessage(response);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown message type.");
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return true;
            }
        }

        private async Task UpdateStateAsync()
        {
            while (_cancellation.IsCancellationRequested == false)
            {
                try
                {
                    if (_stateQueue.TryDequeue(out var properties) == false)
                    {
                        await Task.Delay(10, _cancellation.Token);
                    }
                    else
                    {
                        var report = JObject.FromObject(properties);
                        var collection = new TwinCollection(JsonConvert.SerializeObject(report, Formatting.None,
                            new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            }));

                        await _deviceClient.UpdateReportedPropertiesAsync(collection);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error(ex);
                }
            }
        }

        private async Task SendMessageAsync()
        {
            while (_cancellation.IsCancellationRequested == false)
            {
                try
                {
                    if (_messageQueue.TryDequeue(out var messageContent) == false)
                    {
                        await Task.Delay(10, _cancellation.Token);
                    }
                    else
                    {
                        var content = JsonConvert.SerializeObject(messageContent);
                        var message = new Message(Encoding.UTF8.GetBytes(content));

                        await _deviceClient.SendEventAsync(message);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error(ex);
                }
            }
        }

        public void Stop()
        {
            if (_cancellation.IsCancellationRequested)
            {
                return;
            }

            lock (_lock)
            {
                if (_cancellation.IsCancellationRequested)
                {
                    return;
                }

                _cancellation.Cancel();
                _cancellation = new CancellationTokenSource();
            }
        }

        public void UpdateState(Dictionary<string, object> state)
        {
            _stateQueue.Enqueue(state);
        }

        public void SendMessage(DeviceMessage message)
        {
            _messageQueue.Enqueue(message);
        }
    }
}