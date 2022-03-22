using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelUserFunctionXll
{
    public class RtdContext
    {
        public int TopicId { get; set; }

        public string Action { get; set; }

        public int Interval { get; set; }

        public object Value { get; set; }

        public Array Arguments { get; set; }

        public bool DeleteFlag { get; set; }

        public System.Threading.Timer MyTimer { set; get; }
    }
}
