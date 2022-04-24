using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
namespace HzTalk
{
    public class AutofacDataProvider : System.Windows.Data.ObjectDataProvider
    {
        public AutofacDataProvider()
        {
            //unsafe
            //{
            //    var methodPtrO = (long*)typeof(System.Windows.Data.ObjectDataProvider)
            //    .GetMethod("CreateObjectInstance", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            //    .MethodHandle.Value.ToPointer() + 1;
            //    var methodPtrA = (long*)typeof(AutofacDataProvider)
            //       .GetMethod("CreateObjectInstance", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
            //       .MethodHandle.Value.ToPointer() + 1;
            //    *methodPtrO = *methodPtrA;
            //}
        }
        //unsafe static AutofacDataProvider()
        // {
        //     var methodPtrO = (long*)typeof(System.Windows.Data.ObjectDataProvider)
        //         .GetMethod("CreateObjectInstance", System.Reflection.BindingFlags.Instance| System.Reflection.BindingFlags.NonPublic)
        //         .MethodHandle.Value.ToPointer() + 1;
        //     var methodPtrA = (long*)typeof(AutofacDataProvider)
        //        .GetMethod("CreateObjectInstance",System.Reflection.BindingFlags.Instance| System.Reflection.BindingFlags.NonPublic)
        //        .MethodHandle.Value.ToPointer() + 1;
        //     *methodPtrO = *methodPtrA;
        // }

        //public object CreateObjectInstance(out Exception e)
        //{
        //    e = null;
        //    var obj = System.Activator.CreateInstance(this.ObjectType);
        //    return obj;
        //}

        protected override void BeginQuery()
        {
            var obj = App.LifetimeScope.Resolve(this.ObjectType);
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ObjectInstance"));
            this.OnQueryFinished(obj);
        }
    }
}
