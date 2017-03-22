using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Euricom.IoT.DanaLock;

namespace WebApplicationBasic.Controllers
{
    public class DoorLockController : Controller
    {
        private readonly DanaLock _danaLock;

 
        public DoorLockController()
        {
            _danaLock = DanaLock.Instance;
        }

        public IActionResult Index()
        {
            _danaLock.OpenLock();
            _danaLock.CloseLock();

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
