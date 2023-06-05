// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Components.Server;

partial internal sealed class ComponentParameterDeserializer
{
    private readonly ILogger<ComponentParameterDeserializer> _logger;
    private readonly ComponentParametersTypeCache _parametersCache;

    public ComponentParameterDeserializer(
        ILogger<ComponentParameterDeserializer> logger,
        ComponentParametersTypeCache parametersCache
    )
    {
        _logger = logger;
        _parametersCache = parametersCache;
    }

    public bool TryDeserializeParameters(
        IList<ComponentParameter> parametersDefinitions,
        IList<object> parameterValues,
        out ParameterView parameters
    )
    {
        parameters = default;
        var parametersDictionary = new Dictionary<string, object>();

        if (parameterValues.Count != parametersDefinitions.Count)
        {
            // Mismatched number of definition/parameter values.
            Log.MismatchedParameterAndDefinitions(
                _logger,
                parametersDefinitions.Count,
                parameterValues.Count
            );
            return false;
        }

        for (var i = 0; i < parametersDefinitions.Count; i++)
        {
            var definition = parametersDefinitions[i];
            if (definition.Name == null)
            {
                Log.MissingParameterDefinitionName(_logger);
                return false;
            }

            if (definition.TypeName == null && definition.Assembly == null)
            {
                parametersDictionary.Add(definition.Name, null);
            }
            else if (definition.TypeName == null || definition.Assembly == null)
            {
                Log.IncompleteParameterDefinition(
                    _logger,
                    definition.Name,
                    definition.TypeName,
                    definition.Assembly
                );
                return false;
            }
            else
            {
                var parameterType = _parametersCache.GetParameterType(
                    definition.Assembly,
                    definition.TypeName
                );
                if (parameterType == null)
                {
                    Log.InvalidParameterType(
                        _logger,
                        definition.Name,
                        definition.TypeName,
                        definition.Assembly
                    );
                    return false;
                }
                try
                {
                    // At this point we know the parameter is not null, as we don't serialize the type name or the assembly name
                    // for null parameters.
                    var value = (JsonElement)parameterValues[i];
                    var parameterValue = JsonSerializer.Deserialize(
                        value.GetRawText(),
                        parameterType,
                        ServerComponentSerializationSettings.JsonSerializationOptions
                    );

                    parametersDictionary.Add(definition.Name, parameterValue);
                }
                catch (Exception e)
                {
                    Log.InvalidParameterValue(
                        _logger,
                        definition.Name,
                        definition.TypeName,
                        definition.Assembly,
                        e
                    );
                    return false;
                }
            }
        }

        parameters = ParameterView.FromDictionary(parametersDictionary);
        return true;
    }

    partial private static class Log
    {
        [LoggerMessage(
            1,
            LogLevel.Debug,
            "Parameter values must be an array.",
            EventName = "ParameterValuesInvalidFormat"
        )]
        partial internal static void ParameterValuesInvalidFormat(
            ILogger<ComponentParameterDeserializer> logger
        );

        [LoggerMessage(
            2,
            LogLevel.Debug,
            "The parameter definition for '{ParameterName}' is incomplete: Type='{TypeName}' Assembly='{Assembly}'.",
            EventName = "IncompleteParameterDefinition"
        )]
        partial internal static void IncompleteParameterDefinition(
            ILogger<ComponentParameterDeserializer> logger,
            string parameterName,
            string typeName,
            string assembly
        );

        [LoggerMessage(
            3,
            LogLevel.Debug,
            "The parameter '{ParameterName} with type '{TypeName}' in assembly '{Assembly}' could not be found.",
            EventName = "InvalidParameterType"
        )]
        partial internal static void InvalidParameterType(
            ILogger<ComponentParameterDeserializer> logger,
            string parameterName,
            string typeName,
            string assembly
        );

        [LoggerMessage(
            4,
            LogLevel.Debug,
            "Could not parse the parameter value for parameter '{Name}' of type '{TypeName}' and assembly '{Assembly}'.",
            EventName = "InvalidParameterValue"
        )]
        partial internal static void InvalidParameterValue(
            ILogger<ComponentParameterDeserializer> logger,
            string name,
            string typeName,
            string assembly,
            Exception e
        );

        [LoggerMessage(
            5,
            LogLevel.Debug,
            "Failed to parse the parameter definitions.",
            EventName = "FailedToParseParameterDefinitions"
        )]
        partial internal static void FailedToParseParameterDefinitions(
            ILogger<ComponentParameterDeserializer> logger,
            Exception e
        );

        [LoggerMessage(
            6,
            LogLevel.Debug,
            "Failed to parse the parameter values.",
            EventName = "FailedToParseParameterValues"
        )]
        partial internal static void FailedToParseParameterValues(
            ILogger<ComponentParameterDeserializer> logger,
            Exception e
        );

        [LoggerMessage(
            7,
            LogLevel.Debug,
            "The number of parameter definitions '{DescriptorsLength}' does not match the number parameter values '{ValuesLength}'.",
            EventName = "MismatchedParameterAndDefinitions"
        )]
        partial internal static void MismatchedParameterAndDefinitions(
            ILogger<ComponentParameterDeserializer> logger,
            int descriptorsLength,
            int valuesLength
        );

        [LoggerMessage(
            8,
            LogLevel.Debug,
            "The name is missing in a parameter definition.",
            EventName = "MissingParameterDefinitionName"
        )]
        partial internal static void MissingParameterDefinitionName(
            ILogger<ComponentParameterDeserializer> logger
        );
    }
}
