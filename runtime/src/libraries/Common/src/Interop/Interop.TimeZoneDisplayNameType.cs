partial
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static class Interop
{
    partial internal static class Globalization
    {
        // needs to be kept in sync with TimeZoneDisplayNameType in System.Globalization.Native
        internal enum TimeZoneDisplayNameType
        {
            Generic = 0,
            Standard = 1,
            DaylightSavings = 2,
            GenericLocation = 3,
            ExemplarCity = 4,
        }
    }
}
