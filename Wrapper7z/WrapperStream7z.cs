using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Wrapper7z {
    public class WrapperStream7z : ISequentialOutStream, IOutStream, ISequentialInStream, IInStream
    {
        Stream targetStream { set; get; }
        public WrapperStream7z(string path, FileMode mode, FileAccess access)
        {
            this.targetStream = new FileStream(path, mode, access);
        }

        public WrapperStream7z(Stream stream)
        {
            this.targetStream = stream;
        }

        public int SetSize(long newSize) {
            this.targetStream.SetLength(newSize);
            return 0;
        }

        public int Write(byte[] data, uint size, IntPtr processedSize) {
            this.targetStream.Write(data, 0, (int)size);
            Marshal.WriteInt32(processedSize, (int)size);
            return 0;
        }

        public virtual void Seek(long offset, uint seekOrigin, IntPtr newPosition)
        {
            long Position = (uint)this.targetStream.Seek(offset, (SeekOrigin)seekOrigin);
            Marshal.WriteInt64(newPosition, Position);
        }

        public uint Read(byte[] data, uint size)
        {
            return (uint)this.targetStream.Read(data, 0, (int)size);
        }
    }
}
