//---------------------------------------------------------------------
// <copyright file="ICustomPluralizationMapping.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
// @owner       leil
// @backupOwner jeffreed
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.Entity.Design.Common;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Design.PluralizationServices
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1704:IdentifiersShouldBeSpelledCorrectly",
        MessageId = "Pluralization"
    )]
    public interface ICustomPluralizationMapping
    {
        void AddWord(string singular, string plural);
    }
}
