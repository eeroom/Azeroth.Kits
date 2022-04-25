using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HzTalk
{
    public class LoginUser :ModelBase,IUserInfo
    {
        public string LoginName
        {
            get;
            set;
            
        }

        public string Mark { get; set; }


        public LoginUser()
        {
            this.LoginName = "eeroom";
            this.Mark = "天天向上33";
        }

    }
}
