// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// this is regression for VSW 491671
// we load a generic type that takes itself as generic parameter.
// at depth 39 we were AVing when ngening the assembly.
// This test is for JIT.
// NGEN test will be located in the NGEN tree.


using System;

public class Test_GenTypeItself {
   public static int Main() {
      MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<int>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> obj = new MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<MyClass<int>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>();

      Console.WriteLine("PASS");
      return 100;
   }
}

public class MyClass<T> {}
