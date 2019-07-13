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
        public ColumnMeta()
        {
            this.Constraints = new List<ColumnConstraint>();
        }
        public string TableName { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public Nullable<int> CharacterMaxLength { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public Comment Comment { get; set; }
        /// <summary>
        /// 非空
        /// </summary>
        public bool UnNullable { get; set; }

        /// <summary>
        /// 约束
        /// </summary>
        public List<ColumnConstraint> Constraints { get; set; }

        public override bool Equals(object obj)
        {
            ColumnMeta target = obj as ColumnMeta;
            if (target == null)
                return false;
            if (string.IsNullOrEmpty(this.Name) || string.IsNullOrEmpty(target.Name))
                return false;
            return this.Name.ToLower() == target.Name.ToLower();
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public object GetNullableString()
        {
            if (this.UnNullable)
                return "NOT NULL";
            return "NULL";
        }
    }
}
