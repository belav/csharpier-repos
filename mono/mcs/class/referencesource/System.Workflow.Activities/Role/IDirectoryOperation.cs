#region Using directives

using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;

#endregion

namespace System.Workflow.Activities
{
    internal interface IDirectoryOperation
    {
        void GetResult(
            DirectoryEntry rootEntry,
            DirectoryEntry currentEntry,
            List<DirectoryEntry> response
        );
    }
}
