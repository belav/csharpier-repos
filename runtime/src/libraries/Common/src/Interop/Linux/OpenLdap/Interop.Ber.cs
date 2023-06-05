// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.DirectoryServices.Protocols;
using System.Diagnostics;

partial internal static class Interop
{
    partial internal static class Ldap
    {
        public const int ber_default_successful_return_code = 0;

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_alloc_t")]
        partial public static IntPtr ber_alloc(int option);

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_init")]
        partial public static IntPtr ber_init(BerVal value);

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_free")]
        partial public static IntPtr ber_free(IntPtr berelement, int option);

        public static int ber_printf_emptyarg(SafeBerHandle berElement, string format, nuint tag)
        {
            if (format == "{")
            {
                return ber_start_seq(berElement, tag);
            }
            else if (format == "}")
            {
                return ber_put_seq(berElement, tag);
            }
            else if (format == "[")
            {
                return ber_start_set(berElement, tag);
            }
            else if (format == "]")
            {
                return ber_put_set(berElement, tag);
            }
            else
            {
                Debug.Assert(format == "n");
                return ber_put_null(berElement, tag);
            }
        }

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_start_seq")]
        partial public static int ber_start_seq(SafeBerHandle berElement, nuint tag);

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_start_set")]
        partial public static int ber_start_set(SafeBerHandle berElement, nuint tag);

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_put_seq")]
        partial public static int ber_put_seq(SafeBerHandle berElement, nuint tag);

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_put_set")]
        partial public static int ber_put_set(SafeBerHandle berElement, nuint tag);

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_put_null")]
        partial public static int ber_put_null(SafeBerHandle berElement, nuint tag);

        public static int ber_printf_int(
            SafeBerHandle berElement,
            string format,
            int value,
            nuint tag
        )
        {
            if (format == "i")
            {
                return ber_put_int(berElement, value, tag);
            }
            else if (format == "e")
            {
                return ber_put_enum(berElement, value, tag);
            }
            else
            {
                Debug.Assert(format == "b");
                return ber_put_boolean(berElement, value, tag);
            }
        }

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_put_int")]
        partial public static int ber_put_int(SafeBerHandle berElement, int value, nuint tag);

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_put_enum")]
        partial public static int ber_put_enum(SafeBerHandle berElement, int value, nuint tag);

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_put_boolean")]
        partial public static int ber_put_boolean(SafeBerHandle berElement, int value, nuint tag);

        public static int ber_printf_bytearray(
            SafeBerHandle berElement,
            string format,
            HGlobalMemHandle value,
            nuint length,
            nuint tag
        )
        {
            if (format == "o")
            {
                return ber_put_ostring(berElement, value, length, tag);
            }
            else if (format == "s")
            {
                return ber_put_string(berElement, value, tag);
            }
            else
            {
                Debug.Assert(format == "X");
                return ber_put_bitstring(berElement, value, length, tag);
            }
        }

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_put_ostring")]
        partial private static int ber_put_ostring(
            SafeBerHandle berElement,
            HGlobalMemHandle value,
            nuint length,
            nuint tag
        );

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_put_string")]
        partial private static int ber_put_string(
            SafeBerHandle berElement,
            HGlobalMemHandle value,
            nuint tag
        );

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_put_bitstring")]
        partial private static int ber_put_bitstring(
            SafeBerHandle berElement,
            HGlobalMemHandle value,
            nuint length,
            nuint tag
        );

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_flatten")]
        partial public static int ber_flatten(SafeBerHandle berElement, ref IntPtr value);

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_bvfree")]
        partial public static int ber_bvfree(IntPtr value);

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_bvecfree")]
        partial public static int ber_bvecfree(IntPtr value);

        public static int ber_scanf_emptyarg(SafeBerHandle berElement, string format)
        {
            Debug.Assert(
                format == "{"
                    || format == "}"
                    || format == "["
                    || format == "]"
                    || format == "n"
                    || format == "x"
            );
            if (format == "{" || format == "[")
            {
                nuint len = 0;
                return ber_skip_tag(berElement, ref len);
            }
            else if (format == "]" || format == "}")
            {
                return ber_default_successful_return_code;
            }
            else
            {
                Debug.Assert(format == "n" || format == "x");
                return ber_get_null(berElement);
            }
        }

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_skip_tag")]
        partial private static int ber_skip_tag(SafeBerHandle berElement, ref nuint len);

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_get_null")]
        partial private static int ber_get_null(SafeBerHandle berElement);

        public static int ber_scanf_int(SafeBerHandle berElement, string format, ref int value)
        {
            if (format == "i")
            {
                return ber_get_int(berElement, ref value);
            }
            else if (format == "e")
            {
                return ber_get_enum(berElement, ref value);
            }
            else
            {
                Debug.Assert(format == "b");
                return ber_get_boolean(berElement, ref value);
            }
        }

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_get_int")]
        partial private static int ber_get_int(SafeBerHandle berElement, ref int value);

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_get_enum")]
        partial private static int ber_get_enum(SafeBerHandle berElement, ref int value);

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_get_boolean")]
        partial private static int ber_get_boolean(SafeBerHandle berElement, ref int value);

        public static int ber_scanf_bitstring(
            SafeBerHandle berElement,
            string format,
            ref IntPtr value,
            ref uint bitLength
        )
        {
            Debug.Assert(format == "B");
            nuint bitLengthAsNuint = 0;
            int res = ber_get_stringb(berElement, ref value, ref bitLengthAsNuint);
            bitLength = (uint)bitLengthAsNuint;
            return res;
        }

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_get_stringb")]
        partial private static int ber_get_stringb(
            SafeBerHandle berElement,
            ref IntPtr value,
            ref nuint bitLength
        );

        public static int ber_scanf_ptr(SafeBerHandle berElement, string format, ref IntPtr value)
        {
            Debug.Assert(format == "O");
            return ber_get_stringal(berElement, ref value);
        }

        [LibraryImport(Libraries.OpenLdap, EntryPoint = "ber_get_stringal")]
        partial private static int ber_get_stringal(SafeBerHandle berElement, ref IntPtr value);

#pragma warning disable IDE0060
        public static int ber_printf_berarray(
            SafeBerHandle berElement,
            string format,
            IntPtr value,
            nuint tag
        )
        {
            Debug.Assert(format == "v" || format == "V");
            // V and v are not supported on Unix yet.
            return -1;
        }

        public static int ber_scanf_multibytearray(
            SafeBerHandle berElement,
            string format,
            ref IntPtr value
        )
        {
            Debug.Assert(format == "v" || format == "V");
            // V and v are not supported on Unix yet.
            return -1;
        }
#pragma warning restore IDE0060
    }
}
