//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace System.ServiceModel.Channels
{
    using System.ServiceModel.Description;
    using System.Xml;

    public interface ITransportTokenAssertionProvider
    {
        XmlElement GetTransportTokenAssertion();
    }
}
