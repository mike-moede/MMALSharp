﻿using SharPicam.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharPicam
{
    public unsafe class MMALPoolImpl : MMALObject
    {
        public MMAL_POOL_T* Ptr { get; set; }
        public MMALPortImpl Port { get; set; }

        public MMALPoolImpl(MMALPortImpl port)
        {
            this.Ptr = MMALUtil.mmal_port_pool_create(port.Ptr, port.BufferNum, port.BufferSize);
        }
        
        public void Destroy()
        {
            MMALPool.mmal_pool_destroy(this.Ptr);
        }

        public void Resize(uint numHeaders, uint size)
        {
            MMALCheck(MMALPool.mmal_pool_resize(this.Ptr, numHeaders, size), "Unable to resize pool");
        }

    }
}
