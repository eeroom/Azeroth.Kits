using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelFunctionCOM
{
    /// <summary>
    /// ClassInterfaceType这个特性可以让这个类的public方法直接暴露给com自动化，默认是不会暴露，
    /// 那样只能利用接口暴露，（接口的情况下，vba引用com，然后使用vba的通用模块实现公式）
    /// 使用.net sdk的regasm工具，把程序集包装成com并向注册表添加类guid，然后excel就可以使用这个类的方法作为公式调用
    /// </summary>
    [System.Runtime.InteropServices.Guid("19E151FF-9944-476C-BC47-6C23281A504D")]
    [System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.AutoDual)]
    public class IO {
        #region 因为excel中的com自动化功能，对于标准的com，可能用不了（vba中可以正常引用，但是com自动化不行），所以添加这几个方法
        /// <summary>
        /// 解决在某些机器的Excel提示找不到mscoree.dll的问题
        /// 这里在注册表中将该dll的路径注册进去，当使用regasm注册该类库为com组件
        /// 时会调用该方法
        /// </summary>
        /// <param name="type"></param>
        [System.Runtime.InteropServices.ComRegisterFunction]
        public static void RegisterFunction(Type type) {
            Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(
                GetSubKeyName(type, "Programmable"));
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(
                GetSubKeyName(type, "InprocServer32"), true);
            key.SetValue("", System.Environment.SystemDirectory + @"\mscoree.dll",
                Microsoft.Win32.RegistryValueKind.String);
        }

        [System.Runtime.InteropServices.ComUnregisterFunction]
        public static void UnregisterFunction(Type type) {
            Microsoft.Win32.Registry.ClassesRoot.DeleteSubKey(
                GetSubKeyName(type, "Programmable"), false);
        }

        private static string GetSubKeyName(Type type, string subKeyName) {
            return string.Format("CLSID\\{{{0}}}\\{1}", type.GUID.ToString().ToUpper(), subKeyName);
        }
        #endregion

        public string DateTimeNow() {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

    }
}
