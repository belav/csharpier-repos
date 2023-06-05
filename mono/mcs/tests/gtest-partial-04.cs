partial
// Compiler options: gtest-partial-04-p2.cs

public struct Bug
{
    [System.Runtime.InteropServices.FieldOffset(0)]
    public int Integer;
}

class C
{
    public static void Main() { }
}
