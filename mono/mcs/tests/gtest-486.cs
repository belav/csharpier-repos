using System;
using System.Collections.Generic;

interface IMyCollection<T> : ICollection<T> { }

class MyCollection<T> : IMyCollection<T>
{
    public void AddRange(IMyCollection<T> items) { }

    public void AddRange(IEnumerable<T> items) { }

    public int Count
    {
        get { return 0; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    public void Add(T item) { }

    public void Clear() { }

    public bool Contains(T item)
    {
        return false;
    }

    public void CopyTo(T[] a, int i) { }

    public bool Remove(T item)
    {
        return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return null;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return null;
    }
}

class P
{
    protected static MyCollection<String> foo = new MyCollection<String>();

    protected static MyCollection<String> bar = new MyCollection<String>();

    public static MyCollection<String> IgnoreTokens
    {
        get
        {
            if (foo.Count == 0)
                foo.AddRange(bar); // false error on Mono 2.0 and 2.4: The call is ambiguous between...
            return foo;
        }
    }

    public static void Main() { }
}
