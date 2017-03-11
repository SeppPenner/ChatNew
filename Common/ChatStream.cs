using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Chat
{
    public class ChatStream : Form, IChatStream
    {
        protected ChatStream()
        {
        }

        public ChatStream(NetworkStream stream)
        {
            Stream = stream;
        }


        public NetworkStream Stream { get; set; }

        public string Read()
        {
            return Read(Stream);
        }

        public string Read(NetworkStream n)
        {
            var bytes = new byte[512];
            var totalbytes = 0;
            while (true)
            {
                var i = n.Read(bytes, totalbytes, 2);
                if (i == 2)
                {
                    if (bytes[totalbytes] == 0x03)
                        if (bytes[totalbytes + 1] == 0x00)
                            break;
                }
                else
                {
                    return "";
                }
                totalbytes += i;
            }
            var unicode = new UnicodeEncoding();
            var charCount = unicode.GetCharCount(bytes, 0, totalbytes);
            var chars = new char[charCount];
            unicode.GetChars(bytes, 0, totalbytes, chars, 0);
            var s = new string(chars);
            return s;
        }


        public void Write(string s)
        {
            Write(Stream, s);
        }

        public void Write(NetworkStream n, string s)
        {
            s = s + new string((char) 0x03, 1);
            var unicode = new UnicodeEncoding();
            var bytes = unicode.GetBytes(s);
            n.Write(bytes, 0, bytes.Length);
            n.Flush();
        }

        public byte[] ReadBinary(int numbytes)
        {
            return ReadBinary(Stream, numbytes);
        }

        public byte[] ReadBinary(NetworkStream n, int numbytes)
        {
            var totalbytes = 0;
            var readbytes = new byte[numbytes];
            while (totalbytes < numbytes)
            {
                var i = n.Read(readbytes, totalbytes, numbytes - totalbytes);
                if (i == 0)
                    return null;
                totalbytes += i;
            }
            return readbytes;
        }


        public void WriteBinary(byte[] b)
        {
            WriteBinary(Stream, b);
        }

        public void WriteBinary(NetworkStream n, byte[] b)
        {
            n.Write(b, 0, b.Length);
            n.Flush();
        }
    }
}