// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Generated by Fuzzlyn v1.2 on 2021-08-16 12:59:38
// Run on .NET 6.0.0-dev on X64 Windows
// Seed: 9053537220764489964
// Reduced from 104.8 KiB to 0.8 KiB in 00:00:44
// Debug: Outputs 1, 0
// Release: Outputs 1, 1
struct S0
{
    public short F1;
    public ushort F4;
    public S0(short f1) : this()
    {
        F1 = f1;
    }
}

struct S1
{
    public S0 F0;
    public S1(S0 f0) : this()
    {
        F0 = f0;
    }
}

struct S2
{
    public S1 F1;
    public S0 F2;
    public S2(S1 f1) : this()
    {
        F1 = f1;
    }
}

struct S3
{
    public S2 F0;
    public S3(S2 f0) : this()
    {
        F0 = f0;
    }
}

struct S4
{
    public sbyte F4;
    public S3 F5;
    public S4(S3 f5) : this()
    {
        F5 = f5;
    }
}

public class Program
{
    public static int Test()
    {
        S4 vr0 = new S4(new S3(new S2(new S1(new S0(1)))));
        System.Console.WriteLine(vr0.F5.F0.F1.F0.F1);
        System.Console.WriteLine(vr0.F5.F0.F1.F0.F4);
        return vr0.F5.F0.F1.F0.F1 + vr0.F5.F0.F1.F0.F4;
    }

    public static int Main()
    {
        if (Test() == 1)
        {
            return 100;
        }
        return 101;
    }
}