using System.Reflection;

namespace Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore.Tests.Helpers;

public class StringsHelpers
{
    public static string GetResourceString(string stringName, params object[] parameters)
    {
        var strings = typeof(DatabaseErrorPageMiddleware).Assembly
            .GetType("Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore.Strings")
            .GetTypeInfo();
        var method = strings.GetDeclaredMethods(stringName).SingleOrDefault();
        if (method != null)
        {
            return (string)method.Invoke(null, parameters);
        }
        var property = strings.GetDeclaredProperty(stringName);
        return (string)property.GetValue(null);
    }
}
