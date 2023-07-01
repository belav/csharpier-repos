//------------------------------------------------------------------------------
// <copyright file="SqlXmlStorage.cs" company="Microsoft">
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
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    internal sealed class SqlXmlStorage : DataStorage
    {
        private SqlXml[] values;

        public SqlXmlStorage(DataColumn column)
            : base(column, typeof(SqlXml), SqlXml.Null, SqlXml.Null, StorageType.Empty) { }

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
                        return null; // no data => null

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
                throw ExprException.Overflow(typeof(SqlXml));
            }
            throw ExceptionBuilder.AggregateException(kind, _dataType);
        }

        public override int Compare(int recordNo1, int recordNo2)
        {
            //return values[recordNo1].CompareTo(values[recordNo2]);
            return 0;
        }

        public override int CompareValueTo(int recordNo, Object value)
        {
            // SqlXml valueNo2 = ((value == null)||(value == DBNull.Value))? SqlXml.Null : (SqlXml)value;
            // return values[recordNo].CompareTo(valueNo2);
            return 0;
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
            if ((value == DBNull.Value) || (value == null))
            {
                values[record] = SqlXml.Null;
            }
            else
            {
                values[record] = (SqlXml)value;
            }
        }

        public override void SetCapacity(int capacity)
        {
            SqlXml[] newValues = new SqlXml[capacity];
            if (null != values)
            {
                Array.Copy(values, 0, newValues, 0, Math.Min(capacity, values.Length));
            }
            values = newValues;
        }

        public override object ConvertXmlToObject(string s)
        {
            XmlTextReader reader = new XmlTextReader(s, XmlNodeType.Element, null);
            return (new SqlXml(reader));

            /*            SqlXml newValue = new SqlXml();
                        
                        StringReader strReader = new  StringReader(s);
                        XmlTextReader xmlTextReader = new XmlTextReader(strReader);
                        ((IXmlSerializable)newValue).ReadXml(xmlTextReader);
                        xmlTextReader.Close();
                        return newValue;
            */
        }

        public override string ConvertObjectToXml(object value)
        {
            SqlXml reader = (SqlXml)value;
            if (reader.IsNull)
                return ADP.StrEmpty;
            else
                return reader.Value; // SqlXml.Value returns string
        }

        protected override object GetEmptyStorage(int recordCount)
        {
            return new SqlXml[recordCount];
        }

        protected override void CopyValue(
            int record,
            object store,
            BitArray nullbits,
            int storeIndex
        )
        {
            SqlXml[] typedStore = (SqlXml[])store;
            typedStore[storeIndex] = values[record];
            nullbits.Set(storeIndex, IsNull(record));
        }

        protected override void SetStorage(object store, BitArray nullbits)
        {
            values = (SqlXml[])store;
            //SetNullStorage(nullbits);
        }
    }
}
