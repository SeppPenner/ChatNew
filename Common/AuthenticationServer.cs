using System;
using System.Net.Sockets;

namespace Chat
{
    public delegate bool AuthenticateAction();

    public class AuthenticationServer : IChatUser
    {
        private readonly ChatServer _chatserver;
        private readonly TcpClient _client;
        private AuthenticateAction _action;

        public AuthenticationServer(ChatServer s, TcpClient c)
        {
            _chatserver = s;
            _client = c;
        }

        public string Password { get; set; }

        public string UserId { get; set; }

        public bool Authenticate()
        {
            var validateCommand = false;
            while (!validateCommand)
            {
                _chatserver.Write(_client.GetStream(),
                    AuthenticationProtocolValues.CommandPrompt);
                var data = _chatserver.Read(_client.GetStream());
                data = data.Trim();
                _action = action_default;
                if (data.ToUpper().IndexOf(AuthenticationProtocolValues.LoginCommand, StringComparison.Ordinal) == 0)
                {
                    _action = action_login;
                    validateCommand = true;
                }

                if (data.ToUpper().IndexOf(AuthenticationProtocolValues.RegisterCommand, StringComparison.Ordinal) == 0)
                {
                    _action = action_register;
                    validateCommand = true;
                }
                if (!_action()) return false;
            }
            _chatserver.Write(_client.GetStream(),
                AuthenticationProtocolValues.AutenticatedMsg);

            return true;
        }

        private bool IsRegisteredUser(string userid)
        {
            return _chatserver.ChatUsers[userid.ToUpper()] != null;
        }

        private bool IsUserPassword(string userid, string password)
        {
            if (_chatserver.ChatUsers[userid.ToUpper()] == null) return false;
            return ((ChatUser) _chatserver.ChatUsers[userid.ToUpper()]).Password == password;
        }

        private bool action_default()
        {
            _chatserver.Write(_client.GetStream(),
                AuthenticationProtocolValues.ValidComandsMsg);
            return true;
        }

        private bool action_register()
        {
            UserId = "";
            string temp;
            while (UserId == "")
            {
                _chatserver.Write(_client.GetStream(),
                    AuthenticationProtocolValues.UseridPrompt);
                temp = _chatserver.Read(_client.GetStream());
                if (!ChatUser.ValidUserId(temp))
                {
                    _chatserver.Write(_client.GetStream(),
                        AuthenticationProtocolValues.UseridStartWithWordcharMsg);
                    continue;
                }
                if (IsRegisteredUser(temp))
                    _chatserver.Write(_client.GetStream(),
                        AuthenticationProtocolValues.UserAlreadyRegisteredMsg);
                else
                    UserId = temp;
            }
            Password = "";
            while (Password == "")
            {
                _chatserver.Write(_client.GetStream(),
                    AuthenticationProtocolValues.PasswordPrompt);
                temp = _chatserver.Read(_client.GetStream());
                if (!ChatUser.ValidPassword(temp))
                    _chatserver.Write(_client.GetStream(),
                        AuthenticationProtocolValues.PasswordAtLeast4CharMsg);
                else
                    Password = temp;
            }
            _chatserver.AddChatUser(UserId, new ChatUser(UserId, Password));
            _chatserver.SerializeChatUsers("users.bin");

            return true;
        }

        private bool action_login()
        {
            UserId = "";
            string temp;
            var numtries = 0;
            while (UserId == "")
            {
                _chatserver.Write(_client.GetStream(),
                    AuthenticationProtocolValues.UseridPrompt);
                temp = _chatserver.Read(_client.GetStream());
                if (!IsRegisteredUser(temp))
                {
                    _chatserver.Write(_client.GetStream(),
                        AuthenticationProtocolValues.UserNotRegisteredMsg);
                    if (++numtries >= 3) return false;
                }
                else
                {
                    UserId = temp;
                }
            }
            Password = "";
            numtries = 0;
            while (Password == "")
            {
                _chatserver.Write(_client.GetStream(),
                    AuthenticationProtocolValues.PasswordPrompt);
                temp = _chatserver.Read(_client.GetStream());
                if (!IsUserPassword(UserId, temp))
                {
                    if (++numtries >= 3) return false;
                }
                else
                {
                    Password = temp;
                }
            }
            if (_chatserver.ClientConnections[UserId.ToUpper()] == null) return true;
            _chatserver.Write(_client.GetStream(),
                AuthenticationProtocolValues.UserAlreadyLoggedIn);
            return false;
        }
    }
}