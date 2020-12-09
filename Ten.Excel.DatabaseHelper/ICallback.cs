using ExcelDna.Integration.CustomUI;
using stdole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ten.Excel.DatabaseHelper
{
    /// <summary>
    /// The following table lists all of the callbacks, along with their procedure signatures
    /// </summary>
    public interface ICallback
    {
        //string GetDescription(IRibbonControl control);

        /// <summary>
        /// several controls
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        bool getEnabled(IRibbonControl control);

        /// <summary>
        /// several controls
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        IPictureDisp getImage(IRibbonControl control);

        //string GetImageMso(IRibbonControl control);

        /// <summary>
        /// several controls
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        string getLabel(IRibbonControl control);

        //string GetKeytip(IRibbonControl control);

        //RibbonControlSize GetSize(IRibbonControl control);

        //string GetScreentip(IRibbonControl control);

        //string GetSupertip(IRibbonControl control);

        /// <summary>
        /// several controls
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        bool getVisible(IRibbonControl control);

        /// <summary>
        /// several controls
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        bool getShowImage(IRibbonControl control);

        /// <summary>
        /// button
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        bool getShowLabel(IRibbonControl control);

        /// <summary>
        /// button
        /// </summary>
        /// <param name="control"></param>
        void onAction(IRibbonControl control);

        /// <summary>
        /// checkBox
        /// </summary>
        /// <param name="control"></param>
        /// <param name="pressed"></param>
        void onAction(IRibbonControl control, bool pressed);

        /// <summary>
        /// comboBox
        /// dropDown
        /// </summary>
        /// <param name="control"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        string getItemID(IRibbonControl control, int index);

        /// <summary>
        /// comboBox
        /// dropDown
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        int getItemCount(IRibbonControl control);


        /// <summary>
        /// comboBox
        /// dropDown
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        IPictureDisp getItemImage(IRibbonControl control, int index);

        /// <summary>
        /// comboBox
        /// dropDown
        /// </summary>
        /// <param name="control"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        string getItemLabel(IRibbonControl control, int index);

        /// <summary>
        /// comboBox
        /// editBox
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        string getText(IRibbonControl control);
        /// <summary>
        /// comboBox
        /// editBox
        /// </summary>
        /// <param name="control"></param>
        /// <param name="text"></param>
        void onChange(IRibbonControl control, string text);

        /// <summary>
        /// customUI
        /// </summary>
        /// <param name="ribbon"></param>
        void onLoad(IRibbonUI ribbon);

        /// <summary>
        /// dropDown
        /// </summary>
        /// <param name="control"></param>
        /// <param name="selectedId"></param>
        /// <param name="selectedIndex"></param>
        void onAction(IRibbonControl control, string selectedId, int selectedIndex);

        /// <summary>
        /// dropDown
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        int getSelectedItemIndex(IRibbonControl control);

        /// <summary>
        /// dropDown
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        string getSelectedItemID(IRibbonControl control);
    }
}
