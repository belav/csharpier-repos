// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;

using Internal.Cryptography.Pal.Windows;

namespace Internal.Cryptography
{
    partial internal abstract class PkcsPal
    {
        private static readonly PkcsPal s_instance = new PkcsPalWindows();
    }
}
