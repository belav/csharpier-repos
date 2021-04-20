// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.EntityFrameworkCore.TestModels.ConcurrencyModel
{
    public class Sponsor
    {
        public static readonly string ClientTokenPropertyName = "ClientToken";

        private readonly ObservableCollection<Team> _teams = new();

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Team> Teams
            => _teams;
    }
}
