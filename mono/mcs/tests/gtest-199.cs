// Compiler options: /unsafe

// similar code from #75772.
public class Splay<T>
{
    private unsafe struct Node
    {
        private int* foo;
        private T data;
    }
}

class Foo
{
    public static void Main() { }
}
