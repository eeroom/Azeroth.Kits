using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Wrapper7z {
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int CreateObjectDelegate(
        [In] ref Guid classID,
        [In] ref Guid interfaceID,
        //out IntPtr outObject);
        [MarshalAs(UnmanagedType.Interface)] out object outObject);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int GetHandlerPropertyDelegate(
        ArchivePropId propID,
        ref PropVariant value); // PROPVARIANT

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int GetNumberOfFormatsDelegate(out uint numFormats);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int GetHandlerProperty2Delegate(
        uint formatIndex,
        ArchivePropId propID,
        ref PropVariant value); // PROPVARIANT
}
