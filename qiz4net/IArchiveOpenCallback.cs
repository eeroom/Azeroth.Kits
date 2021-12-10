using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace qiz4net {
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000600100000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IArchiveOpenCallback {
        // ref ulong replaced with IntPtr because handlers ofter pass null value
        // read actual value with Marshal.ReadInt64
        void SetTotal(
            IntPtr files, // [In] ref ulong files, can use 'ulong* files' but it is unsafe
            IntPtr bytes); // [In] ref ulong bytes

        void SetCompleted(
            IntPtr files, // [In] ref ulong files
            IntPtr bytes); // [In] ref ulong bytes
    }
}
