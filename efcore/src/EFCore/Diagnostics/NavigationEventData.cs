// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Diagnostics
{
    /// <summary>
    ///     A <see cref="DiagnosticSource" /> event payload class for events that have an <see cref="INavigation" />.
    /// </summary>
    public class NavigationEventData : EventData, INavigationBaseEventData
    {
        /// <summary>
        ///     Constructs the event payload.
        /// </summary>
        /// <param name="eventDefinition"> The event definition. </param>
        /// <param name="messageGenerator"> A delegate that generates a log message for this event. </param>
        /// <param name="navigation"> The navigation. </param>
        public NavigationEventData(
            EventDefinitionBase eventDefinition,
            Func<EventDefinitionBase, EventData, string> messageGenerator,
            IReadOnlyNavigation navigation)
            : base(eventDefinition, messageGenerator)
        {
            Navigation = navigation;
        }

        /// <summary>
        ///     The navigation.
        /// </summary>
        public virtual IReadOnlyNavigation Navigation { get; }

        /// <summary>
        ///     The navigation.
        /// </summary>
        INavigationBase INavigationBaseEventData.NavigationBase
            => (INavigationBase)Navigation;
    }
}
