using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel.DbTool
{
    /// <summary>
    /// 列的元数据
    /// </summary>
    public class ColumnMeta
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public double Size { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }
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
    /// 列的比较器，按列名称比较
    /// </summary>
    public class ColumnMetaComparer : IEqualityComparer<ColumnMeta>
    {
        public bool Equals(ColumnMeta x, ColumnMeta y)
        {
            if (string.IsNullOrEmpty(x.Name) || string.IsNullOrEmpty(y.Name))
                return false;
            return x.Name.Equals(y.Name, StringComparison.CurrentCultureIgnoreCase);
        }

        public int GetHashCode(ColumnMeta obj)
        {
            if (obj == null)
                return 0;
            return obj.Name.GetHashCode();
        }
    }

}
