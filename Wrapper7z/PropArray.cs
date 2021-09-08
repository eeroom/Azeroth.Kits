using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Wrapper7z {
    [StructLayout(LayoutKind.Sequential)]
    public struct PropArray {
        uint length;
        IntPtr pointerValues;
    }
}
