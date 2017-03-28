using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Common;
using Euricom.IoT.DataLayer;
using System;

namespace Euricom.IoT.Api.Managers
{
    public class HardwareManager : IHardwareManager
    {
        private readonly ICameraManager _cameraManager;
        private readonly IDanaLockManager _danaLockManager;
        private readonly ILazyBoneManager _lazyBoneManager;


        public HardwareManager()
        {

        }
        public Hardware GetHardware()
        {
            var hardware = Database.Instance.GetHardware();
            return hardware;
        }

        public void AddHardware(string name, string type)
        {
            switch (type)
            {
                case "camera":
                    _cameraManager.Add(new Camera()
                    {
                        Name = name
                    });

                    break;
                case "danalock":
                    _danaLockManager.Add(new Common.DanaLock()
                    {
                        Name = name
                    });
                    break;
                case "lazybone":
                    _lazyBoneManager.Add(new Common.LazyBone()
                    {
                        Name = name
                    });
                    break;
                default:
                    throw new ArgumentException($"type: {type} has no implementation..");
            }

        }
    }
}
