// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Components.Forms
{
    /// <summary>
    /// Adds Data Annotations validation support to an <see cref="EditContext"/>.
    /// </summary>
    public class DataAnnotationsValidator : ComponentBase, IDisposable
    {
        private IDisposable? _subscriptions;
        private EditContext? _originalEditContext;

        [CascadingParameter] EditContext? CurrentEditContext { get; set; }

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{nameof(DataAnnotationsValidator)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. For example, you can use {nameof(DataAnnotationsValidator)} " +
                    $"inside an EditForm.");
            }

            _subscriptions = CurrentEditContext.EnableDataAnnotationsValidation();
            _originalEditContext = CurrentEditContext;
        }

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            if (CurrentEditContext != _originalEditContext)
            {
                // While we could support this, there's no known use case presently. Since InputBase doesn't support it,
                // it's more understandable to have the same restriction.
                throw new InvalidOperationException($"{GetType()} does not support changing the " +
                    $"{nameof(EditContext)} dynamically.");
            }
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
        }

        void IDisposable.Dispose()
        {
            _subscriptions?.Dispose();
            _subscriptions = null;

            Dispose(disposing: true);
        }
    }
}
