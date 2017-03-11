//*********************************************************************************
//
//  Filename:	ChatProtocolValues.cs
//
//	Authors:	Yang Kok Wah
//				Tantrarat Sumalee
//				Valerie Chung Foong Mooi
//				Pang Foong Har
//
//	Date:		28 Feb 2004
//	
//	Summary:	Contains ChatProtocolValues class 
//				Define all Chat protocol components
//				
//********************************************************************************


using System;

namespace Chat
{

	public class ChatProtocolValues
	{
		//Commands
		public static string IS_CMD=":";
		public static string GET_PIC_CMD=":GET PIC:";
		static public string SEND_PIC_CMD=":SEND PIC:";
		public static string GET_MEDIA_CMD=":GET MEDIA:";
		static public string SEND_MEDIA_CMD=":SEND MEDIA:";		
		static public string PRIVATE_MSG_CMD=":PRIVATE:";
		static public string HELP_CMD=":HELP ";
		static public string QUIT_CMD=":QUIT ";
		static public string CHANGE_ROOM_CMD=":CHANGE ROOM ";
		static public string WHICH_ROOM_CMD=":WHICH ROOM ";
		static public string LIST_CMD=":LIST ";

		//Messages
		static public string QUIT_MSG="server> quit";
		static public string NO_SUCH_ROOM_MSG="server> No such room";
		
		static public string SEND_PIC_MSG="server> send pic";
		static public string SEND_MEDIA_MSG="server> send media";	
			
		static public string PIC_SEND_ACK_MSG="server> picture sent";
		static public string MEDIA_SEND_ACK_MSG="server> media sent";
				
		static public string GET_PIC_MSG="server> get pic";
		static public string GET_MEDIA_MSG="server> get media";		
		
		static public string CONNECTION_HEADER_MSG="<Connection>:";

		//Formatted Messages
		static public string GOTTEN_PIC_MSG(string name)
		{
			return "server> "+ name +" gotten picture";
		}
		static public string PIC_NOT_FOUND_MSG(string picname)
		{
			return "server> " +picname +" not found";
		}
		static public string PIC_SEND_MSG(string sender)
		{
			return "server> " + sender + ", picture sent";
		}

		static public string PIC_FROM_MSG(string sender,string target)
		{
			return "server> "+ target + ", picture from " + sender;
		}
		
		
		static public string GOTTEN_MEDIA_MSG(string name)
		{
			return "server> "+ name +" gotten media";
		}
		static public string MEDIA_NOT_FOUND_MSG(string medianame)
		{
			return "server> " +medianame +" not found";
		}
		static public string MEDIA_SEND_MSG(string sender)
		{
			return "server> " + sender + ", media sent";
		}

		static public string MEDIA_FROM_MSG(string sender,string target)
		{
			return "server> "+ target + ", media from " + sender;
		}		
		
		

		static public string YOUR_ROOM_NO_MSG(int room_no)
		{
			return "server> " +"You are in Room " + room_no;
		}

		static public string NORMAL_MSG(string sender,string msg)
		{
		  return sender+"> " + msg;	
		}

		static public string USER_NOT_FOUND_MSG(string name)
		{
			return "server> " + name + " is not found";
		}

		static public string UNKNOWN_CMD_MSG(string cmd)
		{
			return "server> "+cmd + " is an unknown command";
		}

		static public string CONNECTION_MSG(string name)
		{
			return CONNECTION_HEADER_MSG+name;
		}

		static public string MOVE_TO(string name,int room)
		{
			return "server> "+name + " moved to Room " + room;
		}

		static public string CHOOSE_ROOM(string name,int num_rooms)
		{
			return "server> "+name +" Choose Room Number: 1 - " + num_rooms;
		}

		static public string WELCOME(string name, int room_no)
		{
			return "server> Welcome "+name+" to Room " + room_no;
		}

		static public string USER_LOG_OUT(string name,int room_no)
		{
			return "server> " +name +" from Room " +room_no + " has logged out";
		}

		//Private Constructor to ensure that class cannot be instantiated
		private ChatProtocolValues()
		{
		}
	}
}
