﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Models
{
    public abstract class ZWaveDevice : Device
    {
        public byte NodeId { get; set; }
    }
}