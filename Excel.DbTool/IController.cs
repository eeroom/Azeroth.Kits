using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel.DbTool
{
    public abstract class Controller
    {
        protected DkRibbon DkRibbon;

        public void Init(DkRibbon dkRibbon)
        {
            this.DkRibbon = dkRibbon;
        }
    }
}
