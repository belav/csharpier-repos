//------------------------------------------------------------------------------
// <copyright file="InternalConfigEventArgs.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Configuration.Internal
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Configuration.Internal;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;
    using System.Xml;

    //
    // Event arguments for Configuration events.
    //
    public sealed class InternalConfigEventArgs : EventArgs
    {
        string _configPath;

        public InternalConfigEventArgs(string configPath)
        {
            _configPath = configPath;
        }

        public string ConfigPath
        {
            get { return _configPath; }
            set { _configPath = value; }
        }
    }
}
