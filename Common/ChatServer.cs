using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ChatClient.Properties;

namespace Chat
{
    public class ChatServer : ChatStream, IChatServer
    {
        private const int DefaultNumPerRoom = 2;
        private readonly int _numRoom;
        private readonly int _portNo;

        private ChatServer(int portNo, int numRoom)
        {
            _portNo = portNo;
            _numRoom = numRoom;
            ProposedNumUsersPerRoom = DefaultNumPerRoom;
            Listener = new TcpListener(IPAddress.Any, _portNo);
            DeserializeChatUsers("users.bin");
            RoomUsers = new Hashtable[numRoom];
            for (var i = 0; i < numRoom; i++)
                RoomUsers[i] = new Hashtable();
            ClientConnections = new Hashtable();
            Listener.Start();
            while (true)
            {
                Console.WriteLine(Resources.WaitingForConnection);
                var client = Listener.AcceptTcpClient();
                new SocketHelper(this, client);
                Console.WriteLine(Resources.Connected);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public Hashtable ChatUsers { get; set; }


        public Hashtable ClientConnections { get; set; }

        public TcpListener Listener { get; set; }

        public int NumRoom
        {
            get { return _numRoom; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
                NumRoom = value;
            }
        }

        public int PortNo
        {
            get { return _portNo; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
                PortNo = value;
            }
        }

        public int ProposedNumUsersPerRoom { get; set; }

        public Hashtable[] RoomUsers { get; set; }

        public void DeserializeChatUsers(string filename)
        {
            if (File.Exists(filename))
            {
                var f = new FileStream(filename, FileMode.Open);
                IFormatter b = new BinaryFormatter();
                ChatUsers = (Hashtable) b.Deserialize(f);
                f.Close();
            }
            else
            {
                ChatUsers = new Hashtable();
            }
        }

        public void SerializeChatUsers(string filename)
        {
            try
            {
                var f = new FileStream("users.bin", FileMode.Create);
                IFormatter b = new BinaryFormatter();
                b.Serialize(f, ChatUsers);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public int AssignRoom(string name)
        {
            for (var i = 0; i < NumRoom; i++)
                if (RoomUsers[i].Count < ProposedNumUsersPerRoom)
                {
                    AddRoomUser(RoomUsers[i], name);
                    return i + 1;
                }

            AddRoomUser(RoomUsers[0], name);
            return 1;
        }

        public void AddConnection(string index, TcpClient c)
        {
            ClientConnections.Add(index.ToUpper(), c);
        }

        public void RemoveConnection(string index, TcpClient c)
        {
            ClientConnections.Remove(index.ToUpper());
        }

        public void AddChatUser(string index, ChatUser user)
        {
            ChatUsers.Add(index.ToUpper(), user);
        }

        public void RemoveChatUser(string index)
        {
            ChatUsers.Remove(index.ToUpper());
        }

        public void AddRoomUser(Hashtable room, string index)
        {
            room.Add(index.ToUpper(), index);
        }

        public void RemoveRoomUser(Hashtable room, string index)
        {
            room.Remove(index.ToUpper());
        }

        public void Broadcast(string msg, int room)
        {
            foreach (string s in RoomUsers[room - 1].Keys)
            {
                var t = (TcpClient) ClientConnections[s];
                Write(t.GetStream(), msg);
            }
        }

        public void Broadcast(string s)
        {
            for (var i = 0; i < NumRoom; i++)
                Broadcast(s, i + 1);
        }

        public int FindUserRoom(string index)
        {
            for (var i = 0; i < NumRoom; i++)
                foreach (string  s in RoomUsers[i].Keys)
                    if (s.ToUpper() == index.ToUpper())
                        return i + 1;


            return 0;
        }

        [STAThread]
        public static void Main(string[] arg)
        {
            if (arg.Length != 2)
            {
                Console.WriteLine(Resources.UsageChatServer);
                return;
            }

            try
            {
                new ChatServer(int.Parse(arg[0]), int.Parse(arg[1]));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}