// <copyright file="GenericPort.cs" company="Techyian">
// Copyright (c) Ian Auty. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using MMALSharp.Common.Utility;
using MMALSharp.Handlers;

namespace MMALSharp.Ports
{
    /// <summary>
    /// Represents a generic MMAL port of any type.
    /// </summary>
    public class GenericPort : PortBase
    {
        /// <inheritdoc />
        public override Resolution Resolution
        {
            get => new Resolution(this.Width, this.Height);
            internal set
            {
                this.Width = value.Pad().Width;
                this.Height = value.Pad().Height;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="GenericPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        public GenericPort(IntPtr ptr, MMALComponentBase comp, PortType type, Guid guid) 
            : base(ptr, comp, type, guid)
        {
        }
        
        /// <summary>
        /// Creates a new instance of <see cref="GenericPort"/>. 
        /// </summary>
        /// <param name="ptr">The native pointer.</param>
        /// <param name="comp">The component this port is associated with.</param>
        /// <param name="type">The type of port.</param>
        /// <param name="guid">Managed unique identifier for this component.</param>
        /// <param name="handler">The capture handler.</param>
        public GenericPort(IntPtr ptr, MMALComponentBase comp, PortType type, Guid guid, ICaptureHandler handler) 
            : base(ptr, comp, type, guid, handler)
        {
        }
    }
}