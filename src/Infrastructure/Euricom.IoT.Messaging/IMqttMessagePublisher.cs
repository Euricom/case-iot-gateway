using System.Threading.Tasks;

namespace Euricom.IoT.Messaging
{
    public interface IMqttMessagePublisher
    {
        Task Publish(string json);
    }
}