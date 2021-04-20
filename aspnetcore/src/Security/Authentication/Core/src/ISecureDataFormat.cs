// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AspNetCore.Authentication
{
    /// <summary>
    /// A contract for securing data.
    /// </summary>
    /// <typeparam name="TData">The type of the data to protect.</typeparam>
    public interface ISecureDataFormat<TData>
    {
        /// <summary>
        /// Protects the specified <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The value to protect</param>
        /// <returns>The data protected value.</returns>
        string Protect(TData data);

        /// <summary>
        /// Protects the specified <paramref name="data"/> for the specified <paramref name="purpose"/>.
        /// </summary>
        /// <param name="data">The value to protect</param>
        /// <param name="purpose">The purpose.</param>
        /// <returns>A data protected value.</returns>
        string Protect(TData data, string? purpose);

        /// <summary>
        /// Unprotects the specified <paramref name="protectedText"/>.
        /// </summary>
        /// <param name="protectedText">The data protected value.</param>
        /// <returns>An instance of <typeparamref name="TData"/>.</returns>
        TData? Unprotect(string? protectedText);

        /// <summary>
        /// Unprotects the specified <paramref name="protectedText"/> using the specified <paramref name="purpose"/>.
        /// </summary>
        /// <param name="protectedText">The data protected value.</param>
        /// <param name="purpose">The purpose.</param>
        /// <returns>An instance of <typeparamref name="TData"/>.</returns>
        TData? Unprotect(string? protectedText, string? purpose);
    }
}
