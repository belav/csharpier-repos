// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Razor.Resources;
using System.Web.Razor.Text;

namespace System.Web.Razor.Editor
{
    internal static class RazorEditorTrace
    {
        private static bool? _enabled;

        private static bool IsEnabled()
        {
            if (_enabled == null)
            {
                bool enabled;
                if (Boolean.TryParse(Environment.GetEnvironmentVariable("RAZOR_EDITOR_TRACE"), out enabled))
                {
                    Trace.WriteLine(String.Format(
                        CultureInfo.CurrentCulture,
                        RazorResources.Trace_Startup,
                        enabled ? RazorResources.Trace_Enabled : RazorResources.Trace_Disabled));
                    _enabled = enabled;
                }
                else
                {
                    _enabled = false;
                }
            }
            return _enabled.Value;
        }

        [Conditional("EDITOR_TRACING")]
        public static void TraceLine(string format, params object[] args)
        {
            if (IsEnabled())
            {
                Trace.WriteLine(String.Format(
                    CultureInfo.CurrentCulture,
                    RazorResources.Trace_Format,
                    String.Format(CultureInfo.CurrentCulture, format, args)));
            }
        }
    }
}
