//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel
{
    public enum InstanceContextMode
    {
        PerSession,
        PerCall,
        Single,
    }

    static class InstanceContextModeHelper
    {
        public static bool IsDefined(InstanceContextMode x)
        {
            return x == InstanceContextMode.PerCall
                || x == InstanceContextMode.PerSession
                || x == InstanceContextMode.Single
                || false;
        }
    }
}
