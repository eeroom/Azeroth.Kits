using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharpcore
{
    interface ProcessHandlerOut<out T>
    {
        T Get(int index);
    }
}
