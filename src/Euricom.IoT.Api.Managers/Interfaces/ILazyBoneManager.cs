using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface ILazyBoneManager
    {
        Common.LazyBone Add(Common.LazyBone lazyBone);
        void Switch(string device, string state);
    }
}
