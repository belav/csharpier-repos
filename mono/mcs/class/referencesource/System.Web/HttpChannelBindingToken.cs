//------------------------------------------------------------------------------
// <copyright file="HttpChannelBindingToken.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

/*
 * HttpChannelBindingToken class
 *
 * Copyright (c) 2008 Microsoft Corporation
 */

namespace System.Web
{
    using System.Security.Authentication.ExtendedProtection;
    using System.Security.Permissions;

    internal sealed class HttpChannelBindingToken : ChannelBinding
    {
        private int _size;

        internal HttpChannelBindingToken(IntPtr token, int tokenSize)
        {
            SetHandle(token);
            _size = tokenSize;
        }

        protected override bool ReleaseHandle()
        {
            SetHandle(IntPtr.Zero);
            _size = 0;

            return true;
        }

        public override int Size
        {
            get { return _size; }
        }
    }
}
