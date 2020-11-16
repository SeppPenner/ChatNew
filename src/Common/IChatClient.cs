using System.Net.Sockets;

namespace Chat
{
    public interface IChatClient
    {
        TcpClient Connect(string address, int port);
        bool Authenticate();
        void Listen();
    }
}