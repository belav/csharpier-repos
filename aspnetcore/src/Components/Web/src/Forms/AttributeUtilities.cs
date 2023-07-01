using System.Globalization;

namespace Microsoft.AspNetCore.Components.Forms;

internal static class AttributeUtilities
{
    public static string? CombineClassNames(
        IReadOnlyDictionary<string, object>? additionalAttributes,
        string? classNames
    )
    {
        if (
            additionalAttributes is null
            || !additionalAttributes.TryGetValue("class", out var @class)
        )
        {
            return classNames;
        }

        var classAttributeValue = Convert.ToString(@class, CultureInfo.InvariantCulture);

        if (string.IsNullOrEmpty(classAttributeValue))
        {
            return classNames;
        }

        if (string.IsNullOrEmpty(classNames))
        {
            return classAttributeValue;
        }

        return $"{classAttributeValue} {classNames}";
    }
}
