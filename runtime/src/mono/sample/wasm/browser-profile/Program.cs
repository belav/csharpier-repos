// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;

namespace Sample
{
    partial public class Test
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }

        [JSExport]
        public static int TestMeaning()
        {
            return 42;
        }

        [JSExport]
        public static void StopProfile() { }
    }
}
