//---------------------------------------------------------------------
// <copyright file="EntityDataSourceContextCreatedEventArgs.cs" company="Microsoft">
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
    public class EntityDataSourceContextCreatedEventArgs : EventArgs
    {
        private readonly ObjectContext _context;

        internal EntityDataSourceContextCreatedEventArgs(ObjectContext context)
        {
            _context = context;
        }

        public ObjectContext Context
        {
            get { return _context; }
        }
    }
}
