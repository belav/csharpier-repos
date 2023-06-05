// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Security
{
    partial public interface ISecurityEncodable
    {
        void FromXml(SecurityElement e);
        SecurityElement? ToXml();
    }
}
