using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
#if NETFRAMEWORK || NETSTANDARD
using System.Runtime.Serialization;
#else
using System.Runtime.CompilerServices;
#endif

namespace Microsoft.Extensions.Internal;

internal static partial class ParameterDefaultValue
{
    public static bool TryGetDefaultValue(ParameterInfo parameter, out object? defaultValue)
    {
        var hasDefaultValue = CheckHasDefaultValue(parameter, out var tryToGetDefaultValue);
        defaultValue = null;

        if (parameter.HasDefaultValue)
        {
            if (tryToGetDefaultValue)
            {
                defaultValue = parameter.DefaultValue;
            }

            bool isNullableParameterType =
                parameter.ParameterType.IsGenericType
                && parameter.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>);

            // Workaround for https://github.com/dotnet/runtime/issues/18599
            if (
                defaultValue == null
                && parameter.ParameterType.IsValueType
                && !isNullableParameterType
            ) // Nullable types should be left null
            {
                defaultValue = CreateValueType(parameter.ParameterType);
            }

            // Handle nullable enums
            if (defaultValue != null && isNullableParameterType)
            {
                Type? underlyingType = Nullable.GetUnderlyingType(parameter.ParameterType);
                if (underlyingType != null && underlyingType.IsEnum)
                {
                    defaultValue = Enum.ToObject(underlyingType, defaultValue);
                }
            }
        }

        return hasDefaultValue;
    }

#if NETFRAMEWORK || NETSTANDARD
    private static bool CheckHasDefaultValue(ParameterInfo parameter, out bool tryToGetDefaultValue)
    {
        tryToGetDefaultValue = true;
        try
        {
            return parameter.HasDefaultValue;
        }
        catch (FormatException) when (parameter.ParameterType == typeof(DateTime))
        {
            // Workaround for https://github.com/dotnet/runtime/issues/18844
            // If HasDefaultValue throws FormatException for DateTime
            // we expect it to have default value
            tryToGetDefaultValue = false;
            return true;
        }
    }

    private static object? CreateValueType(Type t) =>
        FormatterServices.GetSafeUninitializedObject(t);

#else
    private static bool CheckHasDefaultValue(ParameterInfo parameter, out bool tryToGetDefaultValue)
    {
        tryToGetDefaultValue = true;
        return parameter.HasDefaultValue;
    }

    [UnconditionalSuppressMessage(
        "ReflectionAnalysis",
        "IL2067:UnrecognizedReflectionPattern",
        Justification = "CreateValueType is only called on a ValueType. You can always create an instance of a ValueType."
    )]
    private static object? CreateValueType(Type t) => RuntimeHelpers.GetUninitializedObject(t);
#endif
}
