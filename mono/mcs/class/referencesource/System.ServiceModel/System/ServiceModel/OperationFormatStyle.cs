//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel
{
    public enum OperationFormatStyle
    {
        Document,
        Rpc,
    }

    static class OperationFormatStyleHelper
    {
        public static bool IsDefined(OperationFormatStyle x)
        {
            return x == OperationFormatStyle.Document || x == OperationFormatStyle.Rpc || false;
        }
    }
}
