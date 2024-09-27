//------------------------------------------------------------------------------
// <copyright file="SQLStringStorage.cs" company="Microsoft">
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

    internal sealed class SqlStringStorage : DataStorage
    {
        private SqlString[] values;

        public SqlStringStorage(DataColumn column)
            : base(column, typeof(SqlString), SqlString.Null, SqlString.Null, StorageType.SqlString)
        { }

        public override Object Aggregate(int[] recordNos, AggregateType kind)
        {
            try
            {
                int i;
                switch (kind)
                {
                    case AggregateType.Min:
                        int min = -1;
                        for (i = 0; i < recordNos.Length; i++)
                        {
                            if (IsNull(recordNos[i]))
                                continue;
                            min = recordNos[i];
                            break;
                        }
                        if (min >= 0)
                        {
                            for (i = i + 1; i < recordNos.Length; i++)
                            {
                                if (IsNull(recordNos[i]))
                                    continue;
                                if (Compare(min, recordNos[i]) > 0)
                                {
                                    min = recordNos[i];
                                }
                            }
                            return Get(min);
                        }
                        return NullValue;

                    case AggregateType.Max:
                        int max = -1;
                        for (i = 0; i < recordNos.Length; i++)
                        {
                            if (IsNull(recordNos[i]))
                                continue;
                            max = recordNos[i];
                            break;
                        }
                        if (max >= 0)
                        {
                            for (i = i + 1; i < recordNos.Length; i++)
                            {
                                if (Compare(max, recordNos[i]) < 0)
                                {
                                    max = recordNos[i];
                                }
                            }
                            return Get(max);
                        }
                        return NullValue;

                    case AggregateType.Count:
                        int count = 0;
                        for (i = 0; i < recordNos.Length; i++)
                        {
                            if (!IsNull(recordNos[i]))
                                count++;
                        }
                        return count;
                }
            }
            catch (OverflowException)
            {
                throw ExprException.Overflow(typeof(SqlString));
            }
            throw ExceptionBuilder.AggregateException(kind, DataType);
        }

        public override int Compare(int recordNo1, int recordNo2)
        {
            return Compare(values[recordNo1], values[recordNo2]);
        }

        public int Compare(SqlString valueNo1, SqlString valueNo2)
        {
            if (valueNo1.IsNull && valueNo2.IsNull)
                return 0;

            if (valueNo1.IsNull)
                return -1;

            if (valueNo2.IsNull)
                return 1;

            return Table.Compare(valueNo1.Value, valueNo2.Value);
        }

        public override int CompareValueTo(int recordNo, Object value)
        {
            return Compare(values[recordNo], (SqlString)value);
        }

        public override object ConvertValue(object value)
        {
            if (null != value)
            {
                return SqlConvert.ConvertToSqlString(value);
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

        public override int GetStringLength(int record)
        {
            SqlString value = values[record];
            return ((value.IsNull) ? 0 : value.Value.Length);
        }

        public override bool IsNull(int record)
        {
            return (values[record].IsNull);
        }

        public override void Set(int record, Object value)
        {
            values[record] = SqlConvert.ConvertToSqlString(value);
        }

        public override void SetCapacity(int capacity)
        {
            SqlString[] newValues = new SqlString[capacity];
            if (null != values)
            {
                Array.Copy(values, 0, newValues, 0, Math.Min(capacity, values.Length));
            }
            values = newValues;
        }

        public override object ConvertXmlToObject(string s)
        {
            SqlString newValue = new SqlString();
            string tempStr = string.Concat("<col>", s, "</col>"); // this is done since you can give fragmet to reader, bug 98767
            StringReader strReader = new StringReader(tempStr);

            IXmlSerializable tmp = newValue;

            using (XmlTextReader xmlTextReader = new XmlTextReader(strReader))
            {
                tmp.ReadXml(xmlTextReader);
            }
            return ((SqlString)tmp);
        }

        public override string ConvertObjectToXml(object value)
        {
            Debug.Assert(!DataStorage.IsObjectNull(value), "we shouldn't have null here");
            Debug.Assert((value.GetType() == typeof(SqlString)), "wrong input type");

            StringWriter strwriter = new StringWriter(FormatProvider); // consider passing cultureinfo with CultureInfo.InvariantCulture

            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(strwriter))
            {
                ((IXmlSerializable)value).WriteXml(xmlTextWriter);
            }
            return (strwriter.ToString());
        }

        protected override object GetEmptyStorage(int recordCount)
        {
            return new SqlString[recordCount];
        }

        protected override void CopyValue(
            int record,
            object store,
            BitArray nullbits,
            int storeIndex
        )
        {
            SqlString[] typedStore = (SqlString[])store;
            typedStore[storeIndex] = values[record];
            nullbits.Set(storeIndex, IsNull(record));
        }

        protected override void SetStorage(object store, BitArray nullbits)
        {
            values = (SqlString[])store;
            //SetNullStorage(nullbits);
        }
    }
}
