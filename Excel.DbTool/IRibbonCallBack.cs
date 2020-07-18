using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel.DbTool
{
    public interface IRibbonCallBack
    {
        string getDescription(Microsoft.Office.Core.IRibbonControl control);

        bool getEnabled(Microsoft.Office.Core.IRibbonControl control);

        stdole.IPictureDisp getImage(Microsoft.Office.Core.IRibbonControl control);

        string getImageMso(Microsoft.Office.Core.IRibbonControl control);

        string getLabel(Microsoft.Office.Core.IRibbonControl control);

        string getKeytip(Microsoft.Office.Core.IRibbonControl control);

        Microsoft.Office.Core.RibbonControlSize getSize(Microsoft.Office.Core.IRibbonControl control);

        string getScreentip(Microsoft.Office.Core.IRibbonControl control);

        bool getVisible(Microsoft.Office.Core.IRibbonControl control);

        bool getShowImage(Microsoft.Office.Core.IRibbonControl control);
        bool getShowLabel(Microsoft.Office.Core.IRibbonControl control);
        void onAction(Microsoft.Office.Core.IRibbonControl control);
        bool getPressed(Microsoft.Office.Core.IRibbonControl control);
        int getItemCount(Microsoft.Office.Core.IRibbonControl control);
        string getItemID(Microsoft.Office.Core.IRibbonControl control, int index);
        stdole.IPictureDisp getItemImage(Microsoft.Office.Core.IRibbonControl control, int index);
        string getItemLabel(Microsoft.Office.Core.IRibbonControl control, int index);
        string getItemScreenTip(Microsoft.Office.Core.IRibbonControl control,int index);
        string getScreentip(Microsoft.Office.Core.IRibbonControl control, int index);
        string getItemSuperTip(Microsoft.Office.Core.IRibbonControl control, int index);
        string getText(Microsoft.Office.Core.IRibbonControl control);
        void onChange(Microsoft.Office.Core.IRibbonControl control,string text);
        string getSelectedItemID(Microsoft.Office.Core.IRibbonControl control);
        int getSelectedItemIndex(Microsoft.Office.Core.IRibbonControl control);
        void onActiondropDown(Microsoft.Office.Core.IRibbonControl control, string selectedId, int selectedIndex);
        string getContent(Microsoft.Office.Core.IRibbonControl control);
        int getItemHeight(Microsoft.Office.Core.IRibbonControl control);
        int getItemWidth(Microsoft.Office.Core.IRibbonControl control);
        string getTitle(Microsoft.Office.Core.IRibbonControl control);
        string onAction(Microsoft.Office.Core.IRibbonControl control, bool pressed);
    }
}
