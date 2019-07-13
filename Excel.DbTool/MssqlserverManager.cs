using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Excel.DbTool
{
    public class MssqlserverManager:DbManager,IDbManager
    {
        readonly string SpRemarkAdd= "sys.sp_addextendedproperty";
        readonly string SpColumnRename = "sys.sp_rename";
        readonly string SpRemarkDel = "sys.sp_dropextendedproperty";
        readonly string SpRemarkEdit = "sys.sp_updateextendedproperty";
        public override DbType GetDbType()
        {
            return DbType.mssqlserver;
        }

        public override List<string> GetAllTableName()
        {
            if (string.IsNullOrEmpty(this.Cnnstr))
                throw new ArgumentException("必须指定数据库连接字符串");
            string cmdstr= @"select TABLE_NAME
                                        from INFORMATION_SCHEMA.TABLES
                                        where TABLE_TYPE='BASE TABLE'";
          var lstTableMeta=  SqlHelper<SqlConnection>.ExecuteReader<TableMeta>(this.Cnnstr, cmdstr)
                .Invoke((reader, entity) => {
                    entity.Name = reader["TABLE_NAME"].ToString();
                });
            return lstTableMeta.Select(x => x.Name).OrderBy(x => x).ToList();
        }

        public override TableMeta GetTableMetaByName(string tname)
        {
            TableMeta tableMeta = new TableMeta() {  Name=tname};
            string cmdstrColum = @"select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TABLE_NAME";
            tableMeta.Columns = SqlHelper<SqlConnection>.ExecuteReader<ColumnMeta>(this.Cnnstr, cmdstrColum, new SqlParameter("@TABLE_NAME", tableMeta.Name))
                .Invoke((reader, entity) => {
                entity.TableName = reader["TABLE_NAME"].ToString();
                entity.Name = reader["COLUMN_NAME"].ToString();
                entity.DataType = reader["DATA_TYPE"].ToString();
                entity.CharacterMaxLength = reader["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value ? default(Nullable<int>) : (int)reader["CHARACTER_MAXIMUM_LENGTH"];
                entity.UnNullable = reader["IS_NULLABLE"].ToString().ToLower() == "no";
            });
            string cmdstrRemark = @"select cl.name as ColumnName,ep.value as Remark,1 as Target,ao.name as TableName
											from  sys.extended_properties ep
                                            join sys.columns cl on  cl.object_id=ep.major_id and  cl.column_id=ep.minor_id
											join sys.all_objects ao on ao.object_id=cl.object_id
                                            where ao.name=@TABLE_NAME and type='U'
                                            union all
                                            select ao2.name,value,2,ao2.name
                                            from  sys.extended_properties ep2
											join sys.all_objects ao2 on ao2.object_id=ep2.major_id and ep2.minor_id=0
                                            where ao2.name=@TABLE_NAME and type='U'";
            var lstComment = SqlHelper<SqlConnection>.ExecuteReader<Comment>(this.Cnnstr, cmdstrRemark, new SqlParameter("@TABLE_NAME", tableMeta.Name))
                .Invoke((reader, entity) => {
                    entity.TableName = reader["TableName"].ToString();
                    entity.ColumnName = reader["ColumnName"].ToString();
                    entity.Remark = reader["Remark"].ToString();
                    entity.Target = (CommentTarget)reader["Target"];
                });

            string cmdstrConstrant = @"select ccu.TABLE_NAME,ccu.CONSTRAINT_NAME,ccu.COLUMN_NAME,tc.CONSTRAINT_TYPE 
                                                        from INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu
                                                        join INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc on ccu.TABLE_CATALOG=tc.TABLE_CATALOG and ccu.CONSTRAINT_NAME=tc.CONSTRAINT_NAME
                                                        where ccu.TABLE_NAME=@TABLE_NAME";
            var lstConstraint = SqlHelper<SqlConnection>.ExecuteReader<ColumnConstraint>(this.Cnnstr, cmdstrConstrant, new SqlParameter("@TABLE_NAME", tableMeta.Name))
                .Invoke((reader, entity) => {
                    entity.ColumnName = reader["COLUMN_NAME"].ToString();
                    entity.TableName = reader["TABLE_NAME"].ToString();
                    entity.Name = reader["CONSTRAINT_NAME"].ToString();
                    entity.Type = entity.HandlerType(reader["CONSTRAINT_TYPE"].ToString());
                });

            tableMeta.Remark = lstComment.FirstOrDefault(x => x.Target == CommentTarget.表)?.Remark;
            tableMeta.Columns.ForEach(col =>
            {
                col.Comment = lstComment.FirstOrDefault(x => x.TableName == col.TableName && x.ColumnName == col.Name);
                col.Constraints = lstConstraint.Where(x => x.TableName == col.TableName && x.ColumnName == col.Name).ToList();
            });
            return tableMeta;
        }

        /// <summary>
        /// 新增表
        /// </summary>
        /// <param name="tableMeta"></param>
        /// <returns></returns>
        public override bool CreateTable(TableMeta tableMeta)
        {
            List<string> lstSegment = tableMeta.Columns
                .Select(col=> $"[{col.Name}] {col.DataType} {col.GetNullableString()}")
                .ToList();
            List<string> lstConstraintSegment= tableMeta.Columns.SelectMany(x => x.Constraints)
                .Select(x => x.GetDDLSegment(DbType.mssqlserver))
                .Where(x=>!string.IsNullOrEmpty(x))
                .ToList();
            lstSegment.AddRange(lstConstraintSegment);
            string cmdstr = $"CREATE TABLE [{tableMeta.Name}]({string.Join(",", lstSegment)})";
            SqlHelper<SqlConnection>.ExecuteNonQuery(Cnnstr, cmdstr);
            //处理注释
            SqlHelper<SqlConnection>.ExecuteNonQuery(Cnnstr, this.SpRemarkAdd,cmd=> {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("name", "MS_Description"));
                cmd.Parameters.Add(new SqlParameter("level0type", "SCHEMA"));
                cmd.Parameters.Add(new SqlParameter("level0name", "dbo"));
                cmd.Parameters.Add(new SqlParameter("level1type", "TABLE"));
                cmd.Parameters.Add(new SqlParameter("level1name", tableMeta.Name));
                cmd.Parameters.Add(new SqlParameter("level2type", DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("level2name", DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("value", tableMeta.Remark ?? string.Empty));
                cmd.ExecuteNonQuery();//增加表的说明
                ((SqlParameter)cmd.Parameters["level2type"]).Value = "COLUMN";
                tableMeta.Columns.ForEach(col =>
                {
                    ((SqlParameter)cmd.Parameters["level2name"]).Value = col.Name;
                    ((SqlParameter)cmd.Parameters["value"]).Value = col.Comment.Remark ?? string.Empty;
                    cmd.ExecuteNonQuery();
                });
                return false;//约定
            });
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
          
            SqlHelper<SqlConnection>.ExecuteNonQuery(Cnnstr, SpColumnRename, cmd => {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                if (!string.IsNullOrEmpty(meta.NewName))
                {//重命名表
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("objname", meta.Name));
                    cmd.Parameters.Add(new SqlParameter("newname", meta.NewName));
                    cmd.Parameters.Add(new SqlParameter("objtype", DBNull.Value));
                    cmd.ExecuteNonQuery();
                }
                foreach (var kv in dictColName)
                {//重命名列
                    if (string.IsNullOrEmpty(kv.Value) || kv.Key.Equals(kv.Value, StringComparison.CurrentCultureIgnoreCase))
                        continue;//未改变
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("objname", meta.Name + "." + kv.Key));
                    cmd.Parameters.Add(new SqlParameter("newname", kv.Value));
                    cmd.Parameters.Add(new SqlParameter("objtype", "column"));
                    cmd.ExecuteNonQuery();
                }
                return false;//约定
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
            TableMeta metaOld = this.GetTableMetaByName(metaNew.Name);
            //新增的列
            var lstColAdd = metaNew.Columns.Except(metaOld.Columns).ToList();
            var lstCmdAdd = lstColAdd.Select(x => $"ALTER TABLE {metaNew.Name} ADD {x.Name} {x.DataType} {x.GetNullableString()}")
                .ToList();
            AlterTableWithAddColumn(metaNew,lstColAdd, lstCmdAdd);
            //移除的列
            var lstColDel = metaOld.Columns.Except(metaNew.Columns).ToList();
            var lstCmdDel = lstColDel.Select(x => string.Format("ALTER TABLE {0} DROP COLUMN {1}", metaNew.Name, x.Name)).ToList();
            AlterTableWithDelColumn(metaNew, lstColDel, lstCmdDel);
            //修改的列
            var lstColEdit = metaNew.Columns.Join(metaOld.Columns, x => x.Name, x => x.Name, (x, y) => x).ToList();
            var lstCmdEdit = lstColEdit.Select(x => $"ALTER TABLE {metaNew.Name} ALTER COLUMN {x.Name} {x.DataType} {x.GetNullableString()}").ToList();
          
            SqlHelper<SqlConnection>.ExecuteNonQuery(this.Cnnstr, string.Empty, cmd =>
            {
                lstCmdEdit.ForEach(cmdstr =>
                {//修改的列
                    cmd.CommandText = cmdstr;
                    cmd.ExecuteNonQuery();
                });
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("name", "MS_Description"));
                cmd.Parameters.Add(new SqlParameter("level0type", "SCHEMA"));
                cmd.Parameters.Add(new SqlParameter("level0name", "dbo"));
                cmd.Parameters.Add(new SqlParameter("level1type", "TABLE"));
                cmd.Parameters.Add(new SqlParameter("level1name", metaNew.Name));
                cmd.Parameters.Add(new SqlParameter("level2type", DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("level2name", DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("value", metaNew.Remark ?? string.Empty));
                if (!TryExecuteNonQuery(cmd, this.SpRemarkEdit))//可能表是别人之前建的，所以没有值
                    TryExecuteNonQuery(cmd, this.SpRemarkAdd);//可能表是别人之前建的，所以没有值
                ((SqlParameter)cmd.Parameters["level2type"]).Value = "COLUMN";
                lstColEdit.ForEach(col =>
                {
                    ((SqlParameter)cmd.Parameters["level2name"]).Value = col.Name;
                    ((SqlParameter)cmd.Parameters["value"]).Value = col.Comment.Remark ?? string.Empty;
                    if (!TryExecuteNonQuery(cmd, this.SpRemarkEdit))//可能原来没值
                        TryExecuteNonQuery(cmd, this.SpRemarkAdd);//所以直接新增
                });
                return false;
            });
            return true;
        }

        private  void AlterTableWithDelColumn(TableMeta tableMeta, List<ColumnMeta> lstCol, List<string> lstCmdstr)
        {
            SqlHelper<SqlConnection>.ExecuteNonQuery(this.Cnnstr, string.Empty, cmd =>
            {
                cmd.CommandText = this.SpRemarkDel;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("name", "MS_Description"));
                cmd.Parameters.Add(new SqlParameter("level0type", "SCHEMA"));
                cmd.Parameters.Add(new SqlParameter("level0name", "dbo"));
                cmd.Parameters.Add(new SqlParameter("level1type", "TABLE"));
                cmd.Parameters.Add(new SqlParameter("level1name", tableMeta.Name));
                cmd.Parameters.Add(new SqlParameter("level2type", "COLUMN"));
                cmd.Parameters.Add(new SqlParameter("level2name", DBNull.Value));
                //cmd.Parameters.Add(new SqlParameter("value", string.Empty));
                lstCol.ForEach(col =>
                {
                    ((SqlParameter)cmd.Parameters["level2name"]).Value = col.Name;
                    //((SqlParameter)cmd.Parameters["value"]).Value = col.Comment.Remark ?? string.Empty;
                    TryExecuteNonQuery(cmd, this.SpRemarkDel);//可能表是别人之前建的，没有注释，
                });
                cmd.CommandType = CommandType.Text;
                lstCmdstr.ForEach(sqlstr =>
                {//先删除描述，再删除列
                    cmd.CommandText = sqlstr;
                    cmd.ExecuteNonQuery();
                });
                return false;
            });
        }

        private  void AlterTableWithAddColumn(TableMeta tableMeta, List<ColumnMeta> lstCol, List<string> lstCmdstr)
        {
            SqlHelper<SqlConnection>.ExecuteNonQuery(this.Cnnstr, string.Empty, cmd =>
            {
                lstCmdstr.ForEach(sqlstr =>
                {//新增列
                    cmd.CommandText = sqlstr;
                    cmd.ExecuteNonQuery();
                });
                cmd.CommandText = this.SpRemarkAdd;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("name", "MS_Description"));
                cmd.Parameters.Add(new SqlParameter("level0type", "SCHEMA"));
                cmd.Parameters.Add(new SqlParameter("level0name", "dbo"));
                cmd.Parameters.Add(new SqlParameter("level1type", "TABLE"));
                cmd.Parameters.Add(new SqlParameter("level1name", tableMeta.Name));
                cmd.Parameters.Add(new SqlParameter("level2type", "COLUMN"));
                cmd.Parameters.Add(new SqlParameter("level2name", DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("value", string.Empty));
                lstCol.ForEach(col =>
                {
                    ((SqlParameter)cmd.Parameters["level2name"]).Value = col.Name;
                    ((SqlParameter)cmd.Parameters["value"]).Value = col.Comment.Remark ?? string.Empty;
                    cmd.ExecuteNonQuery();
                });
                return false;
            });
        }

        public override List<TableMeta> GetTableMeta()
        {
            throw new NotImplementedException();
        }
    }
}
