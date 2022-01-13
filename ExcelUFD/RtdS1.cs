using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExcelDna.Integration.Rtd;

namespace ExcelUFD
{
    /// <summary>
    /// 使用方法：1、注册com组件，使用cmd的regsvr32 本项目打包依赖的xll.xll，com严格区分32位和64位，需要和excel的版本x86或x64适配
    /// 2、在excel中直接使用公式：=RTD("excelufd.rtds1","","刷新间隔","Action",action的其他参数)
    /// excel-RTD的约定：第一个参数是ProgId,第二个参数是计算机名称，如果本机就空串，第三个参数开始对应于给我们的参数
    /// Rtds1的约定：给我们参数里面，第一个是刷新间隔，第二个是action，后面是action要用到的参数
    /// </summary>
    [System.Runtime.InteropServices.ProgId("ExcelUFD.RtdS1")]
    [System.Runtime.InteropServices.ComVisible(true)]
    [System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.AutoDual)]
    public class RtdS1 : ExcelDna.Integration.Rtd.IRtdServer
    {
        Dictionary<int,System.Threading.Timer> lstTimer { set; get; }

        Dictionary<string,IRtdS1Handler> lstHandler { set; get; }

        Dictionary<int,RtdContext> dictContext { set; get; }

        LockedQueue<RtdContext> lstRtdContextToRefresh { set; get; }
        /// <summary>
        /// 每个单元格调用这个RTD的时候
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="strings"></param>
        /// <param name="newValues"></param>
        /// <returns></returns>
        public object ConnectData(int topicId, ref Array arguments, ref bool newValues)
        {
            if (arguments.Length < 2)
                return "必须指定刷新间隔(s)和执行业务逻辑的action，请通过GetActions函数获取所有支持的action";
            string action = arguments.GetValue(1)?.ToString();
            if(string.IsNullOrEmpty(action))
                return "必须指定执行业务逻辑的action，请通过GetActions函数获取所有支持的action";
            if (!this.lstHandler.ContainsKey(action.ToLower()))
                return "不支持指定的action，请通过GetActions函数获取所有支持的action";
            int interval = 0;
            if (!int.TryParse(arguments.GetValue(0)?.ToString(), out interval))
                return "必须指定刷新间隔(s)";
            if (interval <= 0)
                return "刷新间隔必须大于0";
            var context = new RtdContext()
            {
                Action = action,
                Arguments = arguments,
                DeleteFlag = false,
                Interval = interval*1000,
                TopicId = topicId
            };
            this.dictContext.Add(topicId, context);
            context.MyTimer = new System.Threading.Timer(this.BizTimerHandler, context, context.Interval, context.Interval);
            this.lstTimer.Add(topicId, context.MyTimer);
            return "正在处理...";
        }

        private void BizTimerHandler(object state)
        {
            var context = (RtdContext)state;
            context.MyTimer.Change(System.Threading.Timeout.Infinite, context.Interval);
            context.Value = this.lstHandler[context.Action.ToLower()].ProcessRequest(context.Arguments);
            this.lstRtdContextToRefresh.Enqueue(context);
            context.MyTimer.Change(context.Interval, context.Interval);
        }

        public void DisconnectData(int topicID)
        {
            this.dictContext[topicID].DeleteFlag = true;
            this.dictContext.Remove(topicID);
            this.lstTimer[topicID].Dispose();
            this.lstTimer.Remove(topicID);
        }

        public int Heartbeat()
        {
            return 1;
        }

        int onRefreshDataIngFlag = 0;
        public Array RefreshData(ref int topicCount)
        {
            var lstcontext = this.lstRtdContextToRefresh.ToList().Where(x => !x.DeleteFlag).Distinct().ToList();
            var dict = new object[2, lstcontext.Count];
            for (int i = 0; i < lstcontext.Count; i++)
            {
                dict[0, i] = lstcontext[i].TopicId;
                dict[1, i] = lstcontext[i].Value;
            }
            topicCount = lstcontext.Count;
            System.Threading.Interlocked.Exchange(ref onRefreshDataIngFlag, 0);
            return dict;
        }

        public int ServerStart(IRTDUpdateEvent CallbackObject)
        {
            this.lstTimer = new Dictionary<int, System.Threading.Timer>();
            this.lstHandler = System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => typeof(IRtdS1Handler).IsAssignableFrom(x))
                .Where(x => x.FullName != typeof(IRtdS1Handler).FullName)
                .ToDictionary(x => x.Name.ToLower(), x => (IRtdS1Handler)System.Activator.CreateInstance(x));
            this.lstRtdContextToRefresh = new LockedQueue<RtdContext>();
            this.dictContext = new Dictionary<int, RtdContext>();
            System.Threading.ThreadPool.QueueUserWorkItem(this.RefreshRtdContextData, CallbackObject);
            return 1;
        }

        private void RefreshRtdContextData(object state)
        {
            var cc = (IRTDUpdateEvent)state;
            while (true)
            {
                if (this.lstRtdContextToRefresh.Count() < 1)
                {
                    System.Threading.Thread.Sleep(20);
                    continue;
                }
                if (System.Threading.Interlocked.Exchange(ref onRefreshDataIngFlag, 1) != 0)
                {
                    System.Threading.Thread.Sleep(20);
                    continue;
                }
                cc.UpdateNotify();
            }
        }

        public void ServerTerminate()
        {
            foreach (var kv in this.lstTimer)
            {
                kv.Value.Dispose();
            }
        }
    }
}
