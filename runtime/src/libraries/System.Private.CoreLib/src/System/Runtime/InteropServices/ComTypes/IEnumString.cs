// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Runtime.InteropServices.ComTypes
{
    [Guid("00000101-0000-0000-C000-000000000046")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IEnumString
    {
        [PreserveSig]
        int Next(int celt, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 0), Out] string[] rgelt, IntPtr pceltFetched);
        [PreserveSig]
        int Skip(int celt);
        void Reset();
        void Clone(out IEnumString ppenum);
    }
}
