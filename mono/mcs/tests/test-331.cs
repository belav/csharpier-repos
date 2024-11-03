// Compiler options: -unsafe

class T
{
    private unsafe byte* ptr;
    internal unsafe byte* Ptr
    {
        get { return ptr; }
        set { ptr = value; }
    }

    public static void Main() { }
}
