//---------------------------------------------------------------------
// <copyright file="DbParameterCollectionHelper.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
// @owner  Microsoft
// @backupOwner Microsoft
//---------------------------------------------------------------------

namespace System.Data.EntityClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;

    public sealed partial class EntityParameterCollection : DbParameterCollection
    {
        private List<EntityParameter> _items;

        public override int Count
        {
            get
            {

                return ((null != _items) ? _items.Count : 0);
            }
        }

        private List<EntityParameter> InnerList
        {
            get
            {
                List<EntityParameter> items = _items;

                if (null == items)
                {
                    items = new List<EntityParameter>();
                    _items = items;
                }
                return items;
            }
        }

        public override bool IsFixedSize
        {
            get { return ((System.Collections.IList)InnerList).IsFixedSize; }
        }

        public override bool IsReadOnly
        {
            get { return ((System.Collections.IList)InnerList).IsReadOnly; }
        }

        public override bool IsSynchronized
        {
            get { return ((System.Collections.ICollection)InnerList).IsSynchronized; }
        }

        public override object SyncRoot
        {
            get { return ((System.Collections.ICollection)InnerList).SyncRoot; }
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public override int Add(object value)
        {
            OnChange();
            ValidateType(value);
            Validate(-1, value);
            InnerList.Add((EntityParameter)value);
            return Count - 1;
        }

        public override void AddRange(System.Array values)
        {
            OnChange();
            if (null == values)
            {
                throw EntityUtil.ArgumentNull("values");
            }
            foreach (object value in values)
            {
                ValidateType(value);
            }
            foreach (EntityParameter value in values)
            {
                Validate(-1, value);
                InnerList.Add((EntityParameter)value);
            }
        }

        private int CheckName(string parameterName)
        {
            int index = IndexOf(parameterName);
            if (index < 0)
            {
                throw EntityUtil.EntityParameterCollectionInvalidParameterName(parameterName);
            }
            return index;
        }

        public override void Clear()
        {
            OnChange();
            List<EntityParameter> items = InnerList;

            if (null != items)
            {
                foreach (EntityParameter item in items)
                {
                    item.ResetParent();
                }
                items.Clear();
            }
        }

        public override bool Contains(object value)
        {
            return (-1 != IndexOf(value));
        }

        public override void CopyTo(Array array, int index)
        {
            ((System.Collections.ICollection)InnerList).CopyTo(array, index);
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return ((System.Collections.ICollection)InnerList).GetEnumerator();
        }

        protected override DbParameter GetParameter(int index)
        {
            RangeCheck(index);
            return InnerList[index];
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            int index = IndexOf(parameterName);
            if (index < 0)
            {
                throw EntityUtil.EntityParameterCollectionInvalidParameterName(parameterName);
            }
            return InnerList[index];
        }

        private static int IndexOf(System.Collections.IEnumerable items, string parameterName)
        {
            if (null != items)
            {
                int i = 0;

                foreach (EntityParameter parameter in items)
                {
                    if (0 == EntityUtil.SrcCompare(parameterName, parameter.ParameterName))
                    {
                        return i;
                    }
                    ++i;
                }
                i = 0;

                foreach (EntityParameter parameter in items)
                {
                    if (0 == EntityUtil.DstCompare(parameterName, parameter.ParameterName))
                    {
                        return i;
                    }
                    ++i;
                }
            }
            return -1;
        }

        public override int IndexOf(string parameterName)
        {
            return IndexOf(InnerList, parameterName);
        }

        public override int IndexOf(object value)
        {
            if (null != value)
            {
                ValidateType(value);

                List<EntityParameter> items = InnerList;

                if (null != items)
                {
                    int count = items.Count;

                    for (int i = 0; i < count; i++)
                    {
                        if (value == items[i])
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        public override void Insert(int index, object value)
        {
            OnChange();
            ValidateType(value);
            Validate(-1, (EntityParameter)value);
            InnerList.Insert(index, (EntityParameter)value);
        }

        private void RangeCheck(int index)
        {
            if ((index < 0) || (Count <= index))
            {
                throw EntityUtil.EntityParameterCollectionInvalidIndex(index, Count);
            }
        }

        public override void Remove(object value)
        {
            OnChange();
            ValidateType(value);
            int index = IndexOf(value);
            if (-1 != index)
            {
                RemoveIndex(index);
            }
            else if (this != ((EntityParameter)value).CompareExchangeParent(null, this))
            {
                throw EntityUtil.EntityParameterCollectionRemoveInvalidObject();
            }
        }

        public override void RemoveAt(int index)
        {
            OnChange();
            RangeCheck(index);
            RemoveIndex(index);
        }

        public override void RemoveAt(string parameterName)
        {
            OnChange();
            int index = CheckName(parameterName);
            RemoveIndex(index);
        }

        private void RemoveIndex(int index)
        {
            List<EntityParameter> items = InnerList;
            Debug.Assert(
                (null != items) && (0 <= index) && (index < Count),
                "RemoveIndex, invalid"
            );
            EntityParameter item = items[index];
            items.RemoveAt(index);
            item.ResetParent();
        }

        private void Replace(int index, object newValue)
        {
            List<EntityParameter> items = InnerList;
            Debug.Assert(
                (null != items) && (0 <= index) && (index < Count),
                "Replace Index invalid"
            );
            ValidateType(newValue);
            Validate(index, newValue);
            EntityParameter item = items[index];
            items[index] = (EntityParameter)newValue;
            item.ResetParent();
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            OnChange();
            RangeCheck(index);
            Replace(index, value);
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            OnChange();
            int index = IndexOf(parameterName);
            if (index < 0)
            {
                throw EntityUtil.EntityParameterCollectionInvalidParameterName(parameterName);
            }
            Replace(index, value);
        }

        private void Validate(int index, object value)
        {
            if (null == value)
            {
                throw EntityUtil.EntityParameterNull("value");
            }

            object parent = ((EntityParameter)value).CompareExchangeParent(this, null);
            if (null != parent)
            {
                if (this != parent)
                {
                    throw EntityUtil.EntityParameterContainedByAnotherCollection();
                }
                if (index != IndexOf(value))
                {
                    throw EntityUtil.EntityParameterContainedByAnotherCollection();
                }
            }

            String name = ((EntityParameter)value).ParameterName;
            if (0 == name.Length)
            {
                index = 1;
                do
                {
                    name = EntityUtil.Parameter + index.ToString(CultureInfo.CurrentCulture);
                    index++;
                } while (-1 != IndexOf(name));
                ((EntityParameter)value).ParameterName = name;
            }
        }

        private void ValidateType(object value)
        {
            if (null == value)
            {
                throw EntityUtil.EntityParameterNull("value");
            }
            else if (!ItemType.IsInstanceOfType(value))
            {
                throw EntityUtil.InvalidEntityParameterType(value);
            }
        }
    };
}
