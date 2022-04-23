﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TenpowerTalk
{
    public class RootData
    {
        public LoginUser User { get; set; }

        public RootData(LoginUser loginUser)
        {
            this.User = loginUser;
        }

        public void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("你点我了");
        }
    }
}
