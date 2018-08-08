using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel.Extension
{
    public interface IDbHandler
    {
        /// <summary>
        /// 获取数据库类别
        /// </summary>
        DbIndex DbIndex { get;}

        /// <summary>
        /// 设定数据库连接字符串
        /// </summary>
        /// <param name="cnnstr"></param>
        void SetDbConnectionString(string cnnstr);

        /// <summary>
        /// 获取数据库下的所有表
        /// </summary>
        /// <param name="cnnstr"></param>
        /// <returns></returns>
        List<string> GetAllTableName();

        /// <summary>
        /// 获取表的元数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        TableMeta GetTableMeta(string tableName);

        List<TableMeta> GetTableMeta();


        void GetTableSchemalByMssqlInternal(string cnnstr, string tableName, out System.Data.DataTable table, out System.Data.DataTable tableRemark);

        /// <summary>
        /// 添加表格
        /// </summary>
        /// <param name="tablemeta"></param>
        /// <returns></returns>
        bool AddTable(TableMeta tablemeta);

        void AddTableMetaByMssqlWithDDL(string cnnstr, string tableName, object[,] value);

        /// <summary>
        /// 重命名，表名称，列名称
        /// </summary>
        /// <param name="dictName"></param>
        /// <returns></returns>
        bool ReName(TableMeta tablemeta, Dictionary<string,string> dictName);

        void EditTableMetaByMssqlWithDDL(string cnnstr, string tableName, object[,] value);

        /// <summary>
        /// 修改表设计
        /// </summary>
        /// <param name="tablemeta"></param>
        /// <returns></returns>
        bool EditTable(TableMeta tablemeta);

        void EditTableMetaByMssqlWithReName(string cnnstr, string tableName, object[,] value);
    }

    /// <summary>
    /// 列的元数据
    /// </summary>
    public class ColumnMeta
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public int ColumnSize { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataTypeName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 是否可以为null
        /// </summary>
        public bool AllowDBNull { get; set; }
    }

    public class TableMeta
    {
        /// <summary>
        /// 列信息
        /// </summary>
        public List<ColumnMeta> Columns { get; set; }
        /// <summary>
        /// 表名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
