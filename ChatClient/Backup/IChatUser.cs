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
//	Summary:	Contains IChatUser interface 
//				Define all necessary components for a Chat User
//				
//********************************************************************************

using System;

namespace Chat
{
	public interface IChatUser
	{
		//Basic User Attributes
		string UserID{get;set;}
		string Password{get;set;}

	}
}
