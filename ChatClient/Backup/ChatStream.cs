//*********************************************************************************
//
//  Filename:	ChatStream.cs
//
//	Authors:	Yang Kok Wah
//				Tantrarat Sumalee
//				Valerie Chung Foong Mooi
//				Pang Foong Har
//
//	Date:		28 Feb 2004
//	
//	Summary:	Contains ChatStream Class
//				For network communications.
//
//********************************************************************************

using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.Windows.Forms;
namespace Chat
{

	public class ChatStream:Form,IChatStream 
	{
		private NetworkStream stream;

		//Default Constructor. Network stream not assigned
		public ChatStream()
		{

		}

		//1 parameter Constructor to assign default network stream
		public ChatStream(NetworkStream stream)
		{
			this.stream=stream;
		}

		#region IChatStream Members

		public System.Net.Sockets.NetworkStream Stream
		{
			get
			{
				return stream;
			}
			set
			{
				stream=value;
			}
		}

		public string Read()
		{
			return Read(stream);
		}

		public string Read(NetworkStream n)
		{
			//maximum buffer size is 256 bytes
			byte[] bytes=new byte[512];
			//total number of bytes read
			int totalbytes=0;
			while(true){
				//reading 1 byte at a time
			    int i=n.Read(bytes,totalbytes,2);
				if(i==2)
				{
					//the special end byte if found
					if(bytes[totalbytes]==(byte)0x03)
					  if(bytes[totalbytes+1]==(byte)0x00)
						break;
				}
				else
				{
					//end of stream
					return "";
				}
               //advance to the next position to store read byte(s)
			   totalbytes +=i;			     
			}
			//convert the bytes to a string and return
			UnicodeEncoding Unicode = new UnicodeEncoding();
        	int charCount = Unicode.GetCharCount(bytes, 0, totalbytes);
        	char[] chars = new Char[charCount];
        	Unicode.GetChars(bytes, 0, totalbytes, chars, 0);
			string s=new string(chars);
			return s;
			//System.Text.Encoding.ASCII.GetString(bytes,0,totalbytes);

		}


		public void Write(string s)
		{
			Write(stream,s);

		}

		public void Write(NetworkStream n,string s)
		{	
			//Append char 0x03 to end of text string
			s=s+new string((char)0x03,1);
			//byte[] bytes=System.Text.Encoding.ASCII.GetBytes(s);
			UnicodeEncoding Unicode = new UnicodeEncoding();
			byte[] bytes=Unicode.GetBytes(s);
			n.Write(bytes,0,bytes.Length );
			n.Flush();
		}

		public byte[] ReadBinary(int numbytes)
		{
			return ReadBinary(stream,numbytes);
		}

		public byte[] ReadBinary(NetworkStream n,int numbytes)
		{
			//total bytes read
			int totalbytes=0;

            byte[] readbytes=new byte[numbytes];

			while(totalbytes<numbytes)
			{
				//read as much as possible
				int i=n.Read(readbytes,totalbytes,numbytes-totalbytes);

				//End of stream
				if(i==0)
					return null;

				//advence to the next position to store read byte(s)
				totalbytes +=i;
			}
			return readbytes;
		}


		public void WriteBinary(byte[] b)
		{
			WriteBinary(stream,b);
		}

		public void WriteBinary(NetworkStream n,byte[] b)
		{
			n.Write(b,0,b.Length);
			n.Flush();
		}
		#endregion
	}
}
