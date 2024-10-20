//----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------------------

namespace System.ServiceModel.Activation
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization;

    [DataContract]
    [KnownType(typeof(IPEndPoint))]
    class TcpDuplicateContext : DuplicateContext
    {
        [DataMember]
        SocketInformation socketInformation;

        public TcpDuplicateContext(SocketInformation socketInformation, Uri via, byte[] readData)
            : base(via, readData)
        {
            this.socketInformation = socketInformation;
        }

        public SocketInformation SocketInformation
        {
            get { return this.socketInformation; }
        }
    }
}
