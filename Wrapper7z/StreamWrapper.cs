using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Wrapper7z {
    public class StreamWrapper : IDisposable {
        protected Stream BaseStream;

        protected StreamWrapper(Stream baseStream) {
            this.BaseStream = baseStream;
        }

        public void Dispose() {
            this.BaseStream.Close();
        }

        public virtual void Seek(long offset, uint seekOrigin, IntPtr newPosition) {
            long Position = (uint)this.BaseStream.Seek(offset, (SeekOrigin)seekOrigin);

            if (newPosition != IntPtr.Zero) {
                Marshal.WriteInt64(newPosition, Position);
            }
        }
    }
}
