// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--==
//
// <OWNER>Microsoft</OWNER>
/*=============================================================================
**
** Class: ManualResetEvent
**
**
** Purpose: An example of a WaitHandle class
**
**
=============================================================================*/
namespace System.Threading
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [HostProtection(Synchronization = true, ExternalThreading = true)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public sealed class ManualResetEvent : EventWaitHandle
    {
        public ManualResetEvent(bool initialState)
            : base(initialState, EventResetMode.ManualReset) { }
    }
}
