//*********************************************************************************
//
//  Filename:	IChatServer.cs
//
//	Authors:	Yang Kok Wah
//				Tantrarat Sumalee
//				Valerie Chung Foong Mooi
//				Pang Foong Har
//
//	Date:		28 Feb 2004
//	
//	Summary:	Contains IChatServer interface 
//				Define all necessary components for a Chat Server
//				
//********************************************************************************

using System;
using System.Collections;
using System.Net.Sockets;
using System.Net;

namespace Chat
{
	public interface IChatServer
	{

		//Number of chat rooms
		int NumRoom{get;set;}
		//Number of person per room
		int ProposedNumUsersPerRoom{get;set;}
		//Connection listener
		TcpListener Listener{get;set;}
		//Port number to listen to
		int PortNo{get;set;}
		//Collection of registered chat users
		Hashtable ChatUsers{get;set;}
		//Array of Collections of participants in the room
		//One collection for each room
		Hashtable[] RoomUsers{get;set;}
		//Collection of Tcp Client Connections
		Hashtable ClientConnections{get;set;}

		//User Management
		//Serialize Chat Users to a file
		void SerializeChatUsers(string filename);
		//Deserialize Chat Users from a file
		void DeserializeChatUsers(string filename);
		//Add new Chat User
		void AddChatUser(string index,ChatUser user);
		//Remove a Chat User
		void RemoveChatUser(string index);

		//Room User Management
		//Add a participant to chat room
		void AddRoomUser(Hashtable room,string index);
		//Remove a participant from the chat room 
		void RemoveRoomUser(Hashtable room,string index);
		//Find the chat room for a participant returning chat room number
		int FindUserRoom(string index);
		//Assign a participant to a room returning the chat room number
		//based on ProposedNumUsersPerRoom
		int AssignRoom(string name);

		//Connections
		//Add client connecion to the collection
		void AddConnection(string index,TcpClient c);
		//Remove client connection to the collection
		void RemoveConnection(string index,TcpClient c);

		//Communication
		//Broadcast to one room
		void Broadcast(string s,int room);

		//Broadcast to all
		void Broadcast(string s);

	}
}