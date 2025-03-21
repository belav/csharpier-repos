//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace System.ServiceModel.Security.Tokens
{
    using System;
    using System.IdentityModel.Tokens;
    using System.Xml;

    public delegate void IssuedSecurityTokenHandler(
        SecurityToken issuedToken,
        EndpointAddress tokenRequestor
    );
    public delegate void RenewedSecurityTokenHandler(
        SecurityToken newSecurityToken,
        SecurityToken oldSecurityToken
    );

    public interface IIssuanceSecurityTokenAuthenticator
    {
        IssuedSecurityTokenHandler IssuedSecurityTokenHandler { get; set; }
        RenewedSecurityTokenHandler RenewedSecurityTokenHandler { get; set; }
    }
}
