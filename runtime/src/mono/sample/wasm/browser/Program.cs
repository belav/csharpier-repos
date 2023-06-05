// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.InteropServices;

namespace Sample
{
    partial public class Test
    {
        public static int Main(string[] args)
        {
            DisplayMeaning(42);
            return 0;
        }

        [JSImport("Sample.Test.displayMeaning", "main.js")]
        partial internal static void DisplayMeaning(int meaning);
    }
}
