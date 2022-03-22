using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VclibInvoke
{
    public delegate int TransferCallback(int v1, int v2);

    public class Program
    {
        [System.Runtime.InteropServices.DllImport("vclib.dll")]
        public static extern int Add(int v1, int v2);

        //P/Invoke中不能使用泛型的委托，
        //所以定义一个与原方法中callback函数指针方法签名一致的委托TransferCallback
        [System.Runtime.InteropServices.DllImport("vclib.dll")]
        public static extern int Handler(IntPtr str, TransferCallback callback);

        static void Main(string[] args)
        {
            int rst = Add(1, 2);
            IntPtr str = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi("hello world");
            int rst2 = Handler(str, TransferHandler);
            Console.ReadKey();
        }

        private static int TransferHandler(int v1, int v2) {
            return v1 * 2 + v2 * 3;
        }
    }
}
