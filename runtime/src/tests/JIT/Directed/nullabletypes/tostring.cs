// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
//<Title>Nullable types lift the ToString() method from the underlying struct</Title>
//<Description>
//  A nullable type with a value returns the ToString() from the underlying struct
//</Description>

#pragma warning disable 0649

using System;


interface BaseInter { }
interface GenInter<T> { }

struct Struct { }
struct ImplStruct : BaseInter { }
struct OpenGenImplStruct<T> : GenInter<T> { }
struct CloseGenImplStruct : GenInter<int> { }

class Foo { }

class NullableTest1
{
    static int? i = new int?(1);
    static Struct? s = new Struct?(new Struct());
    static ImplStruct? imps = new ImplStruct?(new ImplStruct());
    static OpenGenImplStruct<Foo>? genfoo = new OpenGenImplStruct<Foo>?(new OpenGenImplStruct<Foo>());
    static CloseGenImplStruct? genint = new CloseGenImplStruct?(new CloseGenImplStruct());


    public static void Run()
    {
        Test_nullabletypes.Eval(i.ToString(), 1.ToString());
        Test_nullabletypes.Eval(s.ToString(), default(Struct).ToString());
        Test_nullabletypes.Eval(imps.ToString(), default(ImplStruct).ToString());
        Test_nullabletypes.Eval(genfoo.ToString(), default(OpenGenImplStruct<Foo>).ToString());
        Test_nullabletypes.Eval(genint.ToString(), default(CloseGenImplStruct).ToString());
    }
}

class NullableTest2
{
    static int? i;
    static Struct? s;
    static ImplStruct? imps;
    static OpenGenImplStruct<Foo>? genfoo;
    static CloseGenImplStruct? genint;


    public static void Run()
    {
        Test_nullabletypes.Eval(i.ToString(), "");
        Test_nullabletypes.Eval(s.ToString(), "");
        Test_nullabletypes.Eval(imps.ToString(), "");
        Test_nullabletypes.Eval(genfoo.ToString(), "");
        Test_nullabletypes.Eval(genint.ToString(), "");
    }
}

class NullableTests
{
    public static void Run()
    {
        NullableTest1.Run();
        NullableTest2.Run();
    }
}
