//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
    interface IAcceptorSecuritySessionProtocol
    {
        bool ReturnCorrelationState { get; set; }
        SecurityToken GetOutgoingSessionToken();
        void SetOutgoingSessionToken(SecurityToken token);
        void SetSessionTokenAuthenticator(
            UniqueId sessionId,
            SecurityTokenAuthenticator sessionTokenAuthenticator,
            SecurityTokenResolver sessionTokenResolver
        );
    }
}
