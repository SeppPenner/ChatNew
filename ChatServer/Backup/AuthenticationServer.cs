//*********************************************************************************
//
//  Filename:	AuthenticationServer.cs
//
//	Authors:	Yang Kok Wah
//				Tantrarat Sumalee
//				Valerie Chung Foong Mooi
//				Pang Foong Har
//
//	Date:		28 Feb 2004
//	
//	Summary:	Contains AuthenticationServer class which implements the IChatUser
//				interface.
//				Also contains AuthenticateAction delegete.
//				Perform Authentication Services: register and login							
//				
//********************************************************************************

using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Chat
{
	public delegate bool AuthenticateAction();

	public class AuthenticationServer :IChatUser
	{
		//Delegate for actions
		private AuthenticateAction action;
		//reference chat server
		private ChatServer chatserver;
		//reference client connection
		private TcpClient client;
		//store for users attributes
		private string userid;
		private string password;


		//Constructor to initialized reference server and client connection
		public AuthenticationServer(ChatServer s,TcpClient c)
		{
			chatserver=s;
			client=c;
		}

	
		//Main authentication method
		public bool Authenticate()
		{
		    byte[] bytes=new byte[256];
			string data=null;
			bool validate_command=false;

			//Get user command
			//Valid command is login and register
			while(!validate_command)
			{
				chatserver.Write(client.GetStream(),
							     AuthenticationProtocolValues.COMMAND_PROMPT);
				data=chatserver.Read(client.GetStream());

				data=data.Trim();

				//default action is to assume that the command is invalid
				action= new AuthenticateAction(action_default);

				//actions are reassigned based on data entered by user
				if(data.ToUpper().IndexOf(AuthenticationProtocolValues.LOGIN_COMMAND)==0)
				{
					action=new AuthenticateAction(action_login);
					validate_command=true;
				}

				if(data.ToUpper().IndexOf(AuthenticationProtocolValues.REGISTER_COMMAND)==0)
				{
					action=new AuthenticateAction(action_register);
					validate_command=true;
				}
				
				//perform the action
				//if it failed the the process has failed
				if(!action()) return false;
			}//While

			//when out of loop the command has succeed
			//send the authenticated message to the client
			chatserver.Write(client.GetStream(),
				AuthenticationProtocolValues.AUTENTICATED_MSG);

			return true;
		}

		//Private Methods
		//Check if user is registered
		private bool IsRegisteredUser(string userid)
		{
			return (chatserver.ChatUsers[userid.ToUpper()]!=null)?true:false;
		}

		//Check if user password is correct
		private bool IsUserPassword(string userid,string password)
		{
			if(chatserver.ChatUsers[userid.ToUpper()]!=null)
				if(((ChatUser)chatserver.ChatUsers[userid.ToUpper()]).Password==password)
					return true;

			return false;
		}

		//All the actions
		//DEFAULT
		//Invalid command
		private bool action_default()
		{
			chatserver.Write(client.GetStream(),
							AuthenticationProtocolValues.VALID_COMANDS_MSG);
			return true;
		}

		//REGISTER
		private bool action_register()
		{
			//init user id to blank
			UserID="";
			string temp="";
			//Loop until user id not blank
			while(UserID=="")
			{
				//prompt for user id
				chatserver.Write(client.GetStream(),
								AuthenticationProtocolValues.USERID_PROMPT);
				temp=chatserver.Read(client.GetStream());

				//check validity of user id
				if(!ChatUser.ValidUserID(temp))
				{
					chatserver.Write(client.GetStream(),
						AuthenticationProtocolValues.USERID_START_WITH_WORDCHAR_MSG);
					continue;
				}
				
				//if the user id is already used
				if(IsRegisteredUser(temp))
				{
					chatserver.Write(client.GetStream(),
									AuthenticationProtocolValues.USER_ALREADY_REGISTERED_MSG);

					continue;
				}
				else
					UserID=temp; //valid and not used id

			}

			//init password to blank
			Password="";
			//Loop until password is not blank
			while(Password=="")
			{
				//prompt for password
				chatserver.Write(client.GetStream(),
								AuthenticationProtocolValues.PASSWORD_PROMPT);

				temp=chatserver.Read(client.GetStream());

				//check validity of password
				if(!ChatUser.ValidPassword(temp))
				{
					chatserver.Write(client.GetStream(),
								AuthenticationProtocolValues.PASSWORD_AT_LEAST_4_CHAR_MSG);

				    continue;
				}
				else
					Password=temp;//Valid password is assigned
			}

			//Add newly registered chat user
			chatserver.AddChatUser(UserID,new ChatUser(UserID,Password));
			//Serialized the chat users to file to reflect new user
			chatserver.SerializeChatUsers("users.bin");

			return true;
		}


		//LOGIN
		private bool action_login()
		{
			//init user id to blank
			UserID="";
			string temp="";

			//init number of tries to 0
			int numtries=0;

			//Loop until user id is not blank
			while(UserID=="")
			{
				//prompt for user id
				chatserver.Write(client.GetStream(),
								AuthenticationProtocolValues.USERID_PROMPT);

				temp=chatserver.Read(client.GetStream());

				//check if user is registered
				//allow up to 3 tries
				if(!IsRegisteredUser(temp))
				{
					chatserver.Write(client.GetStream(),
						AuthenticationProtocolValues.USER_NOT_REGISTERED_MSG);
					if(++numtries>=3) return false;
					continue;
				}
				else
				{
					UserID=temp; //assign valid user id
				}
			}

			//init to blank
			Password="";
			//init number of tries to 0
			numtries=0;
			
			//Loop until password is not blank
			while(Password=="")
			{
				//prompt for password
				chatserver.Write(client.GetStream(),
								AuthenticationProtocolValues.PASSWORD_PROMPT);

				temp=chatserver.Read(client.GetStream());

				//check if password is correct
				if(!IsUserPassword(UserID,temp))
				{
					if(++numtries>=3) return false;
					continue;
				}
				else
					Password=temp; //OK password is assigned
			}

			//Check if user already has a active connection
			if (chatserver.ClientConnections[UserID.ToUpper()]!=null)
			{
				chatserver.Write(client.GetStream(),
					AuthenticationProtocolValues.USER_ALREADY_LOGGED_IN);
				return false;
			}

			
			return true;
		}
	
		#region IChatUser Members

		public string UserID
		{
			get
			{
				return userid;
			}
			set
			{
				userid=value;
			}
		}

		public string Password
		{
			get
			{
				return password;
			}
			set
			{
				password=value;
			}
		}

		#endregion
	}
}
