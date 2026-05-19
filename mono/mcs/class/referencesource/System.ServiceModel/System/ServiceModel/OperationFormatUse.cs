//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel
{
    public enum OperationFormatUse
    {
        Literal,
        Encoded,
    }

    static class OperationFormatUseHelper
    {
        public static bool IsDefined(OperationFormatUse x)
        {
            return x == OperationFormatUse.Literal || x == OperationFormatUse.Encoded || false;
        }
    }
}
