//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Xml;

namespace System.ServiceModel.Dispatcher
{
    class DataContractSerializerFaultFormatter : FaultFormatter
    {
        internal DataContractSerializerFaultFormatter(Type[] detailTypes)
            : base(detailTypes) { }

        internal DataContractSerializerFaultFormatter(
            SynchronizedCollection<FaultContractInfo> faultContractInfoCollection
        )
            : base(faultContractInfoCollection) { }
    }
}
