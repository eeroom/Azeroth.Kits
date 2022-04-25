using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HzTalk
{
    public interface IUserInfo : System.ComponentModel.INotifyPropertyChanged
    {
        string LoginName { set; get; }
        string Mark { get; set; }

    }
}
