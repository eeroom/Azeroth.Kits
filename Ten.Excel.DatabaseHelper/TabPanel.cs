using ExcelDna.Integration.CustomUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using stdole;

namespace Ten.Excel.DatabaseHelper
{
    public class TabPanel : ExcelDna.Integration.CustomUI.ExcelRibbon, ExcelDna.Integration.IExcelAddIn,ICallback
    {
        public void AutoClose()
        {
            
        }

        public void AutoOpen()
        {
            
        }

        public override string GetCustomUI(string RibbonID)
        {
            var xml= System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Ten.Excel.DatabaseHelper.Tab.xml");
            using (var reader=new System.IO.StreamReader(xml))
            {
                return reader.ReadToEnd();
            }
        }

        public bool getEnabled(IRibbonControl control)
        {
            return this.Handler<bool>(control);
        }

       

        public IPictureDisp getImage(IRibbonControl control)
        {
            return this.Handler<IPictureDisp>(control);
        }

        public int getItemCount(IRibbonControl control)
        {
            return this.Handler<int>(control);
        }

        public string getItemID(IRibbonControl control, int index)
        {
            return this.Handler<string>(control, index);
        }

        public IPictureDisp getItemImage(IRibbonControl control, int index)
        {
            return this.Handler<IPictureDisp>(control, index);
        }

        public string getItemLabel(IRibbonControl control, int index)
        {
            return this.Handler<string>(control, index);
        }

        public string getLabel(IRibbonControl control)
        {
            return this.Handler<string>(control);
        }

        public string getSelectedItemID(IRibbonControl control)
        {
            return this.Handler<string>(control);
        }

        public int getSelectedItemIndex(IRibbonControl control)
        {
            return this.Handler<int>(control);
        }

        public bool getShowImage(IRibbonControl control)
        {
            return this.Handler<bool>(control);
        }

        public bool getShowLabel(IRibbonControl control)
        {
            return this.Handler<bool>(control);
        }

        public string getText(IRibbonControl control)
        {
            return this.Handler<string>(control);
        }

        public bool getVisible(IRibbonControl control)
        {
            return this.Handler<bool>(control);
        }

        public void onAction(IRibbonControl control)
        {
            this.Handler(control);
        }

        public void onAction(IRibbonControl control, bool pressed)
        {
             this.Handler(control, pressed);
        }

        public void onAction(IRibbonControl control, string selectedId, int selectedIndex)
        {
            this.Handler(control, selectedId, selectedIndex);
        }

        public void onChange(IRibbonControl control, string text)
        {
            this.Handler(control, text);
        }

        IRibbonUI ribbon;
        Dictionary<string, Controller> dictController;
        Dictionary<string, Type> dictMeta;
        public void onLoad(IRibbonUI ribbon)
        {
            this.ribbon = ribbon;
        
        }

        private T Handler<T>(params object[] parameters)
        {
            var rc= parameters[0] as IRibbonControl;
            var tmp = rc.Tag.Split('/');
            var controller = tmp[0]+"Controller";
            var action = tmp[1];
            var rt= (T)dictMeta[controller].GetMethod(action, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).Invoke(dictController[controller], parameters);
            return rt;
        }

        private void Handler(params object[] parameters)
        {
            var rc = parameters[0] as IRibbonControl;
            var tmp = rc.Tag.Split('/');
            var controller = tmp[0] + "Controller";
            var action = tmp[1];
            dictMeta[controller].GetMethod(action, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).Invoke(dictController[controller], parameters);
            this.ribbon.Invalidate();
        }

        public TabPanel()
        {
            var meta = typeof(Controller);
            var lstmeta = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(x => meta.IsAssignableFrom(x) && meta != x).ToList();
            this.dictController = lstmeta.ToDictionary(x => x.Name, x => (Controller)System.Activator.CreateInstance(x));
            Model md = new Model();
            foreach (var kv in dictController)
            {
                kv.Value.State = md;
            }
            this.dictMeta = lstmeta.ToDictionary(x => x.Name, x => x);
        }
    }
}
