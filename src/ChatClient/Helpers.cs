using System;
using System.Runtime.InteropServices;

namespace ChatClient
{
    internal static class Helpers
    {
        public const int SwMinimize = 6;
        public const int SwNormal = 1;
        public const int SwForceminimize = 11;
        public const int IdcCross = 32515;
        public const int IdcWait = 32514;
        public const int IdcArrow = 32512;
        public const int KeyQueryValue = 0x1;


        [DllImport("shell32.dll")]
        public static extern IntPtr ShellExecute(
            IntPtr hwnd,
            string lpVerb,
            string lpFile,
            string lpParameter,
            string lpDirectory,
            int nShowCmd
        );
    }
}