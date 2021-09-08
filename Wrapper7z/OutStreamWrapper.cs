using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Wrapper7z {
    public class OutStreamWrapper : StreamWrapper, ISequentialOutStream, IOutStream {
        public OutStreamWrapper(Stream baseStream) : base(baseStream) {
        }

        public int SetSize(long newSize) {
            this.BaseStream.SetLength(newSize);
            return 0;
        }

        public int Write(byte[] data, uint size, IntPtr processedSize) {
            this.BaseStream.Write(data, 0, (int)size);

            if (processedSize != IntPtr.Zero) {
                Marshal.WriteInt32(processedSize, (int)size);
            }

            return 0;
        }
    }
}
