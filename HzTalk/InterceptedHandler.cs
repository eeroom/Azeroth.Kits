using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace HzTalk
{
   public class InterceptedHandler : Castle.DynamicProxy.IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var methodName = invocation.MethodInvocationTarget.Name;
            if (methodName.StartsWith("set_"))
            {
                invocation.Proceed();
                var tmp= invocation.InvocationTarget as ModelBase;
                if (tmp == null)
                    return;
                tmp.DispathINotifyPropertyChanged(invocation.Proxy,methodName.Replace("set_", ""));
            }
            else
            {
                invocation.Proceed();
            }
            
        }
    }
}
