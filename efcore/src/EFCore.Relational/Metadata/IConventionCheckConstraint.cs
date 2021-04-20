// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    ///     Represents a check constraint in the <see cref="IConventionEntityType" />.
    /// </summary>
    public interface IConventionCheckConstraint : IReadOnlyCheckConstraint, IConventionAnnotatable
    {
        /// <summary>
        ///     Gets the entity type on which this check constraint is defined.
        /// </summary>
        new IConventionEntityType EntityType { get; }

        /// <summary>
        ///     Gets the configuration source for this check constraint.
        /// </summary>
        /// <returns> The configuration source for this check constraint. </returns>
        ConfigurationSource GetConfigurationSource();
    }
}
