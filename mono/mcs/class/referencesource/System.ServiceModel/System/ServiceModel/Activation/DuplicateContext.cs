//----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------------------

namespace System.ServiceModel.Activation
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.Serialization;

    [DataContract]
    [KnownType(typeof(TcpDuplicateContext))]
    [KnownType(typeof(NamedPipeDuplicateContext))]
    class DuplicateContext
    {
        [DataMember]
        Uri via;

        [DataMember]
        byte[] readData;

        protected DuplicateContext(Uri via, byte[] readData)
        {
            this.via = via;
            this.readData = readData;
        }

        public Uri Via
        {
            get { return this.via; }
        }

        public byte[] ReadData
        {
            get { return this.readData; }
        }
    }
}
