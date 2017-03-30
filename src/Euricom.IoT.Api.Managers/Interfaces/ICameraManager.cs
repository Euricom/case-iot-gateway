using Euricom.IoT.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface ICameraManager
    {
        Camera Add(Camera camera);
        void Notify(string device, string url, string timestamp, int frameNumber, int eventNumber);
    }
}
