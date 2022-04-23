using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HzTalk
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
                this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("LoginName"));
            }
        }

        public string Mark { get; set; }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public LoginUser()
        {
            this.LoginName = "eeroom";
            this.Mark = "天天向上";
        }
    }
}
