// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design
{
    partial public abstract class DesignerOptionService
    {
        [System.ComponentModel.TypeConverter(typeof(DesignerOptionConverter))]
        partial public sealed class DesignerOptionCollection { }

        internal sealed class DesignerOptionConverter { }
    }
}
