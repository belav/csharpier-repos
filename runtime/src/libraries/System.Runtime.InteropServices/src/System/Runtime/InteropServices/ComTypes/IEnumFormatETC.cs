// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Runtime.InteropServices.ComTypes
{
    /// <summary>
    ///     The IEnumFORMATETC interface is used to enumerate an array of FORMATETC
    ///     structures. IEnumFORMATETC has the same methods as all enumerator interfaces:
    ///     Next, Skip, Reset, and Clone.
    /// </summary>
    [ComImport]
    [Guid("00000103-0000-0000-C000-000000000046")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumFORMATETC
    {
        /// <summary>
        ///     Retrieves the next celt items in the enumeration sequence. If there are
        ///     fewer than the requested number of elements left in the sequence, it
        ///     retrieves the remaining elements. The number of elements actually
        ///     retrieved is returned through pceltFetched (unless the caller passed
        ///     in NULL for that parameter).
        /// </summary>
        [PreserveSig]
        int Next(int celt, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] FORMATETC[] rgelt, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pceltFetched);

        /// <summary>
        ///     Skips over the next specified number of elements in the enumeration sequence.
        /// </summary>
        [PreserveSig]
        int Skip(int celt);

        /// <summary>
        ///     Resets the enumeration sequence to the beginning.
        /// </summary>
        [PreserveSig]
        int Reset();

        /// <summary>
        ///     Creates another enumerator that contains the same enumeration state as
        ///     the current one. Using this function, a client can record a particular
        ///     point in the enumeration sequence and then return to that point at a
        ///     later time. The new enumerator supports the same interface as the original one.
        /// </summary>
        void Clone(out IEnumFORMATETC newEnum);
    }
}
