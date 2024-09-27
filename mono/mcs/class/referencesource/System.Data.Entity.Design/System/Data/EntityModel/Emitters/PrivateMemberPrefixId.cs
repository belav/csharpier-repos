//---------------------------------------------------------------------
// <copyright file="PrivateMemberPrefixId.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
// @owner       jeffreed
// @backupOwner srimand
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace System.Data.EntityModel.Emitters
{
    internal enum PrivateMemberPrefixId
    {
        Field,
        IntializeMethod,
        PropertyInfoProperty,
        PropertyInfoField,

        // add additional members here
        Count,
    }
}
