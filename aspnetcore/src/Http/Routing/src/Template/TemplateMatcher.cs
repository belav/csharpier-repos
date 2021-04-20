// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#nullable enable

using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Routing.Template
{
    /// <summary>
    /// Supports matching paths to route templates and extracting parameter values.
    /// </summary>
    public class TemplateMatcher
    {
        private const string SeparatorString = "/";
        private const char SeparatorChar = '/';

        // Perf: This is a cache to avoid looking things up in 'Defaults' each request.
        private readonly bool[] _hasDefaultValue;
        private readonly object?[] _defaultValues;

        private static readonly char[] Delimiters = new char[] { SeparatorChar };
        private RoutePatternMatcher _routePatternMatcher;

        /// <summary>
        /// Creates a new <see cref="TemplateMatcher"/> instance given a <paramref name="template"/> and <paramref name="defaults"/>.
        /// </summary>
        /// <param name="template">The <see cref="RouteTemplate"/> to compare against.</param>
        /// <param name="defaults">The default values for parameters in the <paramref name="template"/>.</param>
        public TemplateMatcher(
            RouteTemplate template,
            RouteValueDictionary defaults)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            Template = template;
            Defaults = defaults ?? new RouteValueDictionary();

            // Perf: cache the default value for each parameter (other than complex segments).
            _hasDefaultValue = new bool[Template.Segments.Count];
            _defaultValues = new object[Template.Segments.Count];

            for (var i = 0; i < Template.Segments.Count; i++)
            {
                var segment = Template.Segments[i];
                if (!segment.IsSimple)
                {
                    continue;
                }

                var part = segment.Parts[0];
                if (!part.IsParameter)
                {
                    continue;
                }

                if (Defaults.TryGetValue(part.Name!, out var value))
                {
                    _hasDefaultValue[i] = true;
                    _defaultValues[i] = value;
                }
            }

            var routePattern = Template.ToRoutePattern();
            _routePatternMatcher = new RoutePatternMatcher(routePattern, Defaults);
        }

        /// <summary>
        /// Gets the default values for parameters in the <see cref="Template"/>.
        /// </summary>
        public RouteValueDictionary Defaults { get; }

        /// <summary>
        /// Gets the <see cref="RouteTemplate"/> to match against.
        /// </summary>
        public RouteTemplate Template { get; }

        /// <summary>
        /// Evaluates if the provided <paramref name="path"/> matches the <see cref="Template"/>. Populates
        /// <paramref name="values"/> with parameter values.
        /// </summary>
        /// <param name="path">A <see cref="PathString"/> representing the route to match.</param>
        /// <param name="values">A <see cref="RouteValueDictionary"/> to populate with parameter values.</param>
        /// <returns><see langword="true"/> if <paramref name="path"/> matches <see cref="Template"/>.</returns>
        public bool TryMatch(PathString path, RouteValueDictionary values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            return _routePatternMatcher.TryMatch(path, values);
        }
    }
}
