using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportPrint
{
    class Program
    {
        static void Main(string[] args)
        {
           var doc= new System.Drawing.Printing.PrintDocument();
            //默认的controller是System.Windows.Forms.PrintControllerWithStatusDialog的实例，会弹一个打印状态模态框，
            //System.Windows.Forms.PrintControllerWithStatusDialog是winform类库里的类，System.Drawing.Printing.PrintDocument会尝试反射获取这个controller的实例进行赋值
            //System.Drawing类库并不依赖System.Windows.Forms类库
            //如果是静默打印的场景，或者自己控制进度提示，就替换这个conroller
            doc.PrintController = new System.Drawing.Printing.StandardPrintController();
        }
    }
}
