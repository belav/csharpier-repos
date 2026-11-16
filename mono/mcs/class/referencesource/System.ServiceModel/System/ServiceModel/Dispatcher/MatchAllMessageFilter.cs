//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.ServiceModel.Channels;

    [DataContract]
    public class MatchAllMessageFilter : MessageFilter
    {
        public MatchAllMessageFilter()
            : base() { }

        public override bool Match(MessageBuffer messageBuffer)
        {
            if (messageBuffer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("messageBuffer");
            }
            return true;
        }

        public override bool Match(Message message)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            }
            return true;
        }
    }
}
