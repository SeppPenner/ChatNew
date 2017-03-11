using System;
using Microsoft.Win32;

namespace ChatClient
{
    public static class WinMediaPlayer
    {
        public static string GetMediaPlayerDirectory()
        {
            try
            {
                var localmachineregkey = Registry.LocalMachine;
                var mediaplayerkey = localmachineregkey.OpenSubKey(@"SOFTWARE\Microsoft\MediaPlayer");
                if (mediaplayerkey == null) return "";
                return (string) mediaplayerkey.GetValue("Installation Directory");
            }
            catch
            {
                return "";
            }
        }

        public static void Play(IntPtr hwnd, string strFileName)
        {
            if (!ChatClient.MediaPlayerDirectory.Equals(""))
                Helpers.ShellExecute(hwnd, "open", "wmplayer", "\"" + strFileName + "\"",
                    ChatClient.MediaPlayerDirectory, Helpers.SwNormal);
        }
    }
}