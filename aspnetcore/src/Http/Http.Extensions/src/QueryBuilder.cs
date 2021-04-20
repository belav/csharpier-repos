// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Http.Extensions
{
    // The IEnumerable interface is required for the collection initialization syntax: new QueryBuilder() { { "key", "value" } };
    /// <summary>
    /// Allows constructing a query string.
    /// </summary>
    public class QueryBuilder : IEnumerable<KeyValuePair<string, string>>
    {
        private IList<KeyValuePair<string, string>> _params;

        /// <summary>
        /// Initializes a new instance of <see cref="QueryBuilder"/>.
        /// </summary>
        public QueryBuilder()
        {
            _params = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="QueryBuilder"/>.
        /// </summary>
        /// <param name="parameters">The parameters to initialize the instance with.</param>
        public QueryBuilder(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            _params = new List<KeyValuePair<string, string>>(parameters);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="QueryBuilder"/>.
        /// </summary>
        /// <param name="parameters">The parameters to initialize the instance with.</param>
        public QueryBuilder(IEnumerable<KeyValuePair<string, StringValues>> parameters)
            : this(parameters.SelectMany(kvp => kvp.Value, (kvp, v) => KeyValuePair.Create(kvp.Key, v)))
        {

        }

        /// <summary>
        /// Adds a query string token to the instance.
        /// </summary>
        /// <param name="key">The query key.</param>
        /// <param name="values">The sequence of query values.</param>
        public void Add(string key, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                _params.Add(new KeyValuePair<string, string>(key, value));
            }
        }

        /// <summary>
        /// Adds a query string token to the instance.
        /// </summary>
        /// <param name="key">The query key.</param>
        /// <param name="value">The query value.</param>
        public void Add(string key, string value)
        {
            _params.Add(new KeyValuePair<string, string>(key, value));
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var builder = new StringBuilder();
            bool first = true;
            for (var i = 0; i < _params.Count; i++)
            {
                var pair = _params[i];
                builder.Append(first ? '?' : '&');
                first = false;
                builder.Append(UrlEncoder.Default.Encode(pair.Key));
                builder.Append('=');
                builder.Append(UrlEncoder.Default.Encode(pair.Value));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Constructs a <see cref="QueryString"/> from this <see cref="QueryBuilder"/>.
        /// </summary>
        /// <returns>The <see cref="QueryString"/>.</returns>
        public QueryString ToQueryString()
        {
            return new QueryString(ToString());
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return ToQueryString().GetHashCode();
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return ToQueryString().Equals(obj);
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _params.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _params.GetEnumerator();
        }
    }
}
