﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Common
{
    public class LazyBone : Device
    {
        public string Host { get; set; }
        public short Port { get; set; }
        public int PollingTime { get; set; }
    }
}
