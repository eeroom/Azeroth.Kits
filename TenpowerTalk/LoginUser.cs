using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TenpowerTalk
{
    public class LoginUser : System.ComponentModel.INotifyPropertyChanged
    {
        string loginName;
        public string LoginName
        {
            get { return this.loginName; }
            set
            {
                this.loginName = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LoginName"));
            }
        }

        public string Mark { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public LoginUser()
        {
            this.LoginName = "eeroom";
            this.Mark = "天天向上";
        }
    }
}
