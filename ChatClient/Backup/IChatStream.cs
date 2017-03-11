//*********************************************************************************
//
//  Filename:	IChatStream.cs

//	Authors:	Yang Kok Wah
//				Tantrarat Sumalee
//				Valerie Chung Foong Mooi
//				Pang Foong Har
//
//	Date:		28 Feb 2004
//	
//	Summary:	Contains IChatStream interface 
//				Define all necessary components for a Chat Network Communication
//				
//********************************************************************************

using System;
using System.Net.Sockets;
using System.Net;

namespace Chat
{
	public interface IChatStream
	{
		//Connected client default network stream
		NetworkStream Stream{get;set;}
		//Read from default stream
		string Read();
		//Read from specified stream
		string Read(NetworkStream n);
		//Write to the default stream
		void Write(string s);
		//Write to a specified stream
		void Write(NetworkStream n,string s);
		//Read numbytes from default stream
		byte[] ReadBinary(int numbytes);
		//Read numbytes from specified stream
		byte[] ReadBinary(NetworkStream n,int numbytes);
		//Write byte array to default stream
		void WriteBinary(byte[] b);
		//Write byte array to specified stream
		void WriteBinary(NetworkStream n,byte[]b);
	}
}
