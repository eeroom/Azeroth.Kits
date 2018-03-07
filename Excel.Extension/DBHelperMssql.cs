using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel.Extension
{
    public class DBHelperMssql:IDBHelper
    {
        const string ALL = "全部";
        const string TableRemarkColName = "_TableName_";
        public DBProvider GetProvider()
        {
            return DBProvider.mssql;
        }

        public List<string> InitDrpTableListByMssql(string cnnstr)
        {
            List<string> lstTableName = new List<string>();
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
                            lstTableName.Add(reader["TABLE_NAME"].ToString());
                        }
                    }
                }
            }
            return lstTableName;
        }

        public void GetTableSchemalByMssqlInternal(string cnnstr, string tableName, out System.Data.DataTable table, out System.Data.DataTable tableRemark)
        {
            using (System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(cnnstr))
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    cmd.CommandText = string.Format("select * from {0}", tableName);
                    using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.SchemaOnly))
                    {
                        table = reader.GetSchemaTable();
                    }
                }
            }
            string cmdstr = string.Format(@"select cl.name,ep.value
                                            from sys.columns cl
                                            inner join sys.extended_properties ep on cl.column_id=ep.minor_id
                                            where cl.object_id=OBJECT_ID('{0}') and ep.major_id=OBJECT_ID('{0}')
                                            union all
                                            select '{1}',value 
                                            from sys.extended_properties 
                                            where sys.extended_properties.major_id=OBJECT_ID('{0}') and minor_id=0", tableName, TableRemarkColName);
            tableRemark = new System.Data.DataTable();
            using (System.Data.SqlClient.SqlDataAdapter adpater = new System.Data.SqlClient.SqlDataAdapter(cmdstr, cnnstr))
            {
                adpater.Fill(tableRemark);
            }
        }

        public void AddTableMetaByMssqlWithDDL(string cnnstr, string tableName, object[,] value)
        {
            //创建新建表的sql语句
            List<string> lstsql = new List<string>();
            Dictionary<string, string> dictRemark = new Dictionary<string, string>();
            if (value[1, 5] != null)
                dictRemark.Add(tableName, value[1, 5].ToString());//表的注释
            for (int i = 3; i <= value.GetUpperBound(0); i++)
            {
                var tmp = string.Format("[{0}] [{1}]{2} {3} {4}", value[i, 1], value[i, 2],
                    value[i, 2].ToString().Contains("char") ? "(" + value[i, 3] + ")" : string.Empty,
                    value[i, 1].ToString().ToLower().Equals("id") ? "PRIMARY KEY CLUSTERED" + (value[i, 2].ToString().Contains("int") ? " IDENTITY(1,1)" : string.Empty) : string.Empty,
                    "是".Equals(value[i, 4]) ? "NULL" : "NOT NULL");
                lstsql.Add(tmp);
                if (value[i, 5] != null)
                    dictRemark.Add(value[i, 1].ToString(), value[i, 5].ToString());
            }
            using (var cnn = new System.Data.SqlClient.SqlConnection(cnnstr))
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    cmd.CommandText = string.Format("CREATE TABLE [{0}]({1})", value[1, 1], string.Join(",", lstsql));
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
                    cmd.Parameters.AddWithValue("value", "逻辑主键");
                    cmd.Parameters.AddWithValue("level0type", "SCHEMA");
                    cmd.Parameters.AddWithValue("level0name", "dbo");
                    cmd.Parameters.AddWithValue("level1type", "TABLE");
                    cmd.Parameters.AddWithValue("level1name", "Article");
                    cmd.Parameters.AddWithValue("level2type", "COLUMN");
                    cmd.Parameters.AddWithValue("level2name", "Id");
                    foreach (var kv in dictRemark)
                    {
                        cmd.Parameters["value"].Value = kv.Value;
                        cmd.Parameters["level1name"].Value = tableName;
                        cmd.Parameters["level2name"].Value = kv.Key.Equals(tableName) ? (object)DBNull.Value : kv.Key;
                        cmd.Parameters["level2type"].Value = kv.Key.Equals(tableName) ? (object)DBNull.Value : "COLUMN";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void EditTableMetaByMssqlWithDDL(string cnnstr, string tableName, object[,] value)
        {
            string spAddRemark = "sys.sp_addextendedproperty";
            string spEditRemark = "sys.sp_updateextendedproperty";
            string colName = "name";
            string colValue = "value";
            System.Data.DataTable tableRemark;
            System.Data.DataTable table;
            GetTableSchemalByMssqlInternal(cnnstr, tableName, out  table, out  tableRemark);
            var metaRows = table.Select();
            var remarkeRows = tableRemark.Select();
            System.Data.DataRow rowTmp;
            List<string> excelColmnNames = new List<string>();
            for (int i = 3; i <= value.GetUpperBound(0); i++)
            {
                var columnName = value[i, 1].ToString();
                excelColmnNames.Add(columnName);
                rowTmp = metaRows.FirstOrDefault(x => columnName.Equals(x["ColumnName"]));
                if (rowTmp == null)//因为是新增的列，所以原来的元数据里面没有
                    SaveColumnMeta("ADD", cnnstr, tableName, value[i, 1].ToString(), value[i, 2].ToString(), value[i, 3], value[i, 4]);
                else if (!rowTmp["DataTypeName"].Equals(value[i, 2]) || !((bool)rowTmp["AllowDBNull"] ? "是" : string.Empty).Equals(value[i, 4] ?? string.Empty) ||
                    (rowTmp["DataTypeName"].ToString().Contains("char") && !rowTmp["ColumnSize"].ToString().Equals(value[i, 3].ToString())))
                    SaveColumnMeta("ALTER COLUMN", cnnstr, tableName, value[i, 1].ToString(), value[i, 2].ToString(), value[i, 3], value[i, 4]);//修改了的列
            }
            //处理移除的列
            var deleteColumns = metaRows.Select(x => x["ColumnName"].ToString()).Except(excelColmnNames).ToList();
            deleteColumns.ForEach(x => DeleteColumnMeta(cnnstr, tableName, x, remarkeRows.Count(y => x.Equals(y["name"])) > 0));
            //表的描述信息额外处理
            rowTmp = remarkeRows.FirstOrDefault(x => TableRemarkColName.Equals(x[colName]));
            if (rowTmp == null && value[1, 5] != null)
                SaveColumnRemark(spAddRemark, cnnstr, tableName, value[1, 5].ToString());
            else if (!object.Equals(value[1, 5] ?? string.Empty, rowTmp != null ? rowTmp[colValue] : string.Empty))
                SaveColumnRemark(spEditRemark, cnnstr, tableName, (value[1, 5] ?? string.Empty).ToString());
            for (int i = 3; i <= value.GetUpperBound(0); i++)
            {//普通的描述信息
                var columnName = value[i, 1].ToString();
                rowTmp = remarkeRows.FirstOrDefault(x => columnName.Equals(x[colName]));
                if (rowTmp == null && value[i, 5] != null)
                    SaveColumnRemark(spAddRemark, cnnstr, tableName, value[i, 5].ToString(), columnName);
                if (!object.Equals(value[i, 5] ?? string.Empty, rowTmp != null ? rowTmp[colValue] : string.Empty))
                    SaveColumnRemark(spEditRemark, cnnstr, tableName, (value[i, 5]??string.Empty).ToString(), columnName);
            }
            
        }

        private void SaveColumnRemark(string spName, string cnnstr, string tableName, string remark)
        {
            using (var cnn = new System.Data.SqlClient.SqlConnection(cnnstr))
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.Parameters.AddWithValue("name", "MS_Description");
                    cmd.Parameters.AddWithValue("value", remark ?? string.Empty);
                    cmd.Parameters.AddWithValue("level0type", "SCHEMA");
                    cmd.Parameters.AddWithValue("level0name", "dbo");
                    cmd.Parameters.AddWithValue("level1type", "TABLE");
                    cmd.Parameters.AddWithValue("level1name", tableName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void SaveColumnRemark(string spName, string cnnstr, string tableName, string remark, string columnName)
        {
            using (var cnn = new System.Data.SqlClient.SqlConnection(cnnstr))
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.Parameters.AddWithValue("name", "MS_Description");
                    cmd.Parameters.AddWithValue("value", remark ?? string.Empty);
                    cmd.Parameters.AddWithValue("level0type", "SCHEMA");
                    cmd.Parameters.AddWithValue("level0name", "dbo");
                    cmd.Parameters.AddWithValue("level1type", "TABLE");
                    cmd.Parameters.AddWithValue("level1name", tableName);
                    cmd.Parameters.AddWithValue("level2type", "COLUMN");
                    cmd.Parameters.AddWithValue("level2name", columnName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void DeleteColumnMeta(string cnnstr, string tableName, string colName, bool hasRemark)
        {
            using (var cnn = new System.Data.SqlClient.SqlConnection(cnnstr))
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    if (hasRemark)
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandText = "sys.sp_dropextendedproperty";
                        cmd.Parameters.AddWithValue("name", "MS_Description");
                        cmd.Parameters.AddWithValue("level0type", "SCHEMA");
                        cmd.Parameters.AddWithValue("level0name", "dbo");
                        cmd.Parameters.AddWithValue("level1type", "TABLE");
                        cmd.Parameters.AddWithValue("level1name", tableName);
                        cmd.Parameters.AddWithValue("level2type", "COLUMN");
                        cmd.Parameters.AddWithValue("level2name", colName);
                        cmd.ExecuteNonQuery();
                    }
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.CommandText = string.Format("ALTER TABLE {0} DROP COLUMN {1}", tableName, colName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void SaveColumnMeta(string methodName, string cnnstr, string tableName, string colName, string colType, object colSize, object allowDbNull)
        {
            string cmdstr = string.Format("ALTER TABLE {0} {1} {2} {3}{4} {5} {6}", tableName,
                methodName,
                colName,
                colType,
                colType.Contains("char") ? "(" + colSize.ToString() + ")" : string.Empty,
                colName.ToLower().Equals("id") ? "PRIMARY KEY CLUSTERED" + (colType.Contains("int") ? " IDENTITY(1,1)" : string.Empty) : string.Empty,
                "是".Equals(allowDbNull) ? "NULL" : "NOT NULL");
            using (var cnn = new System.Data.SqlClient.SqlConnection(cnnstr))
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    cmd.CommandText = cmdstr;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void EditTableMetaByMssqlWithReName(string cnnstr, string tableName, object[,] value)
        {
            using (var cnn = new System.Data.SqlClient.SqlConnection(cnnstr))
            {
                using (var cmd = cnn.CreateCommand())
                {
                    cnn.Open();
                    for (int i = 1; i <= value.GetUpperBound(0); i++)
                    {
                        if (i == 2)
                            continue;
                        if (value[i, 6] == null)
                            continue;
                        var columnName = value[i, 1].ToString();
                        var columnNameNew = value[i, 6].ToString();
                        if (columnName.Equals(columnNameNew))
                            continue;
                        cmd.Parameters.Clear();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("objname", i == 1 ? tableName : tableName + "." + columnName);
                        cmd.Parameters.AddWithValue("newname", columnNameNew);
                        cmd.Parameters.AddWithValue("objtype", i == 1 ? DBNull.Value : (object)"column");
                        cmd.CommandText = "sys.sp_rename";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
