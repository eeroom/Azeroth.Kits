using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenpowerTalk
{
    public class RootData
    {
        public LoginUser User { get; set; }

        public RootData()
        {
            this.User = new LoginUser() { LoginName = "eeroom" };
        }
    }
}
