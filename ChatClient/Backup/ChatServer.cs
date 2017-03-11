//*********************************************************************************
//
//  Filename:	ChatServer.cs
//
//	Authors:	Yang Kok Wah
//				Tantrarat Sumalee
//				Valerie Chung Foong Mooi
//				Pang Foong Har
//
//	Date:		28 Feb 2004
//	
//	Summary:	Contains ChatServer class which extends ChatStream and implements
//				IChatServer.
//				Manages Chat Users, Room Users and Client Connections Collection
//				by implementing the IChatServer Interface.
//				Uses base class ChatStream for network communications.
//********************************************************************************


using System;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Chat
{
	public class ChatServer:ChatStream,IChatServer
	{
		//Define the number of users per room
		//Set to 2 for easy testing
		const int DEFAULT_NUM_PER_ROOM=2;

		//Port Number to listen 
		private int port_no;
		//Number of Chat rooms
		private int num_room;
		//number of participants per chat room
		private int num_per_room;

		//One and only connection listener
		private TcpListener listener;

		//Collections to manage users and connections
		private Hashtable connections;
		private Hashtable chatusers;
		private Hashtable[] roomusers;


		//2 parameters Constructor
		public ChatServer(int port_no,int num_room)
		{
			this.port_no =port_no;
			this.num_room=num_room;

		    //init num per room used for room assignment
			num_per_room=DEFAULT_NUM_PER_ROOM;

			//instantiate the connection listener
			listener=new TcpListener(IPAddress.Any,this.port_no);

			//get all the registered users from file
			DeserializeChatUsers("users.bin");

			//init all the rooms
			roomusers=new Hashtable[num_room];
			for(int i=0;i<num_room;i++)
				roomusers[i]=new Hashtable();

			//init connections
			connections=new Hashtable();

			//start listening for connection
			Listener.Start();

			//Loop forever
			//The only way to break is to use clt break key
			while(true)
			{
				Console.WriteLine("Waiting for connection...");
				//get the connected client
				TcpClient client=Listener.AcceptTcpClient();
				//create a new socket helper to manage the connection
				SocketHelper sh=new SocketHelper(this,client);
				Console.WriteLine("Connected");

			}

		}

        [STAThread]
		public static void Main(string[] arg)
		{
			if(arg.Length!=2)
			{
				Console.WriteLine("Usage: ChatServer <port> <num_room>");
				return;
			}
			    
			try
			{
				ChatServer cs=new ChatServer(int.Parse(arg[0]),int.Parse(arg[1]));
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

		}
		#region IChatServer Members

		public int ProposedNumUsersPerRoom
		{
			get
			{
				return num_per_room;
			}
			set
			{
				num_per_room=value;
			}
		}

		public int NumRoom
		{
			get
			{
				return num_room;
			}

			set
			{
				NumRoom=value;
			}

		}

		public int PortNo
		{
			get
			{
				return port_no;
			}

			set
			{
				PortNo=value;
			}
		}

		public TcpListener Listener
		{
			get
			{
				return listener;
			}

			set
			{
				listener=value;
			}

		}


		public Hashtable ClientConnections
		{
			get
			{
				return connections;
			}

			set
			{
				connections=value;
			}
		}

		public Hashtable ChatUsers
		{
			get
			{
				return chatusers;
			}
			set
			{
				chatusers=value;
			}
		}

		public Hashtable[] RoomUsers
		{
			get
			{
				return roomusers;
			}
			set
			{
				roomusers=value;
			}
		}

		public void DeserializeChatUsers(string filename)
		{
			if(File.Exists(filename))
			{
				FileStream f=new FileStream(filename,FileMode.Open);
				IFormatter b=new BinaryFormatter();
				ChatUsers=(Hashtable) b.Deserialize(f);
				f.Close();
			}
			else
			{
				ChatUsers=new Hashtable();
			}		
		}
		
		public void SerializeChatUsers(string filename)
		{
			try
			{
				FileStream f=new FileStream("users.bin",FileMode.Create);
				IFormatter b=new BinaryFormatter();
				b.Serialize(f,ChatUsers);
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}

		}

		public int AssignRoom(string name)
		{
			for(int i=0;i<NumRoom;i++)
				if(RoomUsers[i].Count <ProposedNumUsersPerRoom)
				{
					AddRoomUser(RoomUsers[i],name);
					return i+1;
				}

			AddRoomUser(RoomUsers[0],name);
			return 1;
		}

		public void AddConnection(string index,TcpClient c)
		{
			connections.Add(index.ToUpper(),c);
		}

		public void RemoveConnection(string index,TcpClient c)
		{
			connections.Remove(index.ToUpper());
		}

		public void AddChatUser(string index,ChatUser user)
		{
			ChatUsers.Add(index.ToUpper(),user);
		}

		public void RemoveChatUser(string index)
		{
			ChatUsers.Remove(index.ToUpper());
		}

		public void AddRoomUser(System.Collections.Hashtable room, string index)
		{
			room.Add(index.ToUpper(),index);
		}

		public void RemoveRoomUser(System.Collections.Hashtable room, string index)
		{
			room.Remove(index.ToUpper());
		}

		public void Broadcast(string msg, int room)
		{
			foreach(string s in RoomUsers[room-1].Keys)
			{
				TcpClient t=(TcpClient)connections[s];
				Write(t.GetStream(),msg);
			}
		}

		public void Broadcast(string s)
		{
			for(int i=0;i<NumRoom;i++)
			{
				Broadcast(s,i+1);
			}
		}

		public int FindUserRoom(string index)
		{
			for(int i=0;i<NumRoom;i++)
			 foreach(string  s in RoomUsers[i].Keys)
				 if(s.ToUpper()==index.ToUpper())
					 return (i+1);


			return 0;
		}

		#endregion
	}
}
