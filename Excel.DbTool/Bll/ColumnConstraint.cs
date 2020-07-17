using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel.DbTool
{
    public class ColumnConstraint
    {
        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public string Name { get; set; }

        public ConstraintType Type { get; set; }

        public ConstraintType HandlerType(string value)
        {
            value = value.ToUpper();
            if (value == "PRIMARY KEY")
                return ConstraintType.主键;
            if (value == "UNIQUE")
                return ConstraintType.唯一;
            return ConstraintType.未识别;
        }

        public string GetDDLSegment(DbType mssqlserver)
        {
            //PRIMARY KEY CLUSTERED([Id] ASC),
            //UNIQUE NONCLUSTERED ([Name] ASC)
            if (this.Type == ConstraintType.主键)
                return $"PRIMARY KEY CLUSTERED([{this.ColumnName}] ASC)";
            if (this.Type == ConstraintType.唯一)
                return $"UNIQUE NONCLUSTERED([{this.ColumnName}] ASC)";
            return string.Empty;
        }
    }
    public enum ConstraintType
    {
        未识别=0,
        主键=1,
        唯一=2,
        非空=4,
    }


}
