// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

using Xunit;

namespace DllImportGenerator.IntegrationTests
{
    partial class NativeExportsNE
    {
        partial public class PreserveSig
        {
            partial public class False
            {
                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_return",
                    PreserveSig = false
                )]
                partial public static void NoReturnValue(int i);

                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_out_int",
                    PreserveSig = false
                )]
                partial public static void Int_Out(int i, out int ret);

                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_out_int",
                    PreserveSig = false
                )]
                partial public static int Int_AsReturn(int i);

                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_out_int",
                    PreserveSig = false
                )]
                partial public static void Bool_Out(
                    int i,
                    [MarshalAs(UnmanagedType.U4)] out bool ret
                );

                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_out_int",
                    PreserveSig = false
                )]
                [return: MarshalAs(UnmanagedType.U4)]
                partial public static bool Bool_AsReturn(int i);

                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_out_ushort",
                    PreserveSig = false
                )]
                partial public static void Char_Out(
                    int i,
                    [MarshalAs(UnmanagedType.U2)] out char ret
                );

                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_out_ushort",
                    PreserveSig = false
                )]
                [return: MarshalAs(UnmanagedType.U2)]
                partial public static char Char_AsReturn(int i);

                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_out_ushort_string",
                    PreserveSig = false
                )]
                partial public static void String_Out(
                    int i,
                    [MarshalAs(UnmanagedType.LPWStr)] out string ret
                );

                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_out_ushort_string",
                    PreserveSig = false
                )]
                [return: MarshalAs(UnmanagedType.LPWStr)]
                partial public static string String_AsReturn(int i);

                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_out_int_array",
                    PreserveSig = false
                )]
                partial public static void IntArray_Out(
                    int i,
                    [MarshalAs(UnmanagedType.LPArray, SizeConst = sizeof(int))] out int[] ret
                );

                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_out_int_array",
                    PreserveSig = false
                )]
                [return: MarshalAs(UnmanagedType.LPArray, SizeConst = sizeof(int))]
                partial public static int[] IntArray_AsReturn(int i);

                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_out_ushort_string_array",
                    PreserveSig = false
                )]
                partial public static void StringArray_Out(
                    int i,
                    [MarshalAs(
                        UnmanagedType.LPArray,
                        ArraySubType = UnmanagedType.LPWStr,
                        SizeConst = sizeof(int)
                    )]
                        out string[] ret
                );

                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_out_ushort_string_array",
                    PreserveSig = false
                )]
                [return: MarshalAs(
                    UnmanagedType.LPArray,
                    ArraySubType = UnmanagedType.LPWStr,
                    SizeConst = sizeof(int)
                )]
                partial public static string[] StringArray_AsReturn(int i);

                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_out_handle",
                    PreserveSig = false
                )]
                partial public static void SafeHandle_Out(int hr, out DummySafeHandle ret);

                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_out_handle",
                    PreserveSig = false
                )]
                partial public static DummySafeHandle SafeHandle_AsReturn(int hr);
            }

            public class DummySafeHandle : Microsoft.Win32.SafeHandles.SafeHandleMinusOneIsInvalid
            {
                private DummySafeHandle()
                    : base(ownsHandle: true) { }

                protected override bool ReleaseHandle() => true;
            }

            partial public class True
            {
                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_return",
                    PreserveSig = true
                )]
                partial public static int NoReturnValue(int i);

                [GeneratedDllImport(
                    NativeExportsNE_Binary,
                    EntryPoint = "hresult_out_int",
                    PreserveSig = true
                )]
                partial public static int Int_Out(int i, out int ret);
            }
        }
    }

    public class PreserveSigTests
    {
        private const int E_INVALIDARG = unchecked((int)0x80070057);
        private const int COR_E_NOTSUPPORTED = unchecked((int)0x80131515);
        private const int S_OK = 0;
        private const int S_FALSE = 1;

        [Theory]
        [InlineData(E_INVALIDARG)]
        [InlineData(COR_E_NOTSUPPORTED)]
        [InlineData(-1)]
        public void PreserveSigFalse_Error(int input)
        {
            Exception exception = Marshal.GetExceptionForHR(input);
            Assert.NotNull(exception);

            int expectedHR = input;
            var exceptionType = exception.GetType();
            Assert.Equal(expectedHR, exception.HResult);
            Exception ex;

            ex = Assert.Throws(
                exceptionType,
                () => NativeExportsNE.PreserveSig.False.NoReturnValue(input)
            );
            Assert.Equal(expectedHR, ex.HResult);

            {
                ex = Assert.Throws(
                    exceptionType,
                    () => NativeExportsNE.PreserveSig.False.Int_Out(input, out int ret)
                );
                Assert.Equal(expectedHR, ex.HResult);

                ex = Assert.Throws(
                    exceptionType,
                    () => NativeExportsNE.PreserveSig.False.Int_AsReturn(input)
                );
                Assert.Equal(expectedHR, ex.HResult);
            }
            {
                ex = Assert.Throws(
                    exceptionType,
                    () => NativeExportsNE.PreserveSig.False.Bool_Out(input, out bool ret)
                );
                Assert.Equal(expectedHR, ex.HResult);

                ex = Assert.Throws(
                    exceptionType,
                    () => NativeExportsNE.PreserveSig.False.Bool_AsReturn(input)
                );
                Assert.Equal(expectedHR, ex.HResult);
            }
            {
                ex = Assert.Throws(
                    exceptionType,
                    () => NativeExportsNE.PreserveSig.False.Char_Out(input, out char ret)
                );
                Assert.Equal(expectedHR, ex.HResult);

                ex = Assert.Throws(
                    exceptionType,
                    () => NativeExportsNE.PreserveSig.False.Char_AsReturn(input)
                );
                Assert.Equal(expectedHR, ex.HResult);
            }
            {
                ex = Assert.Throws(
                    exceptionType,
                    () => NativeExportsNE.PreserveSig.False.String_Out(input, out string ret)
                );
                Assert.Equal(expectedHR, ex.HResult);

                ex = Assert.Throws(
                    exceptionType,
                    () => NativeExportsNE.PreserveSig.False.String_AsReturn(input)
                );
                Assert.Equal(expectedHR, ex.HResult);
            }
            {
                ex = Assert.Throws(
                    exceptionType,
                    () => NativeExportsNE.PreserveSig.False.IntArray_Out(input, out int[] ret)
                );
                Assert.Equal(expectedHR, ex.HResult);

                ex = Assert.Throws(
                    exceptionType,
                    () => NativeExportsNE.PreserveSig.False.IntArray_AsReturn(input)
                );
                Assert.Equal(expectedHR, ex.HResult);
            }
            {
                ex = Assert.Throws(
                    exceptionType,
                    () => NativeExportsNE.PreserveSig.False.StringArray_Out(input, out string[] ret)
                );
                Assert.Equal(expectedHR, ex.HResult);

                ex = Assert.Throws(
                    exceptionType,
                    () => NativeExportsNE.PreserveSig.False.StringArray_AsReturn(input)
                );
                Assert.Equal(expectedHR, ex.HResult);
            }
            {
                ex = Assert.Throws(
                    exceptionType,
                    () =>
                        NativeExportsNE.PreserveSig.False.SafeHandle_Out(
                            input,
                            out NativeExportsNE.PreserveSig.DummySafeHandle ret
                        )
                );
                Assert.Equal(expectedHR, ex.HResult);

                ex = Assert.Throws(
                    exceptionType,
                    () => NativeExportsNE.PreserveSig.False.SafeHandle_AsReturn(input)
                );
                Assert.Equal(expectedHR, ex.HResult);
            }
        }

        [Theory]
        [InlineData(S_OK)]
        [InlineData(S_FALSE)]
        [InlineData(10)]
        public void PreserveSigFalse_Success(int input)
        {
            Assert.True(input >= 0);

            NativeExportsNE.PreserveSig.False.NoReturnValue(input);

            {
                int expected = input;

                int ret;
                NativeExportsNE.PreserveSig.False.Int_Out(input, out ret);
                Assert.Equal(expected, ret);

                ret = NativeExportsNE.PreserveSig.False.Int_AsReturn(input);
                Assert.Equal(expected, ret);
            }
            {
                bool expected = input != 0;

                bool ret;
                NativeExportsNE.PreserveSig.False.Bool_Out(input, out ret);
                Assert.Equal(expected, ret);

                ret = NativeExportsNE.PreserveSig.False.Bool_AsReturn(input);
                Assert.Equal(expected, ret);
            }
            {
                char expected = (char)input;

                char ret;
                NativeExportsNE.PreserveSig.False.Char_Out(input, out ret);
                Assert.Equal(expected, ret);

                ret = NativeExportsNE.PreserveSig.False.Char_AsReturn(input);
                Assert.Equal(expected, ret);
            }
            {
                string expected = input.ToString();

                string ret;
                NativeExportsNE.PreserveSig.False.String_Out(input, out ret);
                Assert.Equal(expected, ret);

                ret = NativeExportsNE.PreserveSig.False.String_AsReturn(input);
                Assert.Equal(expected, ret);
            }
            {
                int[] expected = new int[sizeof(int)];
                Array.Fill(expected, input);

                int[] ret;
                NativeExportsNE.PreserveSig.False.IntArray_Out(input, out ret);
                Assert.Equal(expected, ret);

                ret = NativeExportsNE.PreserveSig.False.IntArray_AsReturn(input);
                Assert.Equal(expected, ret);
            }
            {
                string[] expected = new string[sizeof(int)];
                Array.Fill(expected, input.ToString());

                string[] ret;
                NativeExportsNE.PreserveSig.False.StringArray_Out(input, out ret);
                Assert.Equal(expected, ret);

                ret = NativeExportsNE.PreserveSig.False.StringArray_AsReturn(input);
                Assert.Equal(expected, ret);
            }
            {
                nint expected = input;

                NativeExportsNE.PreserveSig.DummySafeHandle ret;
                NativeExportsNE.PreserveSig.False.SafeHandle_Out(input, out ret);
                Assert.Equal(expected, (nint)ret.DangerousGetHandle());
                ret.Dispose();

                ret = NativeExportsNE.PreserveSig.False.SafeHandle_AsReturn(input);
                Assert.Equal(expected, (nint)ret.DangerousGetHandle());
                ret.Dispose();
            }
        }

        [Theory]
        [InlineData(S_OK)]
        [InlineData(S_FALSE)]
        [InlineData(E_INVALIDARG)]
        [InlineData(COR_E_NOTSUPPORTED)]
        [InlineData(-1)]
        public void PreserveSigTrue(int input)
        {
            int expected = input;
            int hr;

            hr = NativeExportsNE.PreserveSig.True.NoReturnValue(input);
            Assert.Equal(expected, hr);

            int ret;
            hr = NativeExportsNE.PreserveSig.True.Int_Out(input, out ret);
            Assert.Equal(expected, hr);
            Assert.Equal(expected, ret);
        }
    }
}
