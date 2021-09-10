using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Wrapper7z {
    public class WrapperStream7z : ISequentialOutStream, IOutStream, ISequentialInStream, IInStream
    {
        public Stream BaseStream {private set; get; }
        public WrapperStream7z(string path, FileMode mode, FileAccess access)
        {
            this.BaseStream = new FileStream(path, mode, access);
        }

        public WrapperStream7z(Stream stream)
        {
            this.BaseStream = stream;
        }

        public int SetSize(long newSize) {
            this.BaseStream.SetLength(newSize);
            return 0;
        }

        public int Write(byte[] data, uint size, IntPtr processedSize)
        {
            this.BaseStream.Write(data, 0, (int)size);
            Marshal.WriteInt32(processedSize, (int)size);
            return 0;
        }

        public virtual void Seek(long offset, uint seekOrigin, IntPtr newPosition)
        {
            long Position = (uint)this.BaseStream.Seek(offset, (SeekOrigin)seekOrigin);
            if(newPosition!=IntPtr.Zero)
                Marshal.WriteInt64(newPosition, Position);
        }

        public uint Read(byte[] data, uint size)
        {
            return (uint)this.BaseStream.Read(data, 0, (int)size);
        }

        public void Close()
        {
            try
            {
                this.BaseStream.Flush();
                this.BaseStream.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("WrapperStream7z", ex.Message, System.Diagnostics.EventLogEntryType.Error, 7777);
            }
        }
    }
}
