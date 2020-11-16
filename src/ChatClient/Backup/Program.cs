using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Chat
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] arg)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ChatClient c = null;

            try
            {
                if ((arg.Length < 1) || (arg.Length > 2))
                {
                    //Console.WriteLine ("Usage: ChatClient <host> <port>");
                    MessageBox.Show("Usage: ChatClient <host> <port>");
                    return;
                }

                if (arg.Length == 1)
                    c = new ChatClient(arg[0]);

                if (arg.Length == 2)
                    c = new ChatClient(arg[0], int.Parse(arg[1]));

                Application.Run(c);

            }
            catch//Exception e)
            {
                //Console.WriteLine(e);
            }


        }
    }
}
