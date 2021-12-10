using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace qiz4net {
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000300010000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISequentialInStream {
        //[PreserveSig]
        //int Read(
        //  [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] data,
        //  uint size,
        //  IntPtr processedSize); // ref uint processedSize

        uint Read(
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] data,
            uint size);

        /*
    Out: if size != 0, return_value = S_OK and (*processedSize == 0),
      then there are no more bytes in stream.
    if (size > 0) && there are bytes in stream, 
    this function must read at least 1 byte.
    This function is allowed to read less than number of remaining bytes in stream.
    You must call Read function in loop, if you need exact amount of data
    */
    }
}
