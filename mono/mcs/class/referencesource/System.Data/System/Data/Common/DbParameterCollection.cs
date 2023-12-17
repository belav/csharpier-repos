//------------------------------------------------------------------------------
// <copyright file="DbParameterCollection.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
// <owner current="true" primary="false">Microsoft</owner>
//------------------------------------------------------------------------------

namespace System.Data.Common
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;

    public abstract class DbParameterCollection : MarshalByRefObject, IDataParameterCollection
    {
        protected DbParameterCollection()
            : base() { }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public abstract int Count { get; }

        [
            Browsable(false),
            EditorBrowsableAttribute(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual bool IsFixedSize
        {
            get { return false; }
        }

        [
            Browsable(false),
            EditorBrowsableAttribute(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        [
            Browsable(false),
            EditorBrowsableAttribute(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual bool IsSynchronized
        {
            get { return false; }
        }

        [
            Browsable(false),
            EditorBrowsableAttribute(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public abstract object SyncRoot { get; }

        object IList.this[int index]
        {
            get { return GetParameter(index); }
            set { SetParameter(index, (DbParameter)value); }
        }

        object IDataParameterCollection.this[string parameterName]
        {
            get { return GetParameter(parameterName); }
            set { SetParameter(parameterName, (DbParameter)value); }
        }

        public DbParameter this[int index]
        {
            get { return GetParameter(index); }
            set { SetParameter(index, value); }
        }

        public DbParameter this[string parameterName]
        {
            get { return GetParameter(parameterName) as DbParameter; }
            set { SetParameter(parameterName, value); }
        }

        public abstract int Add(object value);

        //

        abstract public void AddRange(System.Array values);

        //

        abstract public bool Contains(object value);

        public abstract bool Contains(string value); // WebData 97349

        //

        abstract public void CopyTo(System.Array array, int index);

        //

        abstract public void Clear();

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public abstract IEnumerator GetEnumerator();

        protected abstract DbParameter GetParameter(int index);

        protected abstract DbParameter GetParameter(string parameterName);

        public abstract int IndexOf(object value);

        //

        abstract public int IndexOf(string parameterName);

        public abstract void Insert(int index, object value);

        public abstract void Remove(object value);

        //

        //

        abstract public void RemoveAt(int index);

        public abstract void RemoveAt(string parameterName);

        protected abstract void SetParameter(int index, DbParameter value);

        protected abstract void SetParameter(string parameterName, DbParameter value);
    }
}
