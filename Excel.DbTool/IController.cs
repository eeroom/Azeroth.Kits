using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel.DbTool
{
    public interface IController
    {
        void Start(Microsoft.Office.Core.IRibbonUI ribbonUI);
    }
}
