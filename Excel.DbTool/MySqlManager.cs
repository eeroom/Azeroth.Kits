using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Excel.DbTool
{
    class MySqlManager:IDbManager
    {
        string cnnstr = string.Empty;

        DbIndex IDbManager.DbIndex
        {
            get
            {
                return DbIndex.mysql;
            }
        }

        void IDbManager.SetDbConnectionString(string cnnstr)
        {
            if (string.IsNullOrEmpty(cnnstr))
                throw new ArgumentNullException("cnnstr");
            this.cnnstr = cnnstr;
        }

        List<string> IDbManager.GetAllTableName()
        {
            if (string.IsNullOrEmpty(this.cnnstr))
                throw new ArgumentException("必须指定数据库连接字符串");
            List<string> lst = new List<string>();
            using (var cnn = new MySql.Data.MySqlClient.MySqlConnection(cnnstr))
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    var catalog = cnnstr.Split(new char[] { ';', '=' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower()).SkipWhile(x => x != "initial catalog").ToArray()[1];
                    cmd.CommandText = string.Format(@"select *
                                                            from INFORMATION_SCHEMA.TABLES
                                                            where TABLE_TYPE='BASE TABLE' and table_schema='{0}'", catalog);
                    using (var reader = cmd.ExecuteReader())
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

        TableMeta IDbManager.GetTableMeta(string tableName)
        {
            System.Data.DataTable Colummeta = new DataTable();
            var tmp = cnnstr.Split(new char[] { ';', '=' }, StringSplitOptions.RemoveEmptyEntries).SkipWhile(x => x.ToLower().IndexOf("initial catalog") < 0).ToList();

            string cmdstr = string.Format("select * from information_schema.TABLES where TABLE_SCHEMA='{0}' and table_name='{1}'", tmp[1], tableName);

            using (var adpater = new MySql.Data.MySqlClient.MySqlDataAdapter(cmdstr, cnnstr))
            {
                adpater.Fill(Colummeta);
            }

            cmdstr = string.Format(@"show full fields from {0}", tableName);
            System.Data.DataTable ColumetaDescription = new DataTable();
            using (var adpater = new MySql.Data.MySqlClient.MySqlDataAdapter(cmdstr, cnnstr))
            {
                adpater.Fill(ColumetaDescription);
            }
            //var lstcolunmeta = Colummeta.Select().Select(x => new ColumnMeta()
            //{
            //    AllowDBNull = (bool)x["AllowDBNull"],
            //    ColumnName = x["ColumnName"].ToString()
            //    //ColumnSize = (int)x["ColumnSize"]
            //}).ToList();
            var dictDescription = ColumetaDescription.Select().Select(x => new ColumnMeta() {
                DataTypeName = x["Type"].ToString(),
                ColumnName = x["Field"].ToString(),
                Description = x["Comment"] as string,
                AllowDBNull = x["NULL"].ToString().Equals("YES", StringComparison.CurrentCultureIgnoreCase) ? true : false
            }).ToList();

            dictDescription.ForEach(x => {
                if (x.DataTypeName.Contains("char"))
                    return;
                x.DataTypeName = x.DataTypeName.Split('(')[0];
            });
            //.ToDictionary(x => x["FieId"].ToString(), x => x["Comment"] as string);
            //lstcolunmeta.ForEach(x => x.Description = dictDescription.ContainsKey(x.ColumnName) ? dictDescription[x.ColumnName] : string.Empty);
            return new TableMeta() {
                Columns = dictDescription,
                Description = Colummeta.Rows[0]["TABLE_COMMENT"] as string,
                Name = tableName
            };
        }

        /// <summary>
        /// 新增表
        /// </summary>
        /// <param name="tablemeta"></param>
        /// <returns></returns>
        bool IDbManager.TableAdd(TableMeta tablemeta)
        {
            List<string> lstsql = tablemeta.Columns.Select(col => string.Format("`{0}` {1} {2} {3} COMMENT '{4}'", col.ColumnName, col.DataTypeName,
                    //col.DataTypeName.Contains("char") ? "(" + (col.ColumnSize >= int.MaxValue ? "max" : col.ColumnSize.ToString()) + ")" : string.Empty,
                    col.ColumnName.ToLower().Equals("id") ? "PRIMARY KEY" + (col.DataTypeName.Contains("int") ? " AUTO_INCREMENT" : string.Empty) : string.Empty,
                    col.AllowDBNull ? "NULL" : "NOT NULL",
                    col.Description ?? string.Empty)).ToList();
            using (var cnn = new MySql.Data.MySqlClient.MySqlConnection(cnnstr))
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    cmd.CommandText = string.Format("CREATE TABLE `{0}`({1})\r\nCOMMENT='{2}';", tablemeta.Name, string.Join(",", lstsql), tablemeta.Description);
                    cmd.ExecuteNonQuery();
                }
            }
            //using (var cnn = new System.Data.SqlClient.SqlConnection(cnnstr))
            //{
            //    using (var cmd = cnn.CreateCommand())
            //    {
            //        cnn.Open();
            //        cmd.CommandType = System.Data.CommandType.StoredProcedure;
            //        cmd.CommandText = "sys.sp_addextendedproperty";
            //        cmd.Parameters.AddWithValue("name", "MS_Description");
            //        cmd.Parameters.AddWithValue("value", tablemeta.Description ?? string.Empty);
            //        cmd.Parameters.AddWithValue("level0type", "SCHEMA");
            //        cmd.Parameters.AddWithValue("level0name", "dbo");
            //        cmd.Parameters.AddWithValue("level1type", "TABLE");
            //        cmd.Parameters.AddWithValue("level1name", tablemeta.Name);
            //        cmd.Parameters.AddWithValue("level2type", DBNull.Value);
            //        cmd.Parameters.AddWithValue("level2name", DBNull.Value);
            //        cmd.ExecuteNonQuery();//表的说明
            //        foreach (var col in tablemeta.Columns)
            //        {
            //            cmd.Parameters["value"].Value = col.Description ?? string.Empty;
            //            cmd.Parameters["level1name"].Value = tablemeta.Name;
            //            cmd.Parameters["level2name"].Value = col.ColumnName;
            //            cmd.Parameters["level2type"].Value = "COLUMN";
            //            cmd.ExecuteNonQuery();
            //        }
            //    }
            //}
            return true;
        }

        /// <summary>
        /// 重命名 表格、列
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="dictColName"></param>
        /// <returns></returns>
        bool IDbManager.TableDesignerReName(TableMeta meta, Dictionary<string, string> dictColName)
        {
            var metadata = ((IDbManager)this).GetTableMeta(meta.Name);
            using (var cnn = new MySql.Data.MySqlClient.MySqlConnection(cnnstr))
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    //cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    //cmd.CommandText = "sys.sp_rename";
                    foreach (var kv in dictColName)
                    {
                        if (string.IsNullOrEmpty(kv.Value) || kv.Key.Equals(kv.Value, StringComparison.CurrentCultureIgnoreCase))
                            continue;
                        //                                                      alter table <表名> change <字段名> <字段新名称> <字段的类型>
                        cmd.CommandText = string.Format("alter table {0} change {1} {2} {3}", metadata.Name,
                            kv.Key,
                            kv.Value,
                            metadata.Columns.FirstOrDefault(x => x.ColumnName.Equals(kv.Key, StringComparison.CurrentCultureIgnoreCase)).DataTypeName);
                        //cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("objname", meta.Name + "." + kv.Key);
                        //cmd.Parameters.AddWithValue("newname", kv.Value);
                        //cmd.Parameters.AddWithValue("objtype", "column");
                        cmd.ExecuteNonQuery();
                    }
                    if (!string.IsNullOrEmpty(meta.ReName))
                    {
                        cmd.CommandText = string.Format("RENAME TABLE {0} TO {1}", meta.Name, meta.ReName);
                        //cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("objname", meta.Name);
                        //cmd.Parameters.AddWithValue("newname", meta.ReName);
                        //cmd.Parameters.AddWithValue("objtype", DBNull.Value);
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
        bool IDbManager.TableDesigner(TableMeta metaNew)
        {
            TableMeta metaOld = ((IDbManager)this).GetTableMeta(metaNew.Name);
            //新增的列
            var lstColAdd = metaNew.Columns.Except(metaOld.Columns, new ColumnMetaComparer()).ToList();
            var lstsqlAdd = lstColAdd.Select(x => string.Format("ALTER TABLE {0} ADD {1} {2} {3} COMMENT '{4}'", metaNew.Name,
                x.ColumnName,
                x.DataTypeName,
                //x.DataTypeName.Contains("char") ? "(" + x.ColumnSize + ")" : string.Empty,
                //"id".Equals(x.ColumnName, StringComparison.CurrentCultureIgnoreCase) ? "PRIMARY KEY" + (x.DataTypeName.Contains("int") ? " AUTO_INCREMENT " : string.Empty) : string.Empty,
                x.AllowDBNull ? "NULL" : "NOT NULL",
                x.Description ?? string.Empty)).ToList();
            //移除的列
            var lstColDel = metaOld.Columns.Except(metaNew.Columns, new ColumnMetaComparer()).ToList();
            var lstsqlDel = lstColDel.Select(x => string.Format("ALTER TABLE {0} DROP COLUMN {1}", metaNew.Name, x.ColumnName)).ToList();
            //修改的列
            var lstColEdit = metaNew.Columns.Join(metaOld.Columns, x => x.ColumnName, x => x.ColumnName, (x, y) => x).ToList();
            var lstsqlEdit = lstColEdit.Select(x => string.Format("ALTER TABLE {0} modify COLUMN {1} {2} {3} COMMENT '{4}'", metaNew.Name,
                x.ColumnName,
                x.DataTypeName,
                //x.DataTypeName.Contains("char") ? "(" + x.ColumnSize + ")" : string.Empty,
                // "id".Equals(x.ColumnName, StringComparison.CurrentCultureIgnoreCase) ? "PRIMARY KEY" + (x.DataTypeName.Contains("int") ? " AUTO_INCREMENT " : string.Empty) : string.Empty,
                x.AllowDBNull ? "NULL" : "NOT NULL",
                x.Description ?? string.Empty)).ToList();
            //string spAdd = "sys.sp_addextendedproperty";
            //string spEdit = "sys.sp_updateextendedproperty";
            //string spDel = "sys.sp_dropextendedproperty";
            using (var cnn = new MySql.Data.MySqlClient.MySqlConnection(this.cnnstr))
            {
                cnn.Open();
                using (var cmd = cnn.CreateCommand())
                {
                    DescriptionHandlerWrapper(cmd, string.Format("alter table {0} comment '{1}'", metaNew.Name, metaNew.Description ?? string.Empty));
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
                    foreach (var sqlstr in lstsqlDel)
                    {//删除列
                        cmd.CommandText = sqlstr;
                        cmd.ExecuteNonQuery();
                    }
                    //foreach (var col in lstColEdit)
                    //{
                    //    cmd.CommandText = string.Format("alter table {0} modify column {1} {2} comment '{3}'", metaOld.Name, col.ColumnName, col.DataTypeName, col.Description);
                    //    cmd.ExecuteNonQuery();
                    //}

                }
            }
            return true;
        }

        List<TableMeta> IDbManager.GetTableMeta()
        {
            throw new NotImplementedException();
        }

        private bool DescriptionHandlerWrapper(MySql.Data.MySqlClient.MySqlCommand cmd, string cmdstr)
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
