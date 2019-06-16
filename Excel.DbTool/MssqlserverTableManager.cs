using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Excel.DbTool
{
    class MssqlserverTableManager:ITableManager
    {
        string cnnstr = string.Empty;

        DbCategory ITableManager.DbCategory
        {
            get
            {
                return DbCategory.mssql;
            }
        }

        void ITableManager.SetConnectionString(string cnnstr)
        {
            if (string.IsNullOrEmpty(cnnstr))
                throw new ArgumentNullException("cnnstr");
            this.cnnstr = cnnstr;
        }

        List<string> ITableManager.GetAllName()
        {
            if (string.IsNullOrEmpty(this.cnnstr))
                throw new ArgumentException("必须指定数据库连接字符串");
            List<string> lst = new List<string>();
            using (System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(cnnstr))
            {
                using (System.Data.SqlClient.SqlCommand cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    cmd.CommandText = @"select TABLE_NAME
                                                            from INFORMATION_SCHEMA.TABLES
                                                            where TABLE_TYPE='BASE TABLE'";
                    using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lst.Add(reader["TABLE_NAME"].ToString());
                        }
                    }
                }
            }
            return lst.OrderBy(x => x).ToList();
        }

        TableMeta ITableManager.GetMeta(string tableName)
        {
            System.Data.DataTable Colummeta;
            using (System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(cnnstr))
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    cmd.CommandText = string.Format("select * from [{0}]", tableName);
                    using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.SchemaOnly))
                    {
                        Colummeta = reader.GetSchemaTable();
                    }
                }
            }
            string cmdstr = string.Format(@"select cl.name as ColumnName,ep.value as Description
                                            from sys.columns cl
                                            inner join sys.extended_properties ep on cl.column_id=ep.minor_id
                                            where cl.object_id=OBJECT_ID('{0}') and ep.major_id=OBJECT_ID('{0}')
                                            union all
                                            select '{0}',value 
                                            from sys.extended_properties 
                                            where sys.extended_properties.major_id=OBJECT_ID('{0}') and minor_id=0", tableName);
            System.Data.DataTable ColumetaDescription = new DataTable();
            using (System.Data.SqlClient.SqlDataAdapter adpater = new System.Data.SqlClient.SqlDataAdapter(cmdstr, cnnstr))
            {
                adpater.Fill(ColumetaDescription);
            }
            var lstcolunmeta = Colummeta.Select().Select(x => new ColumnMeta() {
                AllowDBNull = (bool)x["AllowDBNull"],
                Name = x["ColumnName"].ToString(),
                Size = (int)x["ColumnSize"],
                DataType = x["DataTypeName"].ToString(),
            }).ToList();
            var dictDescription = ColumetaDescription.Select().ToDictionary(x => x["ColumnName"].ToString(), x => x["Description"] as string);
            lstcolunmeta.ForEach(x => {
                x.Description = dictDescription.ContainsKey(x.Name) ? dictDescription[x.Name] : string.Empty;
                if (x.DataType.Contains("char") && x.Size >= int.MaxValue)
                    x.DataType = "text";
                else if (x.DataType.Contains("char") && x.Size < int.MaxValue)
                    x.DataType = string.Format("{0}({1})", x.DataType, x.Size);
            });
            return new TableMeta() {
                Columns = lstcolunmeta,
                Description = dictDescription.ContainsKey(tableName) ? dictDescription[tableName] : string.Empty,
                Name = tableName
            };
        }

        /// <summary>
        /// 新增表
        /// </summary>
        /// <param name="tablemeta"></param>
        /// <returns></returns>
        bool ITableManager.Create(TableMeta tablemeta)
        {
            List<string> lstsql = new List<string>();
            foreach (var col in tablemeta.Columns)
            {
                string masterKey = string.Empty;
                if (col.Name.ToLower().Equals("id"))
                    masterKey = "PRIMARY KEY CLUSTERED" + (col.DataType.Contains("int") ? " IDENTITY(1,1)" : string.Empty);
                else if (!string.IsNullOrEmpty(col.Description) && col.Description.StartsWith("主键"))
                    masterKey = "PRIMARY KEY CLUSTERED";
                string allowDbNull = col.AllowDBNull ? "NULL" : "NOT NULL";
                lstsql.Add($"[{col.Name}] {col.DataType} {masterKey} {allowDbNull}");
            }
            using (var cnn = new System.Data.SqlClient.SqlConnection(cnnstr))
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    cmd.CommandText = string.Format("CREATE TABLE [{0}]({1})", tablemeta.Name, string.Join(",", lstsql));
                    cmd.ExecuteNonQuery();
                }
            }
            using (var cnn = new System.Data.SqlClient.SqlConnection(cnnstr))
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "sys.sp_addextendedproperty";
                    cmd.Parameters.AddWithValue("name", "MS_Description");
                    cmd.Parameters.AddWithValue("value", tablemeta.Description ?? string.Empty);
                    cmd.Parameters.AddWithValue("level0type", "SCHEMA");
                    cmd.Parameters.AddWithValue("level0name", "dbo");
                    cmd.Parameters.AddWithValue("level1type", "TABLE");
                    cmd.Parameters.AddWithValue("level1name", tablemeta.Name);
                    cmd.Parameters.AddWithValue("level2type", DBNull.Value);
                    cmd.Parameters.AddWithValue("level2name", DBNull.Value);
                    cmd.ExecuteNonQuery();//表的说明
                    foreach (var col in tablemeta.Columns)
                    {
                        cmd.Parameters["value"].Value = col.Description ?? string.Empty;
                        cmd.Parameters["level1name"].Value = tablemeta.Name;
                        cmd.Parameters["level2name"].Value = col.Name;
                        cmd.Parameters["level2type"].Value = "COLUMN";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 重命名 表格、列
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="dictColName"></param>
        /// <returns></returns>
        bool ITableManager.AlterByColumnRename(TableMeta meta, Dictionary<string, string> dictColName)
        {
            using (var cnn = new System.Data.SqlClient.SqlConnection(cnnstr))
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "sys.sp_rename";
                    foreach (var kv in dictColName)
                    {
                        if (string.IsNullOrEmpty(kv.Value) || kv.Key.Equals(kv.Value, StringComparison.CurrentCultureIgnoreCase))
                            continue;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("objname", meta.Name + "." + kv.Key);
                        cmd.Parameters.AddWithValue("newname", kv.Value);
                        cmd.Parameters.AddWithValue("objtype", "column");
                        cmd.ExecuteNonQuery();
                    }
                    if (!string.IsNullOrEmpty(meta.NewName))
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("objname", meta.Name);
                        cmd.Parameters.AddWithValue("newname", meta.NewName);
                        cmd.Parameters.AddWithValue("objtype", DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 修改表设计，新增，修改，删除 列
        /// </summary>
        /// <param name="metaNew"></param>
        /// <returns></returns>
        bool ITableManager.Alter(TableMeta metaNew)
        {
            TableMeta metaOld = ((ITableManager)this).GetMeta(metaNew.Name);
            //新增的列
            var lstColAdd = metaNew.Columns.Except(metaOld.Columns, new ColumnMetaComparer()).ToList();
            var lstsqlAdd = lstColAdd.Select(x => string.Format("ALTER TABLE {0} ADD {1} {2} {3} {4}", metaNew.Name,
                x.Name,
                x.DataType,
                "id".Equals(x.Name, StringComparison.CurrentCultureIgnoreCase) ? "PRIMARY KEY CLUSTERED" + (x.DataType.Contains("int") ? " IDENTITY(1,1) " : string.Empty) : string.Empty,
                x.AllowDBNull ? "NULL" : "NOT NULL")).ToList();
            //移除的列
            var lstColDel = metaOld.Columns.Except(metaNew.Columns, new ColumnMetaComparer()).ToList();
            var lstsqlDel = lstColDel.Select(x => string.Format("ALTER TABLE {0} DROP COLUMN {1}", metaNew.Name, x.Name)).ToList();
            //修改的列
            var lstColEdit = metaNew.Columns.Join(metaOld.Columns, x => x.Name, x => x.Name, (x, y) => x).ToList();
            var lstsqlEdit = lstColEdit.Select(x => string.Format("ALTER TABLE {0} ALTER COLUMN {1} {2} {3} {4}", metaNew.Name,
                x.Name,
                x.DataType,
                 //x.DataTypeName.Contains("char") ? "(" + x.ColumnSize + ")" : string.Empty,
                 //"id".Equals(x.ColumnName, StringComparison.CurrentCultureIgnoreCase) ? "PRIMARY KEY CLUSTERED" + (x.DataTypeName.Contains("int") ? " IDENTITY(1,1) " : string.Empty) : string.Empty,
                 string.Empty,
                x.AllowDBNull ? "NULL" : "NOT NULL")).ToList();
            string spAdd = "sys.sp_addextendedproperty";
            string spEdit = "sys.sp_updateextendedproperty";
            string spDel = "sys.sp_dropextendedproperty";
            using (System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(this.cnnstr))
            {
                cnn.Open();
                using (var cmd = cnn.CreateCommand())
                {
                    foreach (var sqlstr in lstsqlAdd)
                    {//新增列
                        cmd.CommandText = sqlstr;
                        cmd.ExecuteNonQuery();
                    }
                    foreach (var sqlstr in lstsqlEdit)
                    {//修改列
                        cmd.CommandText = sqlstr;
                        cmd.ExecuteNonQuery();
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spEdit;
                    cmd.Parameters.AddWithValue("name", "MS_Description");
                    cmd.Parameters.AddWithValue("value", metaNew.Description ?? string.Empty);
                    cmd.Parameters.AddWithValue("level0type", "SCHEMA");
                    cmd.Parameters.AddWithValue("level0name", "dbo");
                    cmd.Parameters.AddWithValue("level1type", "TABLE");
                    cmd.Parameters.AddWithValue("level1name", metaNew.Name);
                    cmd.Parameters.AddWithValue("level2type", DBNull.Value);
                    cmd.Parameters.AddWithValue("level2name", DBNull.Value);
                    if (!DescriptionHandlerWrapper(cmd, spEdit))//可能表是别人之前建的，所以没有值
                        DescriptionHandlerWrapper(cmd, spAdd);//可能表是别人之前建的，所以没有值
                    cmd.Parameters["level2type"].Value = "COLUMN";
                    foreach (var col in lstColEdit)
                    {//修改的列的描述
                        cmd.Parameters["level2name"].Value = col.Name;
                        cmd.Parameters["value"].Value = col.Description ?? string.Empty;
                        if (!DescriptionHandlerWrapper(cmd, spEdit))//可能原来没值
                            DescriptionHandlerWrapper(cmd, spAdd);//所以直接新增
                    }
                    cmd.CommandText = spAdd;
                    foreach (var col in lstColAdd)
                    {//新增的列的描述
                        cmd.Parameters["level2name"].Value = col.Name;
                        cmd.Parameters["value"].Value = col.Description ?? string.Empty;
                        cmd.ExecuteNonQuery();
                    }
                    cmd.CommandText = spDel;
                    cmd.Parameters.Remove(cmd.Parameters["value"]);
                    foreach (var col in lstColDel)
                    {//删除列的描述  ，要在列删除之前，
                        cmd.Parameters["level2name"].Value = col.Name;
                        //cmd.Parameters["value"].Value = DBNull.Value;
                        DescriptionHandlerWrapper(cmd, spDel);//可能表是别人之前建的，没有注释，
                    }
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    foreach (var sqlstr in lstsqlDel)
                    {//先删除描述，再删除列
                        cmd.CommandText = sqlstr;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }

        List<TableMeta> ITableManager.GetAllMeta()
        {
            throw new NotImplementedException();
        }

        private bool DescriptionHandlerWrapper(SqlCommand cmd, string cmdstr)
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
