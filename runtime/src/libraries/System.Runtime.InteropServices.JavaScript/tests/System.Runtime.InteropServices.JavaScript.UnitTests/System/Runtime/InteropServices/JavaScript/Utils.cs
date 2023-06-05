// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Runtime.InteropServices.JavaScript
{
    partial public static unsafe class Utils
    {
        // replaces legacy Runtime.InvokeJS
        [JSImport("globalThis.App.invoke_js")]
        partial public static string InvokeJS(string code);

        [JSImport("INTERNAL.set_property")]
        partial public static void SetProperty(
            JSObject self,
            string propertyName,
            [JSMarshalAs<JSType.Function<JSType.Object>>] Action<JSObject> value
        );

        [JSImport("INTERNAL.get_property")]
        [return: JSMarshalAs<JSType.Function<JSType.Object>>]
        partial public static Action<JSObject> GetActionOfJSObjectProperty(
            JSObject self,
            string propertyName
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function>]
        partial public static Action CreateAction([JSMarshalAs<JSType.String>] string code);

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Boolean>>]
        partial public static Func<bool> CreateFunctionBool(
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number>>]
        partial public static Func<int> CreateFunctionInt([JSMarshalAs<JSType.String>] string code);

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number>>]
        partial public static Func<long> CreateFunctionLong(
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number>>]
        partial public static Func<double> CreateFunctionDouble(
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.String>>]
        partial public static Func<string> CreateFunctionString(
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Object>>]
        partial public static Func<JSObject> CreateFunctionJSObject(
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Boolean>>]
        partial public static Action<bool> CreateActionBool(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number>>]
        partial public static Action<int> CreateActionInt(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number>>]
        partial public static Action<long> CreateActionLong(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number>>]
        partial public static Action<double> CreateActionDouble(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.String>>]
        partial public static Action<string> CreateActionString(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Object>>]
        partial public static Action<JSObject> CreateActionJSObject(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Boolean, JSType.Boolean>>]
        partial public static Func<bool, bool> CreateFunctionBoolBool(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number, JSType.Number>>]
        partial public static Func<int, int> CreateFunctionIntInt(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number, JSType.Number>>]
        partial public static Func<long, long> CreateFunctionLongLong(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number, JSType.Number>>]
        partial public static Func<double, double> CreateFunctionDoubleDouble(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.String, JSType.String>>]
        partial public static Func<string, string> CreateFunctionStringString(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Object, JSType.Object>>]
        partial public static Func<JSObject, JSObject> CreateFunctionJSObjectJSObject(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Boolean, JSType.Object>>]
        partial public static Func<bool, JSObject> CreateFunctionBoolJSObject(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number, JSType.Object>>]
        partial public static Func<int, JSObject> CreateFunctionIntJSObject(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number, JSType.Object>>]
        partial public static Func<long, JSObject> CreateFunctionLongJSObject(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number, JSType.Object>>]
        partial public static Func<double, JSObject> CreateFunctionDoubleJSObject(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.String, JSType.Object>>]
        partial public static Func<string, JSObject> CreateFunctionStringJSObject(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string code
        );

        /* TODO
        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs(JSType.Function, JSType.Boolean, JSType.Promise)]
        public static partial Func<bool, Task<object>> CreateFunctionBoolTask([JSMarshalAs<JSType.String>] string arg1Name, [JSMarshalAs<JSType.String>] string code);
        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs(JSType.Function, JSType.Number, JSType.Promise)]
        public static partial Func<int, Task<object>> CreateFunctionIntTask([JSMarshalAs<JSType.String>] string arg1Name, [JSMarshalAs<JSType.String>] string code);
        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs(JSType.Function, JSType.Number, JSType.Promise)]
        public static partial Func<long, Task<object>> CreateFunctionLongTask([JSMarshalAs<JSType.String>] string arg1Name, [JSMarshalAs<JSType.String>] string code);
        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs(JSType.Function, JSType.Number, JSType.Promise)]
        public static partial Func<double, Task<object>> CreateFunctionDoubleJSTask([JSMarshalAs<JSType.String>] string arg1Name, [JSMarshalAs<JSType.String>] string code);
        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs(JSType.Function, JSType.String, JSType.Promise)]
        public static partial Func<string, Task<object>> CreateFunctionStringTask([JSMarshalAs<JSType.String>] string arg1Name, [JSMarshalAs<JSType.String>] string code);
        [return: JSMarshalAs(JSType.Function, JSType.Promise)]
        public static partial Func<Task<object>> CreateFunctionTask([JSMarshalAs<JSType.String>] string code);
        */

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number, JSType.Number>>]
        partial public static Action<int, int> CreateActionIntInt(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string arg2Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number, JSType.Number>>]
        partial public static Action<long, long> CreateActionLongLong(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string arg2Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number, JSType.Number>>]
        partial public static Action<double, double> CreateActionDoubleDouble(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string arg2Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.String, JSType.String>>]
        partial public static Action<string, string> CreateActionStringString(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string arg2Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number, JSType.Number, JSType.Number>>]
        partial public static Func<int, int, int> CreateFunctionIntIntInt(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string arg2Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number, JSType.Number, JSType.Number>>]
        partial public static Func<long, long, long> CreateFunctionLongLongLong(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string arg2Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.Number, JSType.Number, JSType.Number>>]
        partial public static Func<double, double, double> CreateFunctionDoubleDoubleDouble(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string arg2Name,
            [JSMarshalAs<JSType.String>] string code
        );

        [JSImport("globalThis.App.create_function")]
        [return: JSMarshalAs<JSType.Function<JSType.String, JSType.String, JSType.String>>]
        partial public static Func<string, string, string> CreateFunctionStringStringString(
            [JSMarshalAs<JSType.String>] string arg1Name,
            [JSMarshalAs<JSType.String>] string arg2Name,
            [JSMarshalAs<JSType.String>] string code
        );
    }
}
