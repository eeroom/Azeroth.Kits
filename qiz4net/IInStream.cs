using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace qiz4net {
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000300030000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInStream //: ISequentialInStream
    {
        //[PreserveSig]
        //int Read(
        //  [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] data,
        //  uint size,
        //  IntPtr processedSize); // ref uint processedSize

        uint Read(
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] data,
            uint size);

        //[PreserveSig]
        void Seek(
            long offset,
            uint seekOrigin,
            IntPtr newPosition); // ref long newPosition
    }
}
