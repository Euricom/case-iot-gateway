using Euricom.IoT.Models.Logging;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface ILogManager
    {
        string[] QueryLogFiles();
        Log GetLog(string date);
        string[] GetOpenZWaveLog();
    }
}