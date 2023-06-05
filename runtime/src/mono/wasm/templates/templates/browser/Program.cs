using System;
using System.Runtime.InteropServices.JavaScript;

Console.WriteLine("Hello, Browser!");

partial public class MyClass
{
    [JSExport]
    internal static string Greeting()
    {
        var text = $"Hello, World! Greetings from {GetHRef()}";
        Console.WriteLine(text);
        return text;
    }

    [JSImport("window.location.href", "main.js")]
    partial internal static string GetHRef();
}
