﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.EntityFrameworkCore.TestModels.ManyToManyModel
{
    public class JoinOneToTwoExtra
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

        public virtual ICollection<JoinOneToTwo> JoinEntities { get; set; }
    }
}
