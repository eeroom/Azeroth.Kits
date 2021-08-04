using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duban {
    class Program {
      public  static Guid workflowId;

         static HzWorkFlowContext hzwfContext;

        static string workflowName = "请假流程.xaml";

        public static string bookmark;
        static void Main(string[] args) {
            while (true) {
                Console.WriteLine("1新的申请，2审批");
                var cmd = Console.ReadLine();
                if (cmd == "1") {
                    hzwfContext = StartAppliaction();
                }else if (cmd == "2") {
                    Console.WriteLine("y通过，n驳回");
                    var op = Console.ReadLine();
                    ApproveOp(op=="y",workflowId,hzwfContext);
                }
            }
        }

        //第一步以后，后续所有流程步骤的入口
        private static void ApproveOp(bool op, Guid workflowId, HzWorkFlowContext hzwfContext) {
            System.Xaml.XamlXmlReaderSettings st = new System.Xaml.XamlXmlReaderSettings() {
                LocalAssembly = System.Reflection.Assembly.GetExecutingAssembly()
            };
            System.Activities.Activity act = null;
            using (var reader = new System.Xaml.XamlXmlReader(workflowName, st)) {
                act = System.Activities.XamlIntegration.ActivityXamlServices.Load(reader);
            }
            var wfa= new System.Activities.WorkflowApplication(act);
            ConfigWfa(wfa);
            wfa.Load(workflowId);
            wfa.ResumeBookmark(bookmark, op);
            
        }

        private static void ConfigWfa(WorkflowApplication wfa) {
            var cnnstr = "";
            //配置工作流持久化的数据库
            //创建数据库，执行2个脚本C:\Windows\Microsoft.NET\Framework64\v4.0.30319\SQL\zh-Hans\SqlWorkflowInstanceStoreSchema.sql
            //C:\Windows\Microsoft.NET\Framework64\v4.0.30319\SQL\zh-Hans\SqlWorkflowInstanceStoreLogic.sql
            wfa.InstanceStore = new System.Activities.DurableInstancing.SqlWorkflowInstanceStore(cnnstr);

            wfa.Aborted = AbortedHandler;
            wfa.Completed = CompletedHandler;
            wfa.Idle = IdleHandler;
            wfa.OnUnhandledException = OnUnhandledExceptionHandler;
            wfa.PersistableIdle = PersistableIdleHandler;
            wfa.Unloaded = UnloadedHandler;
        }

        private static void UnloadedHandler(WorkflowApplicationEventArgs obj) {
            Console.WriteLine("UnloadedHandler");
        }

        private static PersistableIdleAction PersistableIdleHandler(WorkflowApplicationIdleEventArgs arg) {
            Console.WriteLine("PersistableIdleHandler");
            return PersistableIdleAction.Unload;
        }

        private static UnhandledExceptionAction OnUnhandledExceptionHandler(WorkflowApplicationUnhandledExceptionEventArgs arg) {
            Console.WriteLine("OnUnhandledExceptionHandler");
            return UnhandledExceptionAction.Abort;
        }

        private static void IdleHandler(WorkflowApplicationIdleEventArgs obj) {
            Console.WriteLine("IdleHandler");
        }

        private static void CompletedHandler(WorkflowApplicationCompletedEventArgs obj) {
            Console.WriteLine("CompletedHandler");
        }

        private static void AbortedHandler(WorkflowApplicationAbortedEventArgs obj) {
            Console.WriteLine("AbortedHandler");
        }

        /// <summary>
        /// 第一步，统一入口，各种流程，请假流程，采购流程，调薪流程，
        /// </summary>
        /// <returns></returns>
        private static HzWorkFlowContext StartAppliaction() {
            System.Xaml.XamlXmlReaderSettings st = new System.Xaml.XamlXmlReaderSettings() {
                LocalAssembly = System.Reflection.Assembly.GetExecutingAssembly()
            };
            System.Activities.Activity act = null;
            using (var reader = new System.Xaml.XamlXmlReader(workflowName, st)) {
                act = System.Activities.XamlIntegration.ActivityXamlServices.Load(reader);
            }
            HzWorkFlowContext context = new HzWorkFlowContext();
            context.CreateDateTime = DateTime.Now;
            context.Creator = "eeroom";
            //formdata由页面表单json提交而来，这里只是为了测试
            context.FormData = Newtonsoft.Json.JsonConvert.SerializeObject(new QinJiaFormdata() {
                 Category=3,
                  Days=5
            });
            var dict = new Dictionary<string, object>();
            //对应活动的入参
            dict.Add("hzWorkFlowContext", context);
            var wfa = new System.Activities.WorkflowApplication(act,dict);
            ConfigWfa(wfa);
            wfa.Run();
            return context;
        }
    }
}
