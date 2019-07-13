using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel.DbTool
{
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
        public string Remark { get; set; }

        /// <summary>
        /// 新名称
        /// </summary>
        public string NewName { get; set; }
    }

}
