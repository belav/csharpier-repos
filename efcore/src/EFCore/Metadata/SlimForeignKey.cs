// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    ///     Represents a relationship where a foreign key composed of properties on the dependent entity type
    ///     references a corresponding primary or alternate key on the principal entity type.
    /// </summary>
    public class SlimForeignKey : AnnotatableBase, IRuntimeForeignKey
    {
        private readonly DeleteBehavior _deleteBehavior;
        private readonly bool _isUnique;
        private readonly bool _isRequired;
        private readonly bool _isRequiredDependent;
        private readonly bool _isOwnership;

        private object? _dependentKeyValueFactory;
        private Func<IDependentsMap>? _dependentsMapFactory;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        [EntityFrameworkInternal]
        public SlimForeignKey(
            IReadOnlyList<SlimProperty> dependentProperties,
            SlimKey principalKey,
            SlimEntityType dependentEntityType,
            SlimEntityType principalEntityType,
            DeleteBehavior deleteBehavior,
            bool unique,
            bool required,
            bool requiredDependent,
            bool ownership)
        {
            Properties = dependentProperties;
            PrincipalKey = principalKey;
            DeclaringEntityType = dependentEntityType;
            PrincipalEntityType = principalEntityType;
            _isRequired = required;
            _isRequiredDependent = requiredDependent;
            _deleteBehavior = deleteBehavior;
            _isUnique = unique;
            _isOwnership = ownership;
        }

        /// <summary>
        ///     Gets the foreign key properties in the dependent entity.
        /// </summary>
        public virtual IReadOnlyList<SlimProperty> Properties { get; }

        /// <summary>
        ///     Gets the primary or alternate key that the relationship targets.
        /// </summary>
        public virtual SlimKey PrincipalKey { get; }

        /// <summary>
        ///     Gets the dependent entity type. This may be different from the type that <see cref="Properties" />
        ///     are defined on when the relationship is defined a derived type in an inheritance hierarchy (since the properties
        ///     may be defined on a base type).
        /// </summary>
        public virtual SlimEntityType DeclaringEntityType { get; }

        /// <summary>
        ///     Gets the principal entity type that this relationship targets. This may be different from the type that
        ///     <see cref="PrincipalKey" /> is defined on when the relationship targets a derived type in an inheritance
        ///     hierarchy (since the key is defined on the base type of the hierarchy).
        /// </summary>
        public virtual SlimEntityType PrincipalEntityType { get; }

        [DisallowNull]
        private SlimNavigation? DependentToPrincipal { get; set; }

        [DisallowNull]
        private SlimNavigation? PrincipalToDependent { get; set; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        [EntityFrameworkInternal]
        public virtual void AddNavigation(
            SlimNavigation navigation,
            bool onDependent)
        {
            if (onDependent)
            {
                DependentToPrincipal = navigation;
            }
            else
            {
                PrincipalToDependent = navigation;
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        [EntityFrameworkInternal]
        public virtual SortedSet<SlimSkipNavigation>? ReferencingSkipNavigations { get; set; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        [EntityFrameworkInternal]
        public virtual DebugView DebugView
            => new(
                () => ((IReadOnlyForeignKey)this).ToDebugString(MetadataDebugStringOptions.ShortDefault),
                () => ((IReadOnlyForeignKey)this).ToDebugString(MetadataDebugStringOptions.LongDefault));

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
            => ((IReadOnlyForeignKey)this).ToDebugString(MetadataDebugStringOptions.SingleLineDefault);

        /// <inheritdoc/>
        IReadOnlyList<IReadOnlyProperty> IReadOnlyForeignKey.Properties
        {
            [DebuggerStepThrough]
            get => Properties;
        }

        /// <inheritdoc/>
        IReadOnlyList<IProperty> IForeignKey.Properties
        {
            [DebuggerStepThrough]
            get => Properties;
        }

        /// <inheritdoc/>
        IReadOnlyKey IReadOnlyForeignKey.PrincipalKey
        {
            [DebuggerStepThrough]
            get => PrincipalKey;
        }

        /// <inheritdoc/>
        IKey IForeignKey.PrincipalKey
        {
            [DebuggerStepThrough]
            get => PrincipalKey;
        }

        /// <inheritdoc/>
        IReadOnlyEntityType IReadOnlyForeignKey.DeclaringEntityType
        {
            [DebuggerStepThrough]
            get => DeclaringEntityType;
        }

        /// <inheritdoc/>
        IEntityType IForeignKey.DeclaringEntityType
        {
            [DebuggerStepThrough]
            get => DeclaringEntityType;
        }

        /// <inheritdoc/>
        IReadOnlyEntityType IReadOnlyForeignKey.PrincipalEntityType
        {
            [DebuggerStepThrough]
            get => PrincipalEntityType;
        }

        /// <inheritdoc/>
        IEntityType IForeignKey.PrincipalEntityType
        {
            [DebuggerStepThrough]
            get => PrincipalEntityType;
        }

        /// <inheritdoc/>
        IReadOnlyNavigation? IReadOnlyForeignKey.DependentToPrincipal
        {
            [DebuggerStepThrough]
            get => DependentToPrincipal;
        }

        /// <inheritdoc/>
        INavigation? IForeignKey.DependentToPrincipal
        {
            [DebuggerStepThrough]
            get => DependentToPrincipal;
        }

        /// <inheritdoc/>
        IReadOnlyNavigation? IReadOnlyForeignKey.PrincipalToDependent
        {
            [DebuggerStepThrough]
            get => PrincipalToDependent;
        }

        /// <inheritdoc/>
        INavigation? IForeignKey.PrincipalToDependent
        {
            [DebuggerStepThrough]
            get => PrincipalToDependent;
        }

        /// <inheritdoc/>
        bool IReadOnlyForeignKey.IsUnique
        {
            [DebuggerStepThrough]
            get => _isUnique;
        }

        /// <inheritdoc/>
        bool IReadOnlyForeignKey.IsRequired
        {
            [DebuggerStepThrough]
            get => _isRequired;
        }

        /// <inheritdoc/>
        bool IReadOnlyForeignKey.IsRequiredDependent
        {
            [DebuggerStepThrough]
            get => _isRequiredDependent;
        }

        /// <inheritdoc/>
        DeleteBehavior IReadOnlyForeignKey.DeleteBehavior
        {
            [DebuggerStepThrough]
            get => _deleteBehavior;
        }

        /// <inheritdoc/>
        bool IReadOnlyForeignKey.IsOwnership
        {
            [DebuggerStepThrough]
            get => _isOwnership;
        }

        /// <inheritdoc />
        [DebuggerStepThrough]
        IEnumerable<IReadOnlySkipNavigation> IReadOnlyForeignKey.GetReferencingSkipNavigations()
            => ReferencingSkipNavigations ?? Enumerable.Empty<SlimSkipNavigation>();

        /// <inheritdoc />
        [DebuggerStepThrough]
        IDependentKeyValueFactory<TKey>? IForeignKey.GetDependentKeyValueFactory<TKey>()
            => (IDependentKeyValueFactory<TKey>?)((IRuntimeForeignKey)this).DependentKeyValueFactory;

        // Note: This is set and used only by IdentityMapFactoryFactory, which ensures thread-safety
        /// <inheritdoc/>
        object IRuntimeForeignKey.DependentKeyValueFactory
        {
            [DebuggerStepThrough]
            get => _dependentKeyValueFactory!;

            [DebuggerStepThrough]
            set => _dependentKeyValueFactory = value;
        }

        // Note: This is set and used only by IdentityMapFactoryFactory, which ensures thread-safety
        /// <inheritdoc/>
        Func<IDependentsMap> IRuntimeForeignKey.DependentsMapFactory
        {
            [DebuggerStepThrough]
            get => _dependentsMapFactory!;

            [DebuggerStepThrough]
            set => _dependentsMapFactory = value;
        }
    }
}
