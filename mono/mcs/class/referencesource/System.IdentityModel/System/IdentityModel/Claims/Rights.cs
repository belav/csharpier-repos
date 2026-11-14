//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.IdentityModel.Claims
{
    public static class Rights
    {
        const string rightNamespace = XsiConstants.Namespace + "/right";

        const string identity = rightNamespace + "/identity";
        const string possessProperty = rightNamespace + "/possessproperty";

        public static string Identity
        {
            get { return identity; }
        }
        public static string PossessProperty
        {
            get { return possessProperty; }
        }
    }
}
