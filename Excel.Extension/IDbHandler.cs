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

        /// <summary>
        /// 获取所有表格的元数据
        /// </summary>
        /// <returns></returns>
        List<TableMeta> GetTableMeta();

        /// <summary>
        /// 添加表格
        /// </summary>
        /// <param name="tablemeta"></param>
        /// <returns></returns>
        bool TableAdd(TableMeta tablemeta);

        /// <summary>
        /// 重命名，表名称，列名称
        /// </summary>
        /// <param name="dictName"></param>
        /// <returns></returns>
        bool TableDesignerReName(TableMeta tablemeta, Dictionary<string,string> dictName);

        /// <summary>
        /// 修改表设计，新增，修改，删除 列，没有重命名
        /// </summary>
        /// <param name="tablemeta"></param>
        /// <returns></returns>
        bool TableDesigner(TableMeta tablemeta);

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
        public double ColumnSize { get; set; }
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

    /// <summary>
    /// 表格元数据
    /// </summary>
    public class TableMeta
    {
        public TableMeta()
        {
            this.Columns = new List<ColumnMeta>();
        }
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

        /// <summary>
        /// 新名称
        /// </summary>
        public string ReName { get; set; }
    }

    /// <summary>
    /// 列的比较器，按列名称比较
    /// </summary>
    public class ColumnMetaComparer : IEqualityComparer<ColumnMeta>
    {
        public bool Equals(ColumnMeta x, ColumnMeta y)
        {
            if (string.IsNullOrEmpty(x.ColumnName) || string.IsNullOrEmpty(y.ColumnName))
                return false;
            return x.ColumnName.Equals(y.ColumnName, StringComparison.CurrentCultureIgnoreCase);
        }

        public int GetHashCode(ColumnMeta obj)
        {
            if (obj == null)
                return 0;
            return obj.ColumnName.GetHashCode();
        }
    }
}
