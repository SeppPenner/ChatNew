//*********************************************************************************
//
//  Filename:	IChatClient.cs
//
//	Authors:	Yang Kok Wah
//				Tantrarat Sumalee
//				Valerie Chung Foong Mooi
//				Pang Foong Har
//
//	Date:		28 Feb 2004
//	
//	Summary:	Contains IChatClient Interface
//				Define necessary components for a Chat Client
//********************************************************************************

using System;
using System.Net.Sockets;
using System.Net;

namespace Chat
{

	public interface IChatClient
	{
		//Connect method
		TcpClient Connect(string address,int port);
		//Authentication
		bool Authenticate();
		//Listen for incoming response from server
		void Listen();
		
	}
}
