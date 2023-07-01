using System;

namespace Microsoft.AspNetCore.Mvc.ApplicationParts;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class ProvideApplicationPartFactoryAttribute : Attribute
{
    public ProvideApplicationPartFactoryAttribute(Type factoryType) { }

    public ProvideApplicationPartFactoryAttribute(string factoryTypeName) { }
}
