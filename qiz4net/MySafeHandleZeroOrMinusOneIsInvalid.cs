using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace qiz4net {
   public class MySafeHandleZeroOrMinusOneIsInvalid : SafeHandleZeroOrMinusOneIsInvalid {
        public MySafeHandleZeroOrMinusOneIsInvalid() : base(true)
        {
        }

        /// <summary>Release library handle</summary>
        /// <returns>true if the handle was released</returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle() {
            return Kernel32Dll.FreeLibrary(this.handle);
        }
    }
}
