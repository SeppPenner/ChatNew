using System.Net.Sockets;

namespace Chat
{
    public interface IChatStream
    {
        NetworkStream Stream { get; set; }

        string Read();

        string Read(NetworkStream n);

        void Write(string s);

        void Write(NetworkStream n, string s);

        byte[] ReadBinary(int numbytes);

        byte[] ReadBinary(NetworkStream n, int numbytes);

        void WriteBinary(byte[] b);

        void WriteBinary(NetworkStream n, byte[] b);
    }
}