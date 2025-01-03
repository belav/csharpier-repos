//---------------------------------------------------------------------
// <copyright file="EntityDataSourceContextCreatingEventArgs.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
// @owner       Microsoft
// @backupOwner objsdev
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace System.Web.UI.WebControls
{
    public class EntityDataSourceContextCreatingEventArgs : EventArgs
    {
        private ObjectContext _context;

        internal EntityDataSourceContextCreatingEventArgs() { }

        public ObjectContext Context
        {
            get { return _context; }
            set { _context = value; }
        }
    }
}
