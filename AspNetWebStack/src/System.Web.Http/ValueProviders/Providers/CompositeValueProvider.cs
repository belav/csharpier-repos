// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Web.Http.ValueProviders.Providers
{
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "It is more fundamentally a value provider than a collection")]
    public class CompositeValueProvider : Collection<IValueProvider>, IValueProvider, IEnumerableValueProvider
    {
        public CompositeValueProvider()
        {
        }

        public CompositeValueProvider(IList<IValueProvider> list)
            : base(list)
        {
        }

        public virtual bool ContainsPrefix(string prefix)
        {
            foreach (IValueProvider vp in this)
            {
                if (vp.ContainsPrefix(prefix))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual ValueProviderResult GetValue(string key)
        {
            // Performance-sensitive
            // Caching the count is faster for IList<T>
            int itemCount = Items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                IValueProvider vp = Items[i];
                ValueProviderResult result = vp.GetValue(key);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public virtual IDictionary<string, string> GetKeysFromPrefix(string prefix)
        {
            foreach (IValueProvider vp in this)
            {
                IDictionary<string, string> result = GetKeysFromPrefixFromProvider(vp, prefix);
                if (result != null && result.Count > 0)
                {
                    return result;
                }
            }
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        internal static IDictionary<string, string> GetKeysFromPrefixFromProvider(IValueProvider provider, string prefix)
        {
            IEnumerableValueProvider enumeratedProvider = provider as IEnumerableValueProvider;
            return (enumeratedProvider != null) ? enumeratedProvider.GetKeysFromPrefix(prefix) : null;
        }

        protected override void InsertItem(int index, IValueProvider item)
        {
            if (item == null)
            {
                throw Error.ArgumentNull("item");
            }
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, IValueProvider item)
        {
            if (item == null)
            {
                throw Error.ArgumentNull("item");
            }
            base.SetItem(index, item);
        }
    }
}
