//*********************************************************************************
//
//  Filename:	SocketHelper.cs
//
//	Authors:	Yang Kok Wah
//				Tantrarat Sumalee
//				Valerie Chung Foong Mooi
//				Pang Foong Har
//
//	Date:		28 Feb 2004
//	
//	Summary:	Contains SocketHelper class and SocketHelperAction delegate.
//				Authenticate the user.
//				Handle all client chat messages and commands.
//				
//********************************************************************************

using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Text;

namespace Chat
{

	//Delegate for actions
	public delegate void SocketHelperAction();

	public class SocketHelper
	{
		//reference chat server
		private ChatServer chatserver;
		//reference chat client
		private TcpClient client;
		//nickname 
		private string nickname="";
		//room number 0 means no room assigned yet
		private int room=0;
		//data read from client
		private string readdata="";

		//action delegate
		private SocketHelperAction action;

		//public properties
		public int Room
		{
			get{return room;}
		}

		public string Nickname
		{
			get{return nickname;}
		}

		//2 parameters Constructor
		public SocketHelper(ChatServer s,TcpClient c)
		{
			client=c;
			chatserver=s;
			Thread t=new Thread(new ThreadStart(HandleClient));
            t.SetApartmentState(ApartmentState.STA);
			t.Start();
		}

		//Actions
		//DEFAULT
		private void action_default()
		{
			chatserver.Write(client.GetStream(),
					ChatProtocolValues.UNKNOWN_CMD_MSG(readdata));
		}

		//MESSAGE
		private void action_send_message()
		{   
			chatserver.Broadcast(nickname+"> " + readdata,room); 	
		}

		//PRIVATE MESSAGE
		private void action_private_message()
		{
			string[] s=readdata.Split(':');
			string name="null_name"; //give a default dummy name

			string temp=""; //hold the message
			//format is
			//:private:<target>:<message>
			if (s.Length>=3)name=s[2].Trim(); 
			if (s.Length>=4)temp=s[3].Trim();

			TcpClient t=null;
			if (chatserver.FindUserRoom(name)!=0)
			  t=(TcpClient)chatserver.ClientConnections[name.ToUpper()];
	
			if (t!=null)
			{
				//to target
				chatserver.Write(t.GetStream(),
					             ChatProtocolValues.NORMAL_MSG(nickname,temp));
				//to inform sender			          	  
				chatserver.Write(client.GetStream(),
								ChatProtocolValues.NORMAL_MSG(nickname,temp));
			}
			else
				chatserver.Write(client.GetStream(),
					            ChatProtocolValues.USER_NOT_FOUND_MSG(name)); 	
		}

		//LIST
		private void action_list()
		{
			string[] s=readdata.Split(' ');
			int p1=0; //default to unknown room
			if((s.Length)==1)p1=room; //set to current room
  	
			//Get the specified room
			if(s.Length>=2)
				if(s[1].ToUpper()=="ALL")
					p1=-1; //LIST ALL: indicate all rooms
				else
					try
					{
						p1=int.Parse(s[1]); //to get a room number
					} 
					catch{};
	 
			if((p1>chatserver.NumRoom)||(p1==0)) 
				chatserver.Write(client.GetStream(),
					            ChatProtocolValues.NO_SUCH_ROOM_MSG);
			else
			{
				string temp="";					           	


				if(p1==-1)
				{
					for(int i=0;i<chatserver.NumRoom;i++)
						foreach(string s1 in chatserver.RoomUsers[i].Values)
							temp=temp+"\n   "+s1 + " : room " +(i+1);
				}
				else
				{
					foreach(string s1 in chatserver.RoomUsers[p1-1].Values)
						temp=temp+"\n   "+s1 + " : room " +(p1);
				}
			    
				if (temp=="") temp="Empty";		         	  
				   chatserver.Write(client.GetStream(),
									ChatProtocolValues.NORMAL_MSG("server",temp));
									
			} 
		}

		//WHICH ROOM
		private void action_which_room()
		{
			//Inform client of room number
			chatserver.Write(client.GetStream(),
					ChatProtocolValues.YOUR_ROOM_NO_MSG(room)); 	
		}

		//QUIT 
		private void action_quit()
		{
			//Inform client to quit the application
			chatserver.Write(client.GetStream(),
					ChatProtocolValues.QUIT_MSG);	
		}

		//CHANGE ROOM
		private void action_change_room()
		{
			//store old room number
			int oldroom=room;

			//Remove the user from the chat room
			chatserver.RemoveRoomUser(chatserver.RoomUsers[oldroom-1],nickname);

			//Assigned to No room first
			room=0;

			//while no room is assigned
			while(room==0)
			{
				//Get room number from client
				chatserver.Write(client.GetStream(),
                  ChatProtocolValues.CHOOSE_ROOM(nickname,chatserver.NumRoom));
				string temp=chatserver.Read(client.GetStream());
				
				//check for valid room number
				try
				{
					//convert the text message to integer
					int r_n=int.Parse(temp);
					//check to make sure that the room number is within range
					if ((r_n>=1) && (r_n<=chatserver.NumRoom))
						room=r_n;
				}
				catch{}  	  
			} 
			//Add user to the assigned room
			chatserver.AddRoomUser(chatserver.RoomUsers[room-1],nickname);
			//Broadcast to old room participants
			chatserver.Broadcast(ChatProtocolValues.MOVE_TO(nickname,room),oldroom);
			//Broadcast to new room participants
			chatserver.Broadcast(ChatProtocolValues.WELCOME(nickname,room),room);  
 
		}

		//HELP
		private void action_help()
		{
			chatserver.Write(client.GetStream(),
				"server>\n"+
				"  :help\n"+
				"  :change room\n" +
				"  :list\n"+
				"  :list all\n"+
				"  :list <room_no>\n"+
				"  :which room\n"+
				"  :private:<target>:<message>\n"+
				"  :send pic:<target>\n"+
				"  :get pic:<sender>\n" +
                "  :send media:<target>\n" +
                "  :get media:<sender>\n" +
				"  :quit" );      	
		}
 
 		//SEND MEDIA
		private void action_send_media()
		{
			string[] s=readdata.Split(':');
			string name="";
			//format is
			//:send media:<target>
			if (s.Length==3)name=s[2];
 
			//Locate the target
			TcpClient t=null;
            if (chatserver.FindUserRoom(name)!=0)
				t=(TcpClient)chatserver.ClientConnections[name.ToUpper()];


			//If target is found
			if((t!=null))
			{	
				//Inform the sender(client) to send the media
				chatserver.Write(client.GetStream(),ChatProtocolValues.SEND_MEDIA_MSG);
				
				string ext=chatserver.Read(client.GetStream());
				//Find out the number of byte to read from sender
				string snumbytes=chatserver.Read(client.GetStream());
                int numbytes=int.Parse(snumbytes);

                if (numbytes==0){
					chatserver.Write(client.GetStream(),
						"server> No media file to send");

					return;                	
                }
                
                //must be less than 5 MB
                if (numbytes>51200000){
					chatserver.Write(client.GetStream(),
						"server> Media File is larger than 5 MB");

					return;                	
                }                
                
				//read the bytes
				byte[] b=chatserver.ReadBinary(client.GetStream(),numbytes);
				

				if (b==null)
				{
					chatserver.Write(client.GetStream(),
						"server> Transmission Error");

					return;
				}

				//To store the data in a file
				//name convention is <sender>_<target>.ext

				//create a file to hold the extension
				FileStream fext=new FileStream(nickname +"_"+name,FileMode.Create);
				UnicodeEncoding Unicode = new UnicodeEncoding();
				byte[] bytes=Unicode.GetBytes(ext);
                fext.Write(bytes,0,bytes.Length );
                fext.Close(); 

				FileStream f=new FileStream(nickname+"_"+name+"."+ext,FileMode.Create);
				f.Write(b,0,b.Length);
				f.Close();
				//Inform the target that there is a file from sender
				chatserver.Write (t.GetStream(),
							ChatProtocolValues.MEDIA_FROM_MSG(nickname,name));
				//Inform the sender that server had received the media
				chatserver.Write(client.GetStream(),
							ChatProtocolValues.MEDIA_SEND_MSG(nickname));
			}
			else
			{
				//If target is not found inform sender
				chatserver.Write(client.GetStream(),
							ChatProtocolValues.USER_NOT_FOUND_MSG(name)); 
			} 				
		}
		
		//GET MEDIA
		private void action_get_media()
		{
			string[] s=readdata.Split(':');
			string sender="";
			string medianame="";
			string ext="";
			//format is
			//:get media:<sender>:ext
			if(s.Length>=3)sender=s[2];
			if(s.Length>=4)ext=s[3];

			//format of saved jpg file is
			//<sender>_<target>.jpg
			//In this case the current user is the target
		    
			//get the extension form the file
			
			if (File.Exists(sender + "_" + nickname))
			{
              FileStream f=new FileStream(sender + "_" + nickname,FileMode.Open);
              FileInfo fi=new FileInfo(sender + "_" + nickname);
              byte[] b=new byte[fi.Length];
			  f.Read(b,0,b.Length);
              f.Close();
				UnicodeEncoding Unicode = new UnicodeEncoding();
				int charCount = Unicode.GetCharCount(b, 0, b.Length);
				char[] chars = new Char[charCount];
				Unicode.GetChars(b, 0, b.Length, chars, 0);
				ext=new string(chars);

			}

			medianame=sender + "_" + nickname + "."+ext;
			
			//Check for existence of file
			if(!File.Exists(medianame))
				chatserver.Write(client.GetStream(),
					ChatProtocolValues.MEDIA_NOT_FOUND_MSG(medianame));
			else
			{
				//Create a file stream
				FileStream f=new FileStream(medianame,FileMode.Open);
				//To get the size of the file for purpose of memory allocation
				FileInfo fi=new FileInfo(medianame);
				byte[] b=new byte[fi.Length];
				//Read the content of the file and close 
				f.Read(b,0,b.Length);
				f.Close();
				//Inform the client to get the media
				chatserver.Write (client.GetStream(),
								ChatProtocolValues.GET_MEDIA_MSG);
				//Inform the client of the extension
				//chatserver.Write(client.GetStream(),ext);

				//Inform the client of the ext
				chatserver.Write(client.GetStream(),ext);
				//Inform the client of number of bytes
				chatserver.Write(client.GetStream(),""+b.Length);
				//Send the binary data
				chatserver.WriteBinary(client.GetStream(),b);  
				//Inform the client that all binary data has been send
				chatserver.Write(client.GetStream(),
								ChatProtocolValues.MEDIA_SEND_ACK_MSG);  

				//Locate the sender of the media
				TcpClient t=null;
				if (chatserver.FindUserRoom(sender)!=0)
					t=(TcpClient)chatserver.ClientConnections[sender.ToUpper()];

				//Inform the sender that the target has gotten the media
				if(t!=null)
					chatserver.Write(t.GetStream(),
						ChatProtocolValues.GOTTEN_MEDIA_MSG(nickname));		        	   	  	  
			} 				
		}			
		
 
		//SEND PIC
		private void action_send_pic()
		{
			string[] s=readdata.Split(':');
			string name="";
			//format is
			//:send pic:<target>
			if (s.Length==3)name=s[2];
 
			//Locate the target
			TcpClient t=null;
            if (chatserver.FindUserRoom(name)!=0)
				t=(TcpClient)chatserver.ClientConnections[name.ToUpper()];


			//If target is found
			if((t!=null))
			{	
				//Inform the sender(client) to send the picture
				chatserver.Write(client.GetStream(),ChatProtocolValues.SEND_PIC_MSG);
				
				//Find out the number of byte to read from sender
				string snumbytes=chatserver.Read(client.GetStream());
                int numbytes=int.Parse(snumbytes);

				//read the bytes
				byte[] b=chatserver.ReadBinary(client.GetStream(),numbytes);
				if (b==null)
				{
					chatserver.Write(client.GetStream(),
						"server> Transmission Error");

					return;
				}

				//To store the data in a jpg file
				//name convention is <sender>_<target>.jpg
				FileStream f=new FileStream(nickname+"_"+name+".jpg",FileMode.Create);
				f.Write(b,0,b.Length);
				f.Close();
				//Inform the target that there is a picture from sender
				chatserver.Write (t.GetStream(),
							ChatProtocolValues.PIC_FROM_MSG(nickname,name));
				//Inform the sender that server had received the picture
				chatserver.Write(client.GetStream(),
							ChatProtocolValues.PIC_SEND_MSG(nickname));
			}
			else
			{
				//If target is not found inform sender
				chatserver.Write(client.GetStream(),
							ChatProtocolValues.USER_NOT_FOUND_MSG(name)); 
			} 	
		}

		//GET PIC
		private void action_get_pic()
		{
			string[] s=readdata.Split(':');
			string sender="";
			string picname="";
			//format is
			//:get pic:<sender>
			if(s.Length==3)sender=s[2];

			//format of saved jpg file is
			//<sender>_<target>.jpg
			//In this case the current user is the target
			picname=sender + "_" + nickname + ".jpg";
			
			//Check for existence of file
			if(!File.Exists(picname))
				chatserver.Write(client.GetStream(),
					ChatProtocolValues.PIC_NOT_FOUND_MSG(picname));
			else
			{
				//Create a file stream
				FileStream f=new FileStream(picname,FileMode.Open);
				//To get the size of the file for purpose of memory allocation
				FileInfo fi=new FileInfo(picname);
				byte[] b=new byte[fi.Length];
				//Read the content of the file and close 
				f.Read(b,0,b.Length);
				f.Close();
				//Inform the client to get the pic
				chatserver.Write (client.GetStream(),
								ChatProtocolValues.GET_PIC_MSG);
				//Inform the client of number of bytes
				chatserver.Write(client.GetStream(),""+b.Length);
				//Send the binary data
				chatserver.WriteBinary(client.GetStream(),b);  
				//Inform the client that all binary data has been send
				chatserver.Write(client.GetStream(),
								ChatProtocolValues.PIC_SEND_ACK_MSG);  

				//Locate the sender of the picture
				TcpClient t=null;
				if (chatserver.FindUserRoom(sender)!=0)
					t=(TcpClient)chatserver.ClientConnections[sender.ToUpper()];

				//Inform the sender that the target has gotten the picture
				if(t!=null)
					chatserver.Write(t.GetStream(),
						ChatProtocolValues.GOTTEN_PIC_MSG(nickname));		        	   	  	  
			} 	
		}


		//The main method that handle all the chat communication with the client
		private void HandleClient()
		{
			try
			{
				//Autheticate the user
				AuthenticationServer auth=new AuthenticationServer(chatserver,client);
				if(!auth.Authenticate())
					throw(new Exception());

				//Send connection message to client
				//returning the user id
				chatserver.Write(client.GetStream(),
					ChatProtocolValues.CONNECTION_MSG(auth.UserID));

				//buffer
				byte[] bytes= new byte[256];

				//set nickname as user id which is alraedy unique
				nickname=auth.UserID;

				//Assign a room to connected client
				room=chatserver.AssignRoom(nickname);
				Console.WriteLine("room assigned= "+room + " for " + nickname);

				//Add to Connections
				try
				{
					chatserver.AddConnection(nickname,client);
				}
				catch{}

				//Broadcast to chat room about the new user
				chatserver.Broadcast(ChatProtocolValues.WELCOME(nickname,room),room);

				//listen to all client data send
				while((readdata=chatserver.Read(client.GetStream()))!="")
				{
					readdata=readdata.Trim();

					//Print out read data in console
					Console.WriteLine("Read>" + readdata);

					//Check if the readdata is a command
					if(readdata.ToUpper().Substring(0,1)==ChatProtocolValues.IS_CMD)
					{
						//Assign a default action
						action=new SocketHelperAction(action_default);

						//Reassign action based on format content
						if ((readdata.ToUpper()+":").IndexOf(ChatProtocolValues.GET_PIC_CMD)==0)
							action=new SocketHelperAction(action_get_pic);

						if ((readdata.ToUpper()+":").IndexOf(ChatProtocolValues.SEND_PIC_CMD)==0)
							action=new SocketHelperAction(action_send_pic);
							
						if ((readdata.ToUpper()+":").IndexOf(ChatProtocolValues.GET_MEDIA_CMD)==0)
							action=new SocketHelperAction(action_get_media);

						if ((readdata.ToUpper()+":").IndexOf(ChatProtocolValues.SEND_MEDIA_CMD)==0)
							action=new SocketHelperAction(action_send_media);							
							

						if ((readdata.ToUpper()+" ").IndexOf(ChatProtocolValues.HELP_CMD)==0)
							action=new SocketHelperAction(action_help);

						if ((readdata.ToUpper()+" ").IndexOf(ChatProtocolValues.QUIT_CMD)==0)
							action=new SocketHelperAction(action_quit);

						if ((readdata.ToUpper()+" ").IndexOf(ChatProtocolValues.CHANGE_ROOM_CMD)==0)
							action=new SocketHelperAction(action_change_room);

						if ((readdata.ToUpper()+" ").IndexOf(ChatProtocolValues.WHICH_ROOM_CMD)==0)
							action=new SocketHelperAction(action_which_room);

						if ((readdata.ToUpper()+" ").IndexOf(ChatProtocolValues.LIST_CMD)==0)
							action=new SocketHelperAction(action_list);

						if ((readdata.ToUpper()+":").IndexOf(ChatProtocolValues.PRIVATE_MSG_CMD )==0)
							action=new SocketHelperAction(action_private_message);
					}
					else //NON COMMAND
					{
						//if not a command assign to a mesage sending action
						action=new SocketHelperAction(action_send_message);
					} //COMMANDS

					//perform the action
					action();
				}//WHILE

			}
			catch(Exception e)
			{
				//Trapped exception
				Console.WriteLine("The following error is trapped by the chat server");	
				Console.WriteLine("*************************************************");
				Console.WriteLine(e);
				Console.WriteLine("*************************************************");
				Console.WriteLine("Waiting for Connection...");
		
			}
			finally
			{
				//while loop ended or when there are some other problems
				//try to inform client to shut down
				try
				{
					chatserver.Write(client.GetStream(),ChatProtocolValues.QUIT_MSG);
				} 
				catch{}

				//if the client had belong to a room
				if ((room!=0) && (nickname!=""))
				{
					//remove user from room
					chatserver.RemoveRoomUser(chatserver.RoomUsers[room-1],nickname);
					//inform all that the client has logged out
					chatserver.Broadcast(ChatProtocolValues.USER_LOG_OUT(nickname,room));
				}

				//remove the client connection
				try
				{
					chatserver.RemoveConnection(nickname,client);
				}
				catch{}

			}
		}

	}
}
