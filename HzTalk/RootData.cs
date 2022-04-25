using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HzTalk
{
    public class RootData
    {
        public IUserInfo User { get; set; }

        public RootData(IUserInfo loginUser)
        {
            this.User = loginUser;
        }

        public void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("你点我了");
        }
    }
}
