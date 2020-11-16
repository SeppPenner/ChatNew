//*********************************************************************************
//
//  Filename:	ChatUser.cs
//
//	Authors:	Yang Kok Wah
//				Tantrarat Sumalee
//				Valerie Chung Foong Mooi
//				Pang Foong Har
//
//	Date:		28 Feb 2004
//	
//	Summary:	Contains ChatUser Class
//				Implements a standard user definition.
//				For user id and password format checking.
//
//********************************************************************************

using System;
using System.Text.RegularExpressions;

namespace Chat
{
	//Marked as Serializable
	[Serializable]
	public class ChatUser:IChatUser
	{
		private string userid;
		private string password;

		//2 parameters Constructor
		public ChatUser(string userid,string password)
		{
			UserID=userid;
			Password=password;
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

		public static bool ValidUserID(string s)
		{
			//the name "server" is reserved
			if(s.ToUpper().IndexOf("SERVER")==0) return false;

			Match m=Regex.Match(s,@"\s+");
			//Spaces not allowed
			if (m.Success) return false;

			//Start with one alphabet and one or more word character
			m=Regex.Match(s,@"^\w+");
			return m.Success;

		}

		//Valid Password must be at least 4 characters
		public static bool ValidPassword(string s)
		{
			Match m=Regex.Match(s,@".{4,}");
			return m.Success;
		}

		#endregion
	}
}
