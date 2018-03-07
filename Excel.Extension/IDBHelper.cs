using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel.Extension
{
    public interface IDBHelper
    {
        DBProvider GetProvider();

        List<string> InitDrpTableListByMssql(string cnnstr);

        void GetTableSchemalByMssqlInternal(string cnnstr, string tableName, out System.Data.DataTable table, out System.Data.DataTable tableRemark);

        void AddTableMetaByMssqlWithDDL(string cnnstr, string tableName, object[,] value);

        void EditTableMetaByMssqlWithDDL(string cnnstr, string tableName, object[,] value);

        void EditTableMetaByMssqlWithReName(string cnnstr, string tableName, object[,] value);
    }
}
