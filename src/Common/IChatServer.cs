using System.Collections;
using System.Net.Sockets;

namespace Chat
{
    public interface IChatServer
    {
        Hashtable ChatUsers { get; set; }

        Hashtable ClientConnections { get; set; }

        TcpListener Listener { get; set; }
        int NumRoom { get; set; }
        int PortNo { get; set; }

        int ProposedNumUsersPerRoom { get; set; }

        Hashtable[] RoomUsers { get; set; }

        void SerializeChatUsers(string filename);

        void DeserializeChatUsers(string filename);

        void AddChatUser(string index, ChatUser user);

        void RemoveChatUser(string index);

        void AddRoomUser(Hashtable room, string index);

        void RemoveRoomUser(Hashtable room, string index);

        int FindUserRoom(string index);

        int AssignRoom(string name);

        void AddConnection(string index, TcpClient c);

        void RemoveConnection(string index, TcpClient c);

        void Broadcast(string s, int room);

        void Broadcast(string s);
    }
}