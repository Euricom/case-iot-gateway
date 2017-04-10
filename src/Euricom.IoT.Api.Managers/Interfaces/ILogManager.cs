using System.Threading.Tasks;
using Euricom.IoT.Common;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface ILogManager
    {
        string[] QueryLogFiles();
        Log GetLog(string date);
        string[] GetOpenZWaveLog();
    }
}