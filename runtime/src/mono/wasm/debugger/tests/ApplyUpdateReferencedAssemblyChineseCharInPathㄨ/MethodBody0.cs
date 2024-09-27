// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;

namespace ApplyUpdateReferencedAssembly
{
    public class MethodBodyUnchangedAssembly
    {
        public static string StaticMethod1()
        {
            Console.WriteLine("original");
            return "ok";
        }
    }
}
