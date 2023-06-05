using System;
using System.Threading.Tasks;

partial class X
{
    partial void Foo();

    partial async void Foo()
    {
        await Task.FromResult(1);
    }

    public static void Main() { }
}
