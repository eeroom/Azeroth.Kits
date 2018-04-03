using System;
using System.Runtime.InteropServices;//调用操作系统动态链接库
using System.Diagnostics;
namespace Excel.Extension
{
    public enum MouseButtonAction
    {
        WM_MOUSEMOVE = 0x200,
        WM_LBUTTONDOWN = 0x201,
        WM_LBUTTONUP = 0x202,
        WM_LBUTTONDBLCLK = 0x203,

        WM_RBUTTONDOWN = 0x204,
        WM_RBUTTONUP = 0x205,
        WM_RBUTTONDBLCLK = 0x206,

        WM_MBUTTONDOWN = 0x207,
        WM_MBUTTONUP = 0x208,
        WM_MBUTTONDBLCLK = 0x209,

        WM_Wheel = 0x20A,
    }
    public delegate int HookCallBack(int nCode, IntPtr wParam, IntPtr lParam);

    public enum HookLevel
    {
        /// <summary>
        /// low level mouse event
        /// </summary>
        WH_MOUSE_LL = 14,
        /// <summary>
        /// normal level mouse event
        /// </summary>
        WH_MOUSE = 7
    }
    public  class MouseHookWrapper
    {
        const string Dlluser32 = "user32.dll";
        const string Dllkernel32 = "kernel32.dll";

        HookLevel hookLevel;
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class LParamStruct
        {
            public POINT pt;
            public int hWnd;
            public int wHitTestCode;
            public int dwExtraInfo;
            //public IntPtr WParma;
            //public IntPtr LParma;
            //public int msg;
            //public IntPtr hWnd;
        }

        [DllImport(Dlluser32, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(HookLevel level, HookCallBack callback, IntPtr hInstance, int threadId);

        [DllImport(Dllkernel32, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetLastError();

        [DllImport(Dllkernel32, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport(Dllkernel32)]
        private static extern int GetCurrentThreadId();

        [DllImport(Dlluser32, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport(Dlluser32, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int hookId, int nCode, IntPtr wParam, IntPtr lParam);



        Action<int, IntPtr, IntPtr> handler;
        public void Hook(Action<int,IntPtr,IntPtr> handler,HookLevel me)
        {
            if (this.hookLevel != 0)
                return;
            this.handler = handler;
            IntPtr modulePtr = GetModuleHandle(Dlluser32);
            hookLevel = (HookLevel)SetWindowsHookEx(me,HookCallBackWrapper, modulePtr, GetCurrentThreadId());
            if (hookLevel == 0)
                throw new ArgumentException("hook失败");
        }

        private int HookCallBackWrapper(int nCode, IntPtr wParam, IntPtr lParam)
        {
            this.handler(nCode,wParam,lParam);
            return CallNextHookEx((int)this.hookLevel,nCode,wParam,lParam);
        }

        public void UnHook()
        {
            UnhookWindowsHookEx((int)hookLevel);
            this.hookLevel = 0;
        }

    }
}
