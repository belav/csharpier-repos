// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Authentication.Certificate
{
    /// <summary>
    /// Cache used to store <see cref="AuthenticateResult"/> results after the certificate has been validated
    /// </summary>
    public interface ICertificateValidationCache
    {
        /// <summary>
        /// Get the <see cref="AuthenticateResult"/> for the connection and certificate.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="certificate">The certificate.</param>
        /// <returns>the <see cref="AuthenticateResult"/></returns>
        AuthenticateResult? Get(HttpContext context, X509Certificate2 certificate);

        /// <summary>
        /// Store a <see cref="AuthenticateResult"/> for the connection and certificate
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="result">The <see cref="AuthenticateResult"/></param>
        void Put(HttpContext context, X509Certificate2 certificate, AuthenticateResult result);
    }
}
