//---------------------------------------------------------------------
// <copyright file="EntityDataSourceContextDisposingEventArgs.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
// @owner       Microsoft
// @backupOwner objsdev
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace System.Web.UI.WebControls
{
    public class EntityDataSourceContextDisposingEventArgs : CancelEventArgs
    {
        private readonly ObjectContext _context = null;

        internal EntityDataSourceContextDisposingEventArgs(ObjectContext instance)
        {
            _context = instance;
        }

        public ObjectContext Context
        {
            get { return _context; }
        }
    }
}
