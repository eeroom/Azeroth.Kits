using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenpowerTalk
{
    public class AutofacDataProvider : System.Windows.Data.ObjectDataProvider
    {
       unsafe static AutofacDataProvider()
        {
            var methodPtrO = (long*)typeof(System.Windows.Data.ObjectDataProvider)
                .GetMethod("CreateObjectInstance")
                .MethodHandle.Value.ToPointer() + 1;
            var methodPtrA = (long*)typeof(AutofacDataProvider)
               .GetMethod("CreateObjectInstance")
               .MethodHandle.Value.ToPointer() + 1;
            *methodPtrO = *methodPtrA;
        }

        private object CreateObjectInstance(out Exception e)
        {
            e = null;
            var obj = System.Activator.CreateInstance(this.ObjectType);
            return obj;
        }
    }
}
