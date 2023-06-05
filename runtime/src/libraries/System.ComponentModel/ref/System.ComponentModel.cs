// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// ------------------------------------------------------------------------------
// Changes to this file must follow the https://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System
{
    partial public interface IServiceProvider
    {
        object? GetService(System.Type serviceType);
    }
}

namespace System.ComponentModel
{
    partial public class CancelEventArgs : System.EventArgs
    {
        public CancelEventArgs() { }

        public CancelEventArgs(bool cancel) { }

        public bool Cancel
        {
            get { throw null; }
            set { }
        }
    }

    partial public interface IChangeTracking
    {
        bool IsChanged { get; }
        void AcceptChanges();
    }

    partial public interface IEditableObject
    {
        void BeginEdit();
        void CancelEdit();
        void EndEdit();
    }

    partial public interface IRevertibleChangeTracking : System.ComponentModel.IChangeTracking
    {
        void RejectChanges();
    }
}
