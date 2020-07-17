using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Excel.DbTool
{
    public class MySqlManager:DbManager,IDbManager
    {
        public override DbType GetDbType()
        {
            return DbType.mysql;
        }

        public override List<string> GetAllTableName()
        {
            if (string.IsNullOrEmpty(this.Cnnstr))
                throw new ArgumentException("必须指定数据库连接字符串");
            var dbName = Cnnstr.Split(new char[] { ';', '=' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower()).SkipWhile(x => x != "initial catalog").ToList()[1];
            string cmdstr = string.Format(@"select *
                                                            from INFORMATION_SCHEMA.TABLES
                                                            where TABLE_TYPE='BASE TABLE' and table_schema='{0}'", dbName);
            var lstTableMeta = SqlHelper<MySql.Data.MySqlClient.MySqlConnection>.ExecuteReader<TableMeta>(this.Cnnstr, cmdstr)
                  .Invoke((reader, entity) => {
                      entity.Name = reader["TABLE_NAME"].ToString();
                  });
            return lstTableMeta.Select(x => x.Name).OrderBy(x => x).ToList();
        }

        public override TableMeta GetTableMetaByName(string tableName)
        {
            var dbName = Cnnstr.Split(new char[] { ';', '=' }, StringSplitOptions.RemoveEmptyEntries).SkipWhile(x => x.ToLower().IndexOf("initial catalog") < 0).ToList()[1];
            string cmdstr = string.Format("select * from information_schema.TABLES where TABLE_SCHEMA='{0}' and table_name='{1}'", dbName, tableName);
            TableMeta tableMate= SqlHelper<MySql.Data.MySqlClient.MySqlConnection>.ExecuteReader<TableMeta>(this.Cnnstr, cmdstr)
                .Invoke((reader, entity) => entity.Remark = reader["TABLE_COMMENT"].ToString())
                .First();
            tableMate.Name = tableName;
            cmdstr = string.Format(@"show full fields from {0}", tableName);
            tableMate.Columns = SqlHelper<MySql.Data.MySqlClient.MySqlConnection>.ExecuteReader<ColumnMeta>(this.Cnnstr, cmdstr)
                .Invoke((reader, entity) =>{
                    entity.TableName = tableName;
                    entity.DataType = reader["Type"].ToString();
                    entity.Name = reader["Field"].ToString();
                    entity.Comment = new Comment()
                    {
                        Remark = reader["Comment"] as string
                    };
                    entity.UnNullable = !reader["NULL"].ToString().Equals("YES", StringComparison.CurrentCultureIgnoreCase);
                });
            tableMate.Columns.ForEach(x =>
            {
                if (x.DataType.Contains("char"))
                    return;
                x.DataType = x.DataType.Split('(')[0];
            });
            return tableMate;
        }

        /// <summary>
        /// 新增表
        /// </summary>
        /// <param name="tablemeta"></param>
        /// <returns></returns>
        public override bool CreateTable(TableMeta tablemeta)
        {
            List<string> lstsql = tablemeta.Columns.Select(col => string.Format("`{0}` {1} {2} {3} COMMENT '{4}'", col.Name, col.DataType,
                    col.Name.ToLower().Equals("id") ? "PRIMARY KEY" + (col.DataType.Contains("int") ? " AUTO_INCREMENT" : string.Empty) : string.Empty,
                    col.GetNullableString(),
                    col.Comment.Remark ?? string.Empty)).ToList();
            string cmdstr= string.Format("CREATE TABLE `{0}`({1})\r\nCOMMENT='{2}';", tablemeta.Name, string.Join(",", lstsql), tablemeta.Remark);
            SqlHelper<MySql.Data.MySqlClient.MySqlConnection>.ExecuteNonQuery(this.Cnnstr, cmdstr);
            return true;
        }

        /// <summary>
        /// 重命名 表格、列
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="dictColName"></param>
        /// <returns></returns>
        public override bool AlterTableWithRename(TableMeta meta, Dictionary<string, string> dictColName)
        {
            var metaOld = this.GetTableMetaByName(meta.Name);
            SqlHelper<MySql.Data.MySqlClient.MySqlConnection>.ExecuteNonQuery(this.Cnnstr, string.Empty, cmd =>
            {
                foreach (var kv in dictColName)
                {
                    if (string.IsNullOrEmpty(kv.Value) || kv.Key.Equals(kv.Value, StringComparison.CurrentCultureIgnoreCase))
                        continue;
                    //alter table <表名> change <字段名> <字段新名称> <字段的类型>
                    cmd.CommandText = string.Format("alter table {0} change {1} {2} {3}", metaOld.Name,
                        kv.Key,
                        kv.Value,
                        metaOld.Columns.FirstOrDefault(x => x.Name.Equals(kv.Key, StringComparison.CurrentCultureIgnoreCase)).DataType);
                    cmd.ExecuteNonQuery();
                }
                if (!string.IsNullOrEmpty(meta.NewName))
                {
                    cmd.CommandText = string.Format("RENAME TABLE {0} TO {1}", meta.Name, meta.NewName);
                    cmd.ExecuteNonQuery();
                }
                return false;
            });
            return true;
        }

        /// <summary>
        /// 修改表设计，新增，修改，删除 列
        /// </summary>
        /// <param name="metaNew"></param>
        /// <returns></returns>
        public override bool AlterTable(TableMeta metaNew)
        {
            TableMeta metaOld =this.GetTableMetaByName(metaNew.Name);
            //新增的列
            var lstColAdd = metaNew.Columns.Except(metaOld.Columns).ToList();
            var lstCmdAdd = lstColAdd.Select(x => string.Format("ALTER TABLE {0} ADD {1} {2} {3} COMMENT '{4}'", metaNew.Name,
                x.Name,
                x.DataType,
                x.GetNullableString(),
                x.Comment.Remark ?? string.Empty)).ToList();
            //移除的列
            var lstColDel = metaOld.Columns.Except(metaNew.Columns).ToList();
            var lstCmdDel = lstColDel.Select(x => string.Format("ALTER TABLE {0} DROP COLUMN {1}", metaNew.Name, x.Name)).ToList();
            //修改的列
            var lstColEdit = metaNew.Columns.Join(metaOld.Columns, x => x.Name, x => x.Name, (x, y) => x).ToList();
            var lstCmdEdit = lstColEdit.Select(x => string.Format("ALTER TABLE {0} modify COLUMN {1} {2} {3} COMMENT '{4}'", metaNew.Name,
                x.Name,
                x.DataType,
                x.GetNullableString(),
                x.Comment.Remark ?? string.Empty)).ToList();
            SqlHelper<MySql.Data.MySqlClient.MySqlConnection>.ExecuteNonQuery(this.Cnnstr, string.Empty, cmd =>
            {
                TryExecuteNonQuery(cmd, string.Format("alter table {0} comment '{1}'", metaNew.Name, metaNew.Remark ?? string.Empty));
                lstCmdAdd.ForEach(sqlstr =>
                {
                    cmd.CommandText = sqlstr;
                    cmd.ExecuteNonQuery();
                });
                lstCmdEdit.ForEach(sqlstr =>
                {
                    cmd.CommandText = sqlstr;
                    cmd.ExecuteNonQuery();
                });
                lstCmdDel.ForEach(sqlstr =>
                {
                    cmd.CommandText = sqlstr;
                    cmd.ExecuteNonQuery();
                });
                return false;
            });
            return true;
        }

        public override List<TableMeta> GetTableMeta()
        {
            throw new NotImplementedException();
        }
    }
}
