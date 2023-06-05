// Compiler options: -target:library

public abstract class A
{
    public abstract bool IsNode { get; }
}

public abstract class B : A
{
    public sealed override bool IsNode
    {
        get { return true; }
    }
}
