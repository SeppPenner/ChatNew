using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Chat
{
    [Flags]
    public enum InputModeFlags
    {
        EnableProcessedInput = 0x01,
        EnableLineInput = 0x02,
        EnableEchoInput = 0x04,
        EnableWindowInput = 0x08,
        EnableMouseInput = 0x10
    }


    public class PasswordConsole
    {
        public static string ReadLine(char passwordchar)
        {
            var s = "";
            char c;
            while ((c = ReadChar()) != '\r')
            {
                s = s + new string(c, 1);
                Console.Write(passwordchar);
            }
            Console.WriteLine();

            return s;
        }

        [DllImport("kernel32.dll", EntryPoint = "SetConsoleMode", SetLastError = true,
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool SetConsoleMode(int hConsoleHandle,
            int dwMode);


        [DllImport("kernel32.dll", EntryPoint = "ReadConsole", SetLastError = true,
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool ReadConsole(int hConsoleInput,
            StringBuilder buf, int nNumberOfCharsToRead, ref int lpNumberOfCharsRead, int lpReserved);

        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true,
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetStdHandle(int nStdHandle);

        private static char ReadChar()
        {
            var stdInputHandle = -10;
            var hConsoleInput = GetStdHandle(stdInputHandle);
            SetConsoleMode(hConsoleInput,
                (int) (InputModeFlags.EnableProcessedInput |
                       InputModeFlags.EnableWindowInput |
                       InputModeFlags.EnableMouseInput));
            var lpNumberOfCharsRead = 0;
            var buf = new StringBuilder(1);
            var success = ReadConsole(hConsoleInput, buf, 1, ref lpNumberOfCharsRead, 0);
            SetConsoleMode(hConsoleInput,
                (int) (InputModeFlags.EnableProcessedInput |
                       InputModeFlags.EnableEchoInput |
                       InputModeFlags.EnableLineInput |
                       InputModeFlags.EnableWindowInput |
                       InputModeFlags.EnableMouseInput));
            if (success)
                return Convert.ToChar(buf[0]);
            throw new ApplicationException("Attempt to call ReadConsole API failed.");
        }
    }
}