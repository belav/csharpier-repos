//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.Activities.Core.Presentation
{
    using System.Runtime;
    using System.Windows;

    [Fx.Tag.XamlVisible(false)]
    public class LocationChangedEventArgs : EventArgs
    {
        Point newLocation;

        public LocationChangedEventArgs(Point newLocation)
        {
            this.newLocation = newLocation;
        }

        public Point NewLocation
        {
            get { return this.newLocation; }
        }
    }
}
