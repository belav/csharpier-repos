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
    public class MatchNoneMessageFilter : MessageFilter
    {
        public MatchNoneMessageFilter()
            : base() { }

        public override bool Match(MessageBuffer messageBuffer)
        {
            if (messageBuffer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("messageBuffer");
            }
            return false;
        }

        public override bool Match(Message message)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            }
            return false;
        }
    }
}
