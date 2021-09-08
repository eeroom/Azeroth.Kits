using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Wrapper7z {
    public class InStreamWrapper : StreamWrapper, ISequentialInStream, IInStream {
        public InStreamWrapper(Stream baseStream) : base(baseStream) {
        }

        public uint Read(byte[] data, uint size) {
            return (uint)this.BaseStream.Read(data, 0, (int)size);
        }
    }
}
