//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Description
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using System.ServiceModel.Channels;
    using System.Xml;

    public class MessagePropertyDescriptionCollection
        : KeyedCollection<string, MessagePropertyDescription>
    {
        internal MessagePropertyDescriptionCollection()
            : base(null, 4) { }

        protected override string GetKeyForItem(MessagePropertyDescription item)
        {
            return item.Name;
        }
    }
}
