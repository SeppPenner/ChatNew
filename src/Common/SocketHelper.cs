using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ChatClient.Properties;

namespace Chat
{
    public delegate void SocketHelperAction();

    public class SocketHelper
    {
        private readonly ChatServer _chatserver;
        private readonly TcpClient _client;
        private SocketHelperAction _action;
        private string _readdata = "";

        public SocketHelper(ChatServer s, TcpClient c)
        {
            _client = c;
            _chatserver = s;
            var t = new Thread(HandleClient);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private string Nickname { get; set; } = "";

        private int Room { get; set; }

        private void action_default()
        {
            _chatserver.Write(_client.GetStream(),
                ChatProtocolValues.UNKNOWN_CMD_MSG(_readdata));
        }

        private void action_send_message()
        {
            _chatserver.Broadcast(Nickname + "> " + _readdata, Room);
        }

        private void action_private_message()
        {
            var s = _readdata.Split(':');
            var name = "null_name";
            var temp = "";
            if (s.Length >= 3) name = s[2].Trim();
            if (s.Length >= 4) temp = s[3].Trim();
            TcpClient t = null;
            if (_chatserver.FindUserRoom(name) != 0)
                t = (TcpClient) _chatserver.ClientConnections[name.ToUpper()];
            if (t != null)
            {
                _chatserver.Write(t.GetStream(),
                    ChatProtocolValues.NORMAL_MSG(Nickname, temp));
                _chatserver.Write(_client.GetStream(),
                    ChatProtocolValues.NORMAL_MSG(Nickname, temp));
            }
            else
            {
                _chatserver.Write(_client.GetStream(),
                    ChatProtocolValues.USER_NOT_FOUND_MSG(name));
            }
        }

        private void action_list()
        {
            var s = _readdata.Split(' ');
            var p1 = 0;
            if (s.Length == 1) p1 = Room;
            if (s.Length >= 2)
                if (s[1].ToUpper() == "ALL")
                    p1 = -1;
                else
                    try
                    {
                        p1 = int.Parse(s[1]);
                    }
                    catch
                    {
                        // ignored
                    }

            if (p1 > _chatserver.NumRoom || p1 == 0)
            {
                _chatserver.Write(_client.GetStream(),
                    ChatProtocolValues.NoSuchRoomMsg);
            }
            else
            {
                var temp = "";
                if (p1 == -1)
                    for (var i = 0; i < _chatserver.NumRoom; i++)
                        foreach (string s1 in _chatserver.RoomUsers[i].Values)
                            temp = temp + "\n   " + s1 + " : room " + (i + 1);
                else
                    foreach (string s1 in _chatserver.RoomUsers[p1 - 1].Values)
                        temp = temp + "\n   " + s1 + " : room " + p1;

                if (temp == "") temp = "Empty";
                _chatserver.Write(_client.GetStream(),
                    ChatProtocolValues.NORMAL_MSG("server", temp));
            }
        }

        private void action_which_room()
        {
            _chatserver.Write(_client.GetStream(),
                ChatProtocolValues.YOUR_ROOM_NO_MSG(Room));
        }

        private void action_quit()
        {
            _chatserver.Write(_client.GetStream(),
                ChatProtocolValues.QuitMsg);
        }

        private void action_change_room()
        {
            var oldroom = Room;
            _chatserver.RemoveRoomUser(_chatserver.RoomUsers[oldroom - 1], Nickname);
            Room = 0;
            while (Room == 0)
            {
                _chatserver.Write(_client.GetStream(),
                    ChatProtocolValues.CHOOSE_ROOM(Nickname, _chatserver.NumRoom));
                var temp = _chatserver.Read(_client.GetStream());
                try
                {
                    var rN = int.Parse(temp);
                    if (rN >= 1 && rN <= _chatserver.NumRoom)
                        Room = rN;
                }
                catch
                {
                    // ignored
                }
            }
            _chatserver.AddRoomUser(_chatserver.RoomUsers[Room - 1], Nickname);
            _chatserver.Broadcast(ChatProtocolValues.MOVE_TO(Nickname, Room), oldroom);
            _chatserver.Broadcast(ChatProtocolValues.Welcome(Nickname, Room), Room);
        }

        private void action_help()
        {
            _chatserver.Write(_client.GetStream(),
                "server>\n" +
                "  :help\n" +
                "  :change room\n" +
                "  :list\n" +
                "  :list all\n" +
                "  :list <room_no>\n" +
                "  :which room\n" +
                "  :private:<target>:<message>\n" +
                "  :send pic:<target>\n" +
                "  :get pic:<sender>\n" +
                "  :send media:<target>\n" +
                "  :get media:<sender>\n" +
                "  :quit");
        }

        private void action_send_media()
        {
            var s = _readdata.Split(':');
            var name = "";
            if (s.Length == 3) name = s[2];
            TcpClient t = null;
            if (_chatserver.FindUserRoom(name) != 0)
                t = (TcpClient) _chatserver.ClientConnections[name.ToUpper()];
            if (t != null)
            {
                _chatserver.Write(_client.GetStream(), ChatProtocolValues.SendMediaMsg);
                var ext = _chatserver.Read(_client.GetStream());
                var snumbytes = _chatserver.Read(_client.GetStream());
                var numbytes = int.Parse(snumbytes);
                if (numbytes == 0)
                {
                    _chatserver.Write(_client.GetStream(),
                        "server> No media file to send");
                    return;
                }
                if (numbytes > 51200000)
                {
                    _chatserver.Write(_client.GetStream(),
                        "server> Media File is larger than 5 MB");
                    return;
                }
                var b = _chatserver.ReadBinary(_client.GetStream(), numbytes);
                if (b == null)
                {
                    _chatserver.Write(_client.GetStream(),
                        "server> Transmission Error");
                    return;
                }
                var fext = new FileStream(Nickname + "_" + name, FileMode.Create);
                var unicode = new UnicodeEncoding();
                var bytes = unicode.GetBytes(ext);
                fext.Write(bytes, 0, bytes.Length);
                fext.Close();
                var f = new FileStream(Nickname + "_" + name + "." + ext, FileMode.Create);
                f.Write(b, 0, b.Length);
                f.Close();
                _chatserver.Write(t.GetStream(),
                    ChatProtocolValues.MEDIA_FROM_MSG(Nickname, name));
                _chatserver.Write(_client.GetStream(),
                    ChatProtocolValues.MEDIA_SEND_MSG(Nickname));
            }
            else
            {
                _chatserver.Write(_client.GetStream(),
                    ChatProtocolValues.USER_NOT_FOUND_MSG(name));
            }
        }

        private void action_get_media()
        {
            var s = _readdata.Split(':');
            var sender = "";
            var ext = "";
            if (s.Length >= 3) sender = s[2];
            if (s.Length >= 4) ext = s[3];
            if (File.Exists(sender + "_" + Nickname))
            {
                var f = new FileStream(sender + "_" + Nickname, FileMode.Open);
                var fi = new FileInfo(sender + "_" + Nickname);
                var b = new byte[fi.Length];
                f.Read(b, 0, b.Length);
                f.Close();
                var unicode = new UnicodeEncoding();
                var charCount = unicode.GetCharCount(b, 0, b.Length);
                var chars = new char[charCount];
                unicode.GetChars(b, 0, b.Length, chars, 0);
                ext = new string(chars);
            }
            var medianame = sender + "_" + Nickname + "." + ext;
            if (!File.Exists(medianame))
            {
                _chatserver.Write(_client.GetStream(),
                    ChatProtocolValues.MEDIA_NOT_FOUND_MSG(medianame));
            }
            else
            {
                var f = new FileStream(medianame, FileMode.Open);
                var fi = new FileInfo(medianame);
                var b = new byte[fi.Length];
                f.Read(b, 0, b.Length);
                f.Close();
                _chatserver.Write(_client.GetStream(),
                    ChatProtocolValues.GetMediaMsg);
                _chatserver.Write(_client.GetStream(), ext);
                _chatserver.Write(_client.GetStream(), "" + b.Length);
                _chatserver.WriteBinary(_client.GetStream(), b);
                _chatserver.Write(_client.GetStream(),
                    ChatProtocolValues.MediaSendAckMsg);
                TcpClient t = null;
                if (_chatserver.FindUserRoom(sender) != 0)
                    t = (TcpClient) _chatserver.ClientConnections[sender.ToUpper()];
                if (t != null)
                    _chatserver.Write(t.GetStream(),
                        ChatProtocolValues.GOTTEN_MEDIA_MSG(Nickname));
            }
        }


        private void action_send_pic()
        {
            var s = _readdata.Split(':');
            var name = "";
            if (s.Length == 3) name = s[2];
            TcpClient t = null;
            if (_chatserver.FindUserRoom(name) != 0)
                t = (TcpClient) _chatserver.ClientConnections[name.ToUpper()];
            if (t != null)
            {
                _chatserver.Write(_client.GetStream(), ChatProtocolValues.SendPicMsg);
                var snumbytes = _chatserver.Read(_client.GetStream());
                var numbytes = int.Parse(snumbytes);
                var b = _chatserver.ReadBinary(_client.GetStream(), numbytes);
                if (b == null)
                {
                    _chatserver.Write(_client.GetStream(),
                        "server> Transmission Error");
                    return;
                }
                var f = new FileStream(Nickname + "_" + name + ".jpg", FileMode.Create);
                f.Write(b, 0, b.Length);
                f.Close();
                _chatserver.Write(t.GetStream(),
                    ChatProtocolValues.PIC_FROM_MSG(Nickname, name));
                _chatserver.Write(_client.GetStream(),
                    ChatProtocolValues.PIC_SEND_MSG(Nickname));
            }
            else
            {
                _chatserver.Write(_client.GetStream(),
                    ChatProtocolValues.USER_NOT_FOUND_MSG(name));
            }
        }

        private void action_get_pic()
        {
            var s = _readdata.Split(':');
            var sender = "";
            if (s.Length == 3) sender = s[2];
            var picname = sender + "_" + Nickname + ".jpg";
            if (!File.Exists(picname))
            {
                _chatserver.Write(_client.GetStream(),
                    ChatProtocolValues.PIC_NOT_FOUND_MSG(picname));
            }
            else
            {
                var f = new FileStream(picname, FileMode.Open);
                var fi = new FileInfo(picname);
                var b = new byte[fi.Length];
                f.Read(b, 0, b.Length);
                f.Close();
                _chatserver.Write(_client.GetStream(),
                    ChatProtocolValues.GetPicMsg);
                _chatserver.Write(_client.GetStream(), "" + b.Length);
                _chatserver.WriteBinary(_client.GetStream(), b);
                _chatserver.Write(_client.GetStream(),
                    ChatProtocolValues.PicSendAckMsg);
                TcpClient t = null;
                if (_chatserver.FindUserRoom(sender) != 0)
                    t = (TcpClient) _chatserver.ClientConnections[sender.ToUpper()];
                if (t != null)
                    _chatserver.Write(t.GetStream(),
                        ChatProtocolValues.GOTTEN_PIC_MSG(Nickname));
            }
        }

        private void HandleClient()
        {
            try
            {
                var auth = new AuthenticationServer(_chatserver, _client);
                if (!auth.Authenticate())
                    throw new Exception();
                _chatserver.Write(_client.GetStream(),
                    ChatProtocolValues.CONNECTION_MSG(auth.UserId));
                Nickname = auth.UserId;
                Room = _chatserver.AssignRoom(Nickname);
                Console.WriteLine(Resources.RoomAssigned + Room + Resources.For + Nickname);
                try
                {
                    _chatserver.AddConnection(Nickname, _client);
                }
                catch
                {
                    // ignored
                }
                _chatserver.Broadcast(ChatProtocolValues.Welcome(Nickname, Room), Room);
                while ((_readdata = _chatserver.Read(_client.GetStream())) != "")
                {
                    _readdata = _readdata.Trim();
                    Console.WriteLine(Resources.Read + _readdata);
                    if (_readdata.ToUpper().Substring(0, 1) == ChatProtocolValues.IsCmd)
                    {
                        _action = action_default;
                        if (
                            (_readdata.ToUpper() + ":").IndexOf(ChatProtocolValues.GetPicCmd, StringComparison.Ordinal) ==
                            0)
                            _action = action_get_pic;
                        if (
                            (_readdata.ToUpper() + ":").IndexOf(ChatProtocolValues.SendPicCmd, StringComparison.Ordinal) ==
                            0)
                            _action = action_send_pic;
                        if (
                            (_readdata.ToUpper() + ":").IndexOf(ChatProtocolValues.GetMediaCmd, StringComparison.Ordinal) ==
                            0)
                            _action = action_get_media;
                        if (
                            (_readdata.ToUpper() + ":").IndexOf(ChatProtocolValues.SendMediaCmd,
                                StringComparison.Ordinal) ==
                            0)
                            _action = action_send_media;
                        if ((_readdata.ToUpper() + " ").IndexOf(ChatProtocolValues.HelpCmd, StringComparison.Ordinal) ==
                            0)
                            _action = action_help;
                        if ((_readdata.ToUpper() + " ").IndexOf(ChatProtocolValues.QuitCmd, StringComparison.Ordinal) ==
                            0)
                            _action = action_quit;
                        if (
                            (_readdata.ToUpper() + " ").IndexOf(ChatProtocolValues.ChangeRoomCmd,
                                StringComparison.Ordinal) == 0)
                            _action = action_change_room;
                        if (
                            (_readdata.ToUpper() + " ").IndexOf(ChatProtocolValues.WhichRoomCmd,
                                StringComparison.Ordinal) ==
                            0)
                            _action = action_which_room;
                        if ((_readdata.ToUpper() + " ").IndexOf(ChatProtocolValues.ListCmd, StringComparison.Ordinal) ==
                            0)
                            _action = action_list;
                        if (
                            (_readdata.ToUpper() + ":").IndexOf(ChatProtocolValues.PrivateMsgCmd,
                                StringComparison.Ordinal) == 0)
                            _action = action_private_message;
                    }
                    else
                    {
                        _action = action_send_message;
                    }
                    _action();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(Resources.ErrorFromServer);
                Console.WriteLine(Resources.Stars);
                Console.WriteLine(e);
                Console.WriteLine(Resources.Stars);
                Console.WriteLine(Resources.WaitingForConnection);
            }
            finally
            {
                try
                {
                    _chatserver.Write(_client.GetStream(), ChatProtocolValues.QuitMsg);
                }
                catch
                {
                    // ignored
                }
                if (Room != 0 && Nickname != "")
                {
                    _chatserver.RemoveRoomUser(_chatserver.RoomUsers[Room - 1], Nickname);
                    _chatserver.Broadcast(ChatProtocolValues.USER_LOG_OUT(Nickname, Room));
                }
                try
                {
                    _chatserver.RemoveConnection(Nickname, _client);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}