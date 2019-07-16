using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel.DbTool
{
    public abstract class DbManager:IDbManager
    {
        public abstract DbType GetDbType();

        protected string Cnnstr { get; set; }

        public abstract bool AlterTable(TableMeta tablemeta);

        public abstract bool AlterTableWithRename(TableMeta tablemeta, Dictionary<string, string> dictName);

        public abstract bool CreateTable(TableMeta tablemeta);

        public abstract List<string> GetAllTableName();

        public abstract List<TableMeta> GetTableMeta();

        public abstract TableMeta GetTableMetaByName(string tableName);

        public  void SetConnectionString(string cnnstr)
        {
            this.Cnnstr = cnnstr;
        }

        public bool TryExecuteNonQuery(System.Data.IDbCommand cmd, string cmdstr)
        {
            try
            {
                cmd.CommandText = cmdstr;
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
