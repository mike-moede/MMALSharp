﻿// <copyright file="OverlayPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Native;

namespace MMALSharp.Ports.Inputs
{
    /// <summary>
    /// Represents port behaviour especially for the static overlay renderer functionality. This object overrides <see cref="NativeInputPortCallback"/>
    /// forcing it to do nothing when it receives a callback from the component.
    /// </summary>
    public unsafe class OverlayPort : InputPort
    {
        /// <summary>
        /// Creates a new instance of <see cref="OverlayPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public OverlayPort(IntPtr ptr, MMALComponentBase comp, PortType type, Guid guid) 
            : base(ptr, comp, type, guid)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="OverlayPort"/>.
        /// </summary>
        /// <param name="copyFrom">The port to copy data from.</param>
        public OverlayPort(PortBase copyFrom)
            : base((IntPtr)copyFrom.Ptr, copyFrom.ComponentReference, copyFrom.PortType, copyFrom.Guid)
        {
        }

        internal override void NativeInputPortCallback(MMAL_PORT_T* port, MMAL_BUFFER_HEADER_T* buffer) { }
    }
}
