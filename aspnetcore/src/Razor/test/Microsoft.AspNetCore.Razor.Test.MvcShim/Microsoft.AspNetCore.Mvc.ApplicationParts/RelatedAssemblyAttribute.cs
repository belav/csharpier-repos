using System;

namespace Microsoft.AspNetCore.Mvc.ApplicationParts;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class RelatedAssemblyAttribute : Attribute
{
    public RelatedAssemblyAttribute(string name) { }
}
