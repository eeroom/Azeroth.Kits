using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel.DbTool
{
    public class Comment
    {
        public string TableName { get; set; }
        public string Remark { get; set; }

        public string ColumnName { get; set; }

        public CommentTarget Target { get; set; }
    }

    
}
