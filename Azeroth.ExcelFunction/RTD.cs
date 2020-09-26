using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azeroth.ExcelFunction
{
    [System.Runtime.InteropServices.ProgId("Function.RTD")]
    [System.Runtime.InteropServices.Guid("1EA8BE51-A7CE-4EC6-A66F-D7AF76F6EB7B")]
    [System.Runtime.InteropServices.ClassInterface( System.Runtime.InteropServices.ClassInterfaceType.AutoDual)]
    public class RTD :Microsoft.Office.Interop.Excel.IRtdServer
    {
        Microsoft.Office.Interop.Excel.IRTDUpdateEvent CallbackObject { set; get; }

        System.Timers.Timer Spollingor { set; get; }

        Dictionary<int, Array> dictCellAndParameter = new Dictionary<int, Array>();

        public dynamic ConnectData(int cellId, ref Array parameters, ref bool getNewValues)
        {//每个单元格调用这个RTD的时候，都会调用一次这个方法
            //parameters就是单元格中传的参数
            //cellId可以标识出每个调用该RTD单元格
            if (!dictCellAndParameter.ContainsKey(cellId))
                dictCellAndParameter.Add(cellId, parameters);
            getNewValues = true;
            return "等待中...";
        }

        public void DisconnectData(int cellId)
        {
            this.dictCellAndParameter.Remove(cellId);
        }

        public int Heartbeat()
        {
            return 1;//如果是0，excel不会再调用这个RTD，相当于该RTD停止
        }

        public Array RefreshData(ref int cellCount)
        {
            cellCount = dictCellAndParameter.Count;
            object[,] value=new object[2,dictCellAndParameter.Count];
            int index=0;
            foreach (var kv in dictCellAndParameter)
            {//第一行是单元格的ID，第二行是单元格的值，这是excel的约定，
                value[0, index] = kv.Key;
                value[1, index] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                index++;
            }
            return value;
        }

        public int ServerStart(Microsoft.Office.Interop.Excel.IRTDUpdateEvent callbackObject)
        {//这里是首次用到自动化服务器，然后创建这个类的实例后，调用的方法，（这个RTD的实例只会有一个）
            this.CallbackObject = callbackObject;
            this.Spollingor = new System.Timers.Timer();
            this.Spollingor.Interval = 5 * 1000;
            this.Spollingor.Elapsed += Spollingor_Elapsed;
            this.Spollingor.Start();
            return 1;
        }

        void Spollingor_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.CallbackObject.UpdateNotify();
        }

        public void ServerTerminate()
        {
            this.Spollingor.Enabled = false;
            this.Spollingor.Elapsed -= Spollingor_Elapsed;
            this.Spollingor.Dispose();
        }
    }
}
