//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

namespace System.Activities.DurableInstancing
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime;
    using System.Runtime.DurableInstancing;

    [Fx.Tag.XamlVisible(false)]
    public sealed class HasActivatableWorkflowEvent
        : InstancePersistenceEvent<HasActivatableWorkflowEvent>
    {
        public HasActivatableWorkflowEvent()
            : base(InstancePersistence.ActivitiesEventNamespace.GetName("HasActivatableWorkflow"))
        { }
    }
}
