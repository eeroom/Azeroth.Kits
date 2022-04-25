using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HzTalk
{
    public class ModelBase : System.ComponentModel.INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void DispathINotifyPropertyChanged(object target,string pName)
        {
            this.PropertyChanged.Invoke(target, new PropertyChangedEventArgs(pName));
        }
    }
}
