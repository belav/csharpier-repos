using System;
using System.Runtime.InteropServices;
using System.Threading;

public class HelloWorld
{
    public static void Main(String[] args)
    {
        var t = new Thread(
            delegate()
            {
                Console.WriteLine("In thread.");
                Thread.Sleep(1000);
            }
        );
        t.Start();
        t.Join();
        Console.WriteLine("Hello, World!");
    }
}
