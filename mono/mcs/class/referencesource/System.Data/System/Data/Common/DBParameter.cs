//------------------------------------------------------------------------------
// <copyright file="DbParameter.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
// <owner current="true" primary="false">Microsoft</owner>
//------------------------------------------------------------------------------

namespace System.Data.Common
{
    using System;
    using System.ComponentModel;
    using System.Data;

    public abstract class DbParameter : MarshalByRefObject, IDbDataParameter
    { // V1.2.3300
        protected DbParameter()
            : base() { }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            RefreshProperties(RefreshProperties.All),
            ResCategoryAttribute(Res.DataCategory_Data),
            ResDescriptionAttribute(Res.DbParameter_DbType),
        ]
        public abstract DbType DbType { get; set; }

        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        public abstract void ResetDbType();

        [
            DefaultValue(ParameterDirection.Input),
            RefreshProperties(RefreshProperties.All),
            ResCategoryAttribute(Res.DataCategory_Data),
            ResDescriptionAttribute(Res.DbParameter_Direction),
        ]
        public abstract ParameterDirection Direction { get; set; }

        [Browsable(false), DesignOnly(true), EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public abstract Boolean IsNullable { get; set; }

        [
            DefaultValue(""),
            ResCategoryAttribute(Res.DataCategory_Data),
            ResDescriptionAttribute(Res.DbParameter_ParameterName),
        ]
        public abstract String ParameterName { get; set; }

        byte IDbDataParameter.Precision
        { // SqlProjectTracking 17233
            get { return 0; }
            set { }
        }

        byte IDbDataParameter.Scale
        { // SqlProjectTracking 17233
            get { return 0; }
            set { }
        }

        public virtual byte Precision
        {
            get { return ((IDbDataParameter)this).Precision; }
            set { ((IDbDataParameter)this).Precision = value; }
        }

        public virtual byte Scale
        {
            get { return ((IDbDataParameter)this).Scale; }
            set { ((IDbDataParameter)this).Scale = value; }
        }

        [ResCategoryAttribute(Res.DataCategory_Data), ResDescriptionAttribute(Res.DbParameter_Size)]
        public abstract int Size { get; set; }

        [
            DefaultValue(""),
            ResCategoryAttribute(Res.DataCategory_Update),
            ResDescriptionAttribute(Res.DbParameter_SourceColumn),
        ]
        public abstract String SourceColumn { get; set; }

        [
            DefaultValue(false),
            EditorBrowsableAttribute(EditorBrowsableState.Advanced),
            RefreshProperties(RefreshProperties.All),
            ResCategoryAttribute(Res.DataCategory_Update),
            ResDescriptionAttribute(Res.DbParameter_SourceColumnNullMapping),
        ]
        public abstract bool SourceColumnNullMapping { get; set; }

        [
            DefaultValue(DataRowVersion.Current),
            ResCategoryAttribute(Res.DataCategory_Update),
            ResDescriptionAttribute(Res.DbParameter_SourceVersion),
        ]
        public virtual DataRowVersion SourceVersion
        {
            get { return DataRowVersion.Default; }
            set { }
        }

        [
            DefaultValue(null),
            RefreshProperties(RefreshProperties.All),
            ResCategoryAttribute(Res.DataCategory_Data),
            ResDescriptionAttribute(Res.DbParameter_Value),
        ]
        public abstract object Value { get; set; }
    }
}
