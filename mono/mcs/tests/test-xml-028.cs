// Compiler options: -doc:xml-028.xml
using System;

partial
/// <summary>
/// Partial comment #2
public class Test
{
    string Bar;

    public static void Main() { }

    partial
    /// <summary>
    /// Partial inner class!
    internal class Inner
    {
        public string Hoge;
    }
}

partial
/// Partial comment #1
/// </summary>
public class Test
{
    public string Foo;

    partial
    /// ... is still available.
    /// </summary>
    internal class Inner
    {
        string Fuga;
    }
}
