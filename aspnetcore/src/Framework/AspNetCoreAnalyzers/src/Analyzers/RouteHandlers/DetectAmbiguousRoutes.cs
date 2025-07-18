// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Analyzers.Infrastructure;
using Microsoft.AspNetCore.Analyzers.Infrastructure.RoutePattern;
using Microsoft.AspNetCore.App.Analyzers.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Microsoft.AspNetCore.Analyzers.RouteHandlers;

using WellKnownType = WellKnownTypeData.WellKnownType;

public partial class RouteHandlerAnalyzer : DiagnosticAnalyzer
{
    private static void DetectAmbiguousRoutes(
        in OperationBlockAnalysisContext context,
        WellKnownTypes wellKnownTypes,
        ConcurrentDictionary<MapOperation, byte> mapOperations
    )
    {
        if (mapOperations.IsEmpty)
        {
            return;
        }

        var groupedByParent = mapOperations
            .Select(kvp => new
            {
                MapOperation = kvp.Key,
                ResolvedOperation = ResolveOperation(kvp.Key.Operation, wellKnownTypes),
            })
            .Where(u =>
                u.ResolvedOperation != null
                && !u.MapOperation.RouteUsageModel.UsageContext.HttpMethods.IsDefault
            )
            .GroupBy(u => new MapOperationGroupKey(
                u.MapOperation.Builder,
                u.ResolvedOperation!,
                u.MapOperation.RouteUsageModel.RoutePattern,
                u.MapOperation.RouteUsageModel.UsageContext.HttpMethods
            ));

        foreach (var ambiguousGroup in groupedByParent.Where(g => g.Count() >= 2))
        {
            foreach (var ambiguousMapOperation in ambiguousGroup)
            {
                var model = ambiguousMapOperation.MapOperation.RouteUsageModel;

                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.AmbiguousRouteHandlerRoute,
                        model.UsageContext.RouteToken.GetLocation(),
                        model.RoutePattern.Root.ToString()
                    )
                );
            }
        }
    }

    private static IOperation? ResolveOperation(IOperation operation, WellKnownTypes wellKnownTypes)
    {
        // We want to group routes in a block together because we know they're being used together.
        // There are some circumstances where we still don't want to use the route, either because it is only conditionally
        // being called, or the IEndpointConventionBuilder returned from the method is being used. We can't accurately
        // detect what extra endpoint metadata is being added to the routes.
        //
        // Don't use route endpoint if:
        // - It's in a conditional statement.
        // - It's in a coalesce statement.
        // - It's has methods called on it.
        // - It's assigned to a variable.
        // - It's an argument to a method call, unless in a known safe method.
        var current = operation;
        if (
            current.Parent
                is IArgumentOperation { Parent: IInvocationOperation invocationOperation }
            && IsAllowedEndpointBuilderMethod(invocationOperation, wellKnownTypes)
        )
        {
            return ResolveOperation(invocationOperation, wellKnownTypes);
        }

        while (current != null)
        {
            if (current.Parent is IBlockOperation or ISwitchCaseOperation)
            {
                return current.Parent;
            }
            else if (
                current.Parent
                is IConditionalOperation
                    or ICoalesceOperation
                    or IAssignmentOperation
                    or IArgumentOperation
                    or IInvocationOperation
                    or ISwitchExpressionArmOperation
            )
            {
                return current;
            }

            current = current.Parent;
        }

        return null;
    }

    /// <summary>
    /// Test the invocation operation. Safe methods are those that we know don't add metadata that impacts metadata.
    /// </summary>
    private static bool IsAllowedEndpointBuilderMethod(
        IInvocationOperation invocationOperation,
        WellKnownTypes wellKnownTypes
    )
    {
        var method = invocationOperation.TargetMethod;

        if (
            SymbolEqualityComparer.Default.Equals(
                method.ContainingType,
                wellKnownTypes.Get(
                    WellKnownType.Microsoft_AspNetCore_Builder_RoutingEndpointConventionBuilderExtensions
                )
            )
        )
        {
            return method.Name switch
            {
                "RequireHost" => false, // Adds IHostMetadata
                "WithDisplayName" => true,
                "WithMetadata" => false, // Can add anything
                "WithName" => true,
                "WithGroupName" => true,
                _ => false,
            };
        }
        else if (
            SymbolEqualityComparer.Default.Equals(
                method.ContainingType,
                wellKnownTypes.Get(
                    WellKnownType.Microsoft_AspNetCore_Builder_AuthorizationEndpointConventionBuilderExtensions
                )
            )
        )
        {
            return method.Name is "RequireAuthorization" or "AllowAnonymous";
        }
        else if (
            SymbolEqualityComparer.Default.Equals(
                method.ContainingType,
                wellKnownTypes.Get(
                    WellKnownType.Microsoft_AspNetCore_Http_OpenApiRouteHandlerBuilderExtensions
                )
            )
        )
        {
            return method.Name switch
            {
                "Accepts" => false, // Adds IAcceptsMetadata
                "ExcludeFromDescription" => true,
                "Produces" => true,
                "ProducesProblem" => true,
                "ProducesValidationProblem" => true,
                "WithDescription" => true,
                "WithSummary" => true,
                "WithTags" => true,
                _ => false,
            };
        }
        else if (
            SymbolEqualityComparer.Default.Equals(
                method.ContainingType,
                wellKnownTypes.Get(
                    WellKnownType.Microsoft_AspNetCore_Builder_CorsEndpointConventionBuilderExtensions
                )
            )
        )
        {
            return method.Name == "RequireCors";
        }
        else if (
            SymbolEqualityComparer.Default.Equals(
                method.ContainingType,
                wellKnownTypes.Get(
                    WellKnownType.Microsoft_Extensions_DependencyInjection_OutputCacheConventionBuilderExtensions
                )
            )
        )
        {
            return method.Name == "CacheOutput";
        }
        else if (
            SymbolEqualityComparer.Default.Equals(
                method.ContainingType,
                wellKnownTypes.Get(
                    WellKnownType.Microsoft_AspNetCore_Builder_RateLimiterEndpointConventionBuilderExtensions
                )
            )
        )
        {
            return method.Name is "RequireRateLimiting" or "DisableRateLimiting";
        }

        return false;
    }

    private readonly struct MapOperationGroupKey : IEquatable<MapOperationGroupKey>
    {
        public IOperation? ParentOperation { get; }
        public IOperation? Builder { get; }
        public RoutePatternTree RoutePattern { get; }
        public ImmutableArray<string> HttpMethods { get; }

        public MapOperationGroupKey(
            IOperation? builder,
            IOperation parentOperation,
            RoutePatternTree routePattern,
            ImmutableArray<string> httpMethods
        )
        {
            Debug.Assert(!httpMethods.IsDefault);

            ParentOperation = parentOperation;
            Builder = builder;
            RoutePattern = routePattern;
            HttpMethods = httpMethods;
        }

        public override bool Equals(object obj)
        {
            if (obj is MapOperationGroupKey key)
            {
                return Equals(key);
            }
            return false;
        }

        public bool Equals(MapOperationGroupKey other)
        {
            return ParentOperation != null
                && Equals(ParentOperation, other.ParentOperation)
                && Builder != null
                && SymbolEqualityComparer.Default.Equals(
                    (Builder as ILocalReferenceOperation)?.Local,
                    (other.Builder as ILocalReferenceOperation)?.Local
                )
                && AmbiguousRoutePatternComparer.Instance.Equals(RoutePattern, other.RoutePattern)
                && HasMatchingHttpMethods(HttpMethods, other.HttpMethods);
        }

        private static bool HasMatchingHttpMethods(
            ImmutableArray<string> httpMethods1,
            ImmutableArray<string> httpMethods2
        )
        {
            if (httpMethods1.IsEmpty || httpMethods2.IsEmpty)
            {
                return true;
            }

            foreach (var item1 in httpMethods1)
            {
                foreach (var item2 in httpMethods2)
                {
                    if (item2 == item1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (ParentOperation?.GetHashCode() ?? 0)
                ^ AmbiguousRoutePatternComparer.Instance.GetHashCode(RoutePattern);
        }
    }
}
