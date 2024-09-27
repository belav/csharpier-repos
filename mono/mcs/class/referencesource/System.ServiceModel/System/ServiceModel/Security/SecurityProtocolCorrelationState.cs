//----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Security
{
    using System.IdentityModel.Claims;
    using System.IdentityModel.Policy;
    using System.IdentityModel.Tokens;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.ServiceModel.Security.Tokens;

    class SecurityProtocolCorrelationState
    {
        SecurityToken token;
        SignatureConfirmations signatureConfirmations;
        ServiceModelActivity activity;

        public SecurityProtocolCorrelationState(SecurityToken token)
        {
            this.token = token;
            this.activity = DiagnosticUtility.ShouldUseActivity
                ? ServiceModelActivity.Current
                : null;
        }

        public SecurityToken Token
        {
            get { return this.token; }
        }

        internal SignatureConfirmations SignatureConfirmations
        {
            get { return this.signatureConfirmations; }
            set { this.signatureConfirmations = value; }
        }

        internal ServiceModelActivity Activity
        {
            get { return this.activity; }
        }
    }
}
