//------------------------------------------------------------------------------
// <copyright file="SettingsPropertyIsReadOnlyException.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Configuration
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.Runtime.Serialization;

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////
    [Serializable]
    public class SettingsPropertyIsReadOnlyException : Exception
    {
        public SettingsPropertyIsReadOnlyException(String message)
            : base(message) { }

        public SettingsPropertyIsReadOnlyException(String message, Exception innerException)
            : base(message, innerException) { }

        protected SettingsPropertyIsReadOnlyException(
            SerializationInfo info,
            StreamingContext context
        )
            : base(info, context) { }

        public SettingsPropertyIsReadOnlyException() { }
    }
}
