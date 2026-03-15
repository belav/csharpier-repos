using System;

[AttributeUsage(AttributeTargets.All)]
public class A : Attribute
{
    public A(object o) { }
}

[A((object)AttributeTargets.All)]
public class Test
{
    public static void Main() { }
}
