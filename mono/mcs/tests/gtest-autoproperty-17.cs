using System;

class MainClass
{
    public abstract class Bar
    {
        public abstract bool Condition { get; }
    }

    class Baz : Bar
    {
        public override bool Condition { get; } = true;
    }

    public static void Main(string[] args) { }
}
