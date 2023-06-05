// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Internal.TypeSystem
{
    partial
    // Additional members of FieldDesc related to serialization.
    public class FieldDesc
    {
        /// <summary>
        /// Gets a value indicating whether this field is not serialized.
        /// specially.
        /// </summary>
        public virtual bool IsNotSerialized
        {
            get { return false; }
        }
    }

    partial public class FieldForInstantiatedType
    {
        public override bool IsNotSerialized
        {
            get { return _fieldDef.IsNotSerialized; }
        }
    }
}
