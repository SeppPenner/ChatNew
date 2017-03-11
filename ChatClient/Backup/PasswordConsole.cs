//****************************************************************
//	Adapted from Tim Sneath (Microsoft)
//
//	Author: Yang Kok Wah
//
//	Using Win32 API to implement a way to hide inputed characters
//  From the console.
//
//****************************************************************

using System;
using System.Runtime.InteropServices;
using System.Text;	

namespace Chat
{	
	public enum InputModeFlags
	{
		ENABLE_PROCESSED_INPUT = 0x01,
		ENABLE_LINE_INPUT      = 0x02,
		ENABLE_ECHO_INPUT      = 0x04,
		ENABLE_WINDOW_INPUT    = 0x08,
		ENABLE_MOUSE_INPUT     = 0x10
	}
	
	
	public class PasswordConsole{
	
		[DllImport("kernel32.dll", EntryPoint="SetConsoleMode", SetLastError=true,
		 CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		private static extern bool SetConsoleMode(int hConsoleHandle,
		int dwMode);
			
			
		[DllImport("kernel32.dll", EntryPoint="ReadConsole", SetLastError=true,
		 CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		private static extern bool ReadConsole(int hConsoleInput,
		StringBuilder buf, int nNumberOfCharsToRead, ref int lpNumberOfCharsRead, int lpReserved);

		[DllImport("kernel32.dll", EntryPoint="GetStdHandle", SetLastError=true,
		 CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
		private static extern int GetStdHandle(int nStdHandle);
	
		private static char ReadChar()
		{
			// Temporarily disable character echo (ENABLE_ECHO_INPUT) and line input
			// (ENABLE_LINE_INPUT) during this operation
			int  STD_INPUT_HANDLE        = -10;
			int hConsoleInput = GetStdHandle(STD_INPUT_HANDLE);
			SetConsoleMode(hConsoleInput, 
				(int) (InputModeFlags.ENABLE_PROCESSED_INPUT | 
				InputModeFlags.ENABLE_WINDOW_INPUT | 
				InputModeFlags.ENABLE_MOUSE_INPUT));

			int lpNumberOfCharsRead = 0;
			StringBuilder buf = new StringBuilder(1);

			bool success = ReadConsole(hConsoleInput, buf, 1, ref lpNumberOfCharsRead, 0);
			
			// Reenable character echo and line input
			SetConsoleMode(hConsoleInput, 
				(int) (InputModeFlags.ENABLE_PROCESSED_INPUT | 
				InputModeFlags.ENABLE_ECHO_INPUT |
				InputModeFlags.ENABLE_LINE_INPUT |
				InputModeFlags.ENABLE_WINDOW_INPUT | 
				InputModeFlags.ENABLE_MOUSE_INPUT));
			
			if (success)
				return Convert.ToChar(buf[0]);
			else 
				throw new ApplicationException("Attempt to call ReadConsole API failed.");
		}
		
		
		public static string ReadLine(char passwordchar){
			string s="";
			char c;
			while((c=PasswordConsole.ReadChar())!='\r')
			{
				s=s+new string(c,1);
				Console.Write(passwordchar);
			}
			Console.WriteLine();
			
			return s;	
		}	
	}
	
}	