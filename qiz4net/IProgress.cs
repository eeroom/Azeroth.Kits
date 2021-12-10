using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace qiz4net {
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000000050000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IProgress {
        void SetTotal(ulong total);
        void SetCompleted([In] ref ulong completeValue);
    }
}
