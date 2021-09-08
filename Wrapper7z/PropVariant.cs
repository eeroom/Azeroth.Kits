using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Wrapper7z {
    [StructLayout(LayoutKind.Explicit)]
    public struct PropVariant {
        [DllImport("ole32.dll")]
        private static extern int PropVariantClear(ref PropVariant pvar);

        [FieldOffset(0)]
        public ushort vt;
        [FieldOffset(8)]
        public IntPtr pointerValue;
        [FieldOffset(8)]
        public byte byteValue;
        [FieldOffset(8)]
        public long longValue;
        [FieldOffset(8)]
        public System.Runtime.InteropServices.ComTypes.FILETIME filetime;
        [FieldOffset(8)]
        public PropArray propArray;

        public VarEnum VarType {
            get {
                return (VarEnum)this.vt;
            }
        }

        public void Clear() {
            switch (this.VarType) {
                case VarEnum.VT_EMPTY:
                    break;

                case VarEnum.VT_NULL:
                case VarEnum.VT_I2:
                case VarEnum.VT_I4:
                case VarEnum.VT_R4:
                case VarEnum.VT_R8:
                case VarEnum.VT_CY:
                case VarEnum.VT_DATE:
                case VarEnum.VT_ERROR:
                case VarEnum.VT_BOOL:
                //case VarEnum.VT_DECIMAL:
                case VarEnum.VT_I1:
                case VarEnum.VT_UI1:
                case VarEnum.VT_UI2:
                case VarEnum.VT_UI4:
                case VarEnum.VT_I8:
                case VarEnum.VT_UI8:
                case VarEnum.VT_INT:
                case VarEnum.VT_UINT:
                case VarEnum.VT_HRESULT:
                case VarEnum.VT_FILETIME:
                    this.vt = 0;
                    break;

                default:
                    PropVariantClear(ref this);
                    break;
            }
        }

        public object GetObject() {
            switch (this.VarType) {
                case VarEnum.VT_EMPTY:
                    return null;

                case VarEnum.VT_FILETIME:
                    return DateTime.FromFileTime(this.longValue);

                default:
                    GCHandle PropHandle = GCHandle.Alloc(this, GCHandleType.Pinned);

                    try {
                        return Marshal.GetObjectForNativeVariant(PropHandle.AddrOfPinnedObject());
                    } finally {
                        PropHandle.Free();
                    }
            }
        }
    }

}
