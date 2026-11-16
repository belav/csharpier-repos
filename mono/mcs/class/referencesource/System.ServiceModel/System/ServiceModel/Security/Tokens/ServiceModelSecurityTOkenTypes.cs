//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System.Globalization;
using System.IdentityModel.Tokens;
using System.ServiceModel;

namespace System.ServiceModel.Security.Tokens
{
    public static class ServiceModelSecurityTokenTypes
    {
        const string Namespace = "http://schemas.microsoft.com/ws/2006/05/servicemodel/tokens";
        const string spnego = Namespace + "/Spnego";
        const string mutualSslnego = Namespace + "/MutualSslnego";
        const string anonymousSslnego = Namespace + "/AnonymousSslnego";
        const string securityContext = Namespace + "/SecurityContextToken";
        const string secureConversation = Namespace + "/SecureConversation";
        const string sspiCredential = Namespace + "/SspiCredential";

        public static string Spnego
        {
            get { return spnego; }
        }
        public static string MutualSslnego
        {
            get { return mutualSslnego; }
        }
        public static string AnonymousSslnego
        {
            get { return anonymousSslnego; }
        }
        public static string SecurityContext
        {
            get { return securityContext; }
        }
        public static string SecureConversation
        {
            get { return secureConversation; }
        }
        public static string SspiCredential
        {
            get { return sspiCredential; }
        }
    }
}
