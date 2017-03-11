//*********************************************************************************
//
//  Filename:	AuthenticationProtocolValues.cs
//
//	Authors:	Yang Kok Wah
//				Tantrarat Sumalee
//				Valerie Chung Foong Mooi
//				Pang Foong Har
//
//	Date:		28 Feb 2004
//
//	Summary:	Contains AuthenticationProtocolValues class 
//				Define all Authentication protocol components
//				
//********************************************************************************


using System;

namespace Chat
{

	public class AuthenticationProtocolValues
	{
		//Prompts
		static public string COMMAND_PROMPT="> Command:";
		static public string USERID_PROMPT="> UserID:";
		static public string PASSWORD_PROMPT="> Password:";
		static public string IS_PROMPT=":";

		//Commands
		static public string LOGIN_COMMAND="LOGIN";
		static public string REGISTER_COMMAND="REGISTER";

		//Messages
		static public string QUIT_MSG="server> quit";
		static public string AUTENTICATED_MSG="> AUTHENTICATED\n";
		static public string USERID_START_WITH_WORDCHAR_MSG=
			                       "> User ID should start with an word character"+
								   " and contain no spaces\n";
		static public string USER_ALREADY_REGISTERED_MSG=
								   "> User already registered\n";
		static public string USER_ALREADY_LOGGED_IN=
								   "> User already logged in\n";
		static public string USER_NOT_REGISTERED_MSG=
								   "> User not registered\n";
		static public string PASSWORD_AT_LEAST_4_CHAR_MSG=
								   "> Password should be at least 4 characters\n";
		static public string VALID_COMANDS_MSG=
								   "Valid commands are login and register\n";


		//Private Constructor to ensure that class cannot be instantiated
		private AuthenticationProtocolValues()
		{
		}
	}
}
