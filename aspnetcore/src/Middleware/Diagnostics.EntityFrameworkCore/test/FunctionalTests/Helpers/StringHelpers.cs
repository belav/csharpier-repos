using System.Reflection;

namespace Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore.FunctionalTests.Helpers;

public class StringsHelpers
{
    public static string GetResourceString(string stringName, params object[] parameters)
    {
        var strings = typeof(DatabaseErrorPageMiddleware).Assembly
            .GetType("Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore.Strings")
            .GetTypeInfo();

        if (parameters.Length > 0)
        {
            var method = strings.GetDeclaredMethods(stringName).Single();
            return (string)method.Invoke(null, parameters);
        }

        return (string)strings.GetDeclaredProperty(stringName).GetValue(null);
    }
}
