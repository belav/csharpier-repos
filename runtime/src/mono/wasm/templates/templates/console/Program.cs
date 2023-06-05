using System;
using System.Runtime.InteropServices.JavaScript;

Console.WriteLine("Hello, Console!");

return 0;

partial public class MyClass
{
    [JSExport]
    internal static string Greeting()
    {
        var text = $"Hello, World! Greetings from node version: {GetNodeVersion()}";
        return text;
    }

    [JSImport("node.process.version", "main.mjs")]
    partial internal static string GetNodeVersion();
}
