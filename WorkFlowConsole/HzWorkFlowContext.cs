using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkFlowConsole {
    public class HzWorkFlowContext {

        public Guid Id { get; set; }

        public string Creator { get; set; }

        public DateTime CreateDateTime { get; set; }

        public string FormData { get; set; }

        
    }
}
