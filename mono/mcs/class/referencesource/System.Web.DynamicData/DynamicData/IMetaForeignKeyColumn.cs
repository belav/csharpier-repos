namespace System.Web.DynamicData
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal interface IMetaForeignKeyColumn : IMetaColumn
    {
        void ExtractForeignKey(IDictionary dictionary, string value);
        ReadOnlyCollection<string> ForeignKeyNames { get; }
        string GetForeignKeyDetailsPath(object row);
        string GetForeignKeyPath(string action, object row);
        string GetForeignKeyPath(string action, object row, string path);
        string GetForeignKeyString(object row);
        IList<object> GetForeignKeyValues(object row);
        bool IsPrimaryKeyInThisTable { get; }
        IMetaTable ParentTable { get; }
    }
}
