//------------------------------------------------------------------------------
// <copyright file="NameValuePair.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

// @owner  Microsoft
// @backupOwner  Microsoft
//------------------------------------------------------------------------------

namespace System.Data.EntityClient
{
    internal
    /// <summary>
    /// Copied from System.Data.dll
    /// </summary>
    sealed class NameValuePair
    {
        private readonly string _name;
        private readonly string _value;
        private readonly int _length;
        private NameValuePair _next;

        internal NameValuePair(string name, string value, int length)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(name), "empty keyname");
            _name = name;
            _value = value;
            _length = length;
        }

        internal NameValuePair Next
        {
            get { return _next; }
            set
            {
                if ((null != _next) || (null == value))
                {
                    throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.NameValuePairNext);
                }
                _next = value;
            }
        }
    }
}
