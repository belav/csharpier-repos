﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Diagnostics
{
    /// <summary>
    ///     A <see cref="DiagnosticSource" /> event payload class for events that have an <see cref="INavigationBase" />.
    /// </summary>
    public class NavigationBaseEventData : EventData, INavigationBaseEventData
    {
        /// <summary>
        ///     Constructs the event payload.
        /// </summary>
        /// <param name="eventDefinition"> The event definition. </param>
        /// <param name="messageGenerator"> A delegate that generates a log message for this event. </param>
        /// <param name="navigationBase"> The navigation base. </param>
        public NavigationBaseEventData(
            EventDefinitionBase eventDefinition,
            Func<EventDefinitionBase, EventData, string> messageGenerator,
            IReadOnlyNavigationBase navigationBase)
            : base(eventDefinition, messageGenerator)
        {
            NavigationBase = navigationBase;
        }

        /// <summary>
        ///     The navigation base.
        /// </summary>
        public virtual IReadOnlyNavigationBase NavigationBase { get; }

        INavigationBase INavigationBaseEventData.NavigationBase
            => (INavigationBase)NavigationBase;
    }
}
