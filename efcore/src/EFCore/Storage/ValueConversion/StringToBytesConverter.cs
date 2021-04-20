// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;

namespace Microsoft.EntityFrameworkCore.Storage.ValueConversion
{
    /// <summary>
    ///     Converts strings to and from arrays of bytes.
    /// </summary>
    public class StringToBytesConverter : ValueConverter<string, byte[]>
    {
        /// <summary>
        ///     Creates a new instance of this converter.
        /// </summary>
        /// <param name="encoding"> The string encoding to use. </param>
        /// <param name="mappingHints">
        ///     Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
        ///     facets for the converted data.
        /// </param>
        public StringToBytesConverter(
            Encoding encoding,
            ConverterMappingHints? mappingHints = null)
            : base(
                // TODO-NULLABLE: Null is already sanitized externally, clean up as part of #13850
                v => v == null ? null! : encoding.GetBytes(v),
                v => v == null ? null! : encoding.GetString(v),
                mappingHints)
        {
        }

        /// <summary>
        ///     A <see cref="ValueConverterInfo" /> for the default use of this converter.
        /// </summary>
        public static ValueConverterInfo DefaultInfo { get; }
            = new(typeof(string), typeof(byte[]), i => new StringToBytesConverter(Encoding.UTF8, i.MappingHints));
    }
}
