using System;
using System.Text.RegularExpressions;

namespace Chat
{
    [Serializable]
    public class ChatUser : IChatUser
    {
        public ChatUser(string userid, string password)
        {
            UserId = userid;
            Password = password;
        }

        public string Password { get; set; }

        public string UserId { get; set; }

        public static bool ValidUserId(string s)
        {
            if (s.ToUpper().IndexOf("SERVER", StringComparison.Ordinal) == 0) return false;
            var m = Regex.Match(s, @"\s+");
            if (m.Success) return false;
            m = Regex.Match(s, @"^\w+");
            return m.Success;
        }

        public static bool ValidPassword(string s)
        {
            var m = Regex.Match(s, @".{4,}");
            return m.Success;
        }
    }
}