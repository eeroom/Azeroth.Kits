using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharpcore
{
    interface ProcessHandlerIn<in T>
    {
        int SetTarget(T target);
    }
}
