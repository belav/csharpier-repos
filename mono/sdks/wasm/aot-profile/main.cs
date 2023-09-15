using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WebAssembly;
using WebAssembly.Core;

public class HelloWorld
{
    public static void Main(String[] args)
    {
        Console.WriteLine("Hello, World!");
        Runtime.StopProfile();
    }
}
