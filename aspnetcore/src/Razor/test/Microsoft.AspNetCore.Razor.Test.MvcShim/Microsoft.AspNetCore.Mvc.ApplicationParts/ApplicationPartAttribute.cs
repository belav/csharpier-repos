using System;

namespace Microsoft.AspNetCore.Mvc.ApplicationParts;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class ApplicationPartAttribute : Attribute
{
    public ApplicationPartAttribute(string assemblyName) { }
}
