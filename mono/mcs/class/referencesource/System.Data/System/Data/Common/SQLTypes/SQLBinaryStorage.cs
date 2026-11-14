//------------------------------------------------------------------------------
// <copyright file="SQLBinaryStorage.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
// <owner current="true" primary="false">Microsoft</owner>
// <owner current="false" primary="false">Microsoft</owner>
//------------------------------------------------------------------------------

namespace System.Data.Common
{
    using System;
    using System.Collections;
    using System.Data.SqlTypes;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    internal sealed class SqlBinaryStorage : DataStorage
    {
        private SqlBinary[] values;

        public SqlBinaryStorage(DataColumn column)
            : base(column, typeof(SqlBinary), SqlBinary.Null, SqlBinary.Null, StorageType.SqlBinary)
        { }

        public override Object Aggregate(int[] records, AggregateType kind)
        {
            try
            {
                switch (kind)
                {
                    case AggregateType.First:
                        if (records.Length > 0)
                        {
                            return values[records[0]];
                        }
                        return null;

                    case AggregateType.Count:
                        int count = 0;
                        for (int i = 0; i < records.Length; i++)
                        {
                            if (!IsNull(records[i]))
                                count++;
                        }
                        return count;
                }
            }
            catch (OverflowException)
            {
                throw ExprException.Overflow(typeof(SqlBinary));
            }
            throw ExceptionBuilder.AggregateException(kind, DataType);
        }

        public override int Compare(int recordNo1, int recordNo2)
        {
            return values[recordNo1].CompareTo(values[recordNo2]);
        }

        public override int CompareValueTo(int recordNo, Object value)
        {
            return values[recordNo].CompareTo((SqlBinary)value);
        }

        public override object ConvertValue(object value)
        {
            if (null != value)
            {
                return SqlConvert.ConvertToSqlBinary(value);
            }
            return NullValue;
        }

        public override void Copy(int recordNo1, int recordNo2)
        {
            values[recordNo2] = values[recordNo1];
        }

        public override Object Get(int record)
        {
            return values[record];
        }

        public override bool IsNull(int record)
        {
            return (values[record].IsNull);
        }

        public override void Set(int record, Object value)
        {
            values[record] = SqlConvert.ConvertToSqlBinary(value);
        }

        public override void SetCapacity(int capacity)
        {
            SqlBinary[] newValues = new SqlBinary[capacity];
            if (null != values)
            {
                Array.Copy(values, 0, newValues, 0, Math.Min(capacity, values.Length));
            }
            values = newValues;
        }

        public override object ConvertXmlToObject(string s)
        {
            SqlBinary newValue = new SqlBinary();
            string tempStr = string.Concat("<col>", s, "</col>"); // this is done since you can give fragmet to reader, bug 98767
            StringReader strReader = new StringReader(tempStr);

            IXmlSerializable tmp = newValue;

            using (XmlTextReader xmlTextReader = new XmlTextReader(strReader))
            {
                tmp.ReadXml(xmlTextReader);
            }
            return ((SqlBinary)tmp);
        }

        public override string ConvertObjectToXml(object value)
        {
            Debug.Assert(!DataStorage.IsObjectNull(value), "we should have null here");
            Debug.Assert((value.GetType() == typeof(SqlBinary)), "wrong input type");

            StringWriter strwriter = new StringWriter(FormatProvider);

            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(strwriter))
            {
                ((IXmlSerializable)value).WriteXml(xmlTextWriter);
            }
            return (strwriter.ToString());
        }

        protected override object GetEmptyStorage(int recordCount)
        {
            return new SqlBinary[recordCount];
        }

        protected override void CopyValue(
            int record,
            object store,
            BitArray nullbits,
            int storeIndex
        )
        {
            SqlBinary[] typedStore = (SqlBinary[])store;
            typedStore[storeIndex] = values[record];
            nullbits.Set(storeIndex, IsNull(record));
        }

        protected override void SetStorage(object store, BitArray nullbits)
        {
            values = (SqlBinary[])store;
            //SetNullStorage(nullbits);
        }
    }
}
