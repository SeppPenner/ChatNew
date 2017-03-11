namespace Chat
{
    public static class ChatProtocolValues
    {
        public static string ChangeRoomCmd = ":CHANGE ROOM ";
        public static string ConnectionHeaderMsg = "<Connection>:";
        public static string GetMediaCmd = ":GET MEDIA:";
        public static string GetMediaMsg = "server> get media";
        public static string GetPicCmd = ":GET PIC:";
        public static string GetPicMsg = "server> get pic";
        public static string HelpCmd = ":HELP ";
        public static string IsCmd = ":";
        public static string ListCmd = ":LIST ";
        public static string MediaSendAckMsg = "server> media sent";
        public static string NoSuchRoomMsg = "server> No such room";
        public static string PicSendAckMsg = "server> picture sent";
        public static string PrivateMsgCmd = ":PRIVATE:";
        public static string QuitCmd = ":QUIT ";
        public static string QuitMsg = "server> quit";
        public static string SendMediaCmd = ":SEND MEDIA:";
        public static string SendMediaMsg = "server> send media";
        public static string SendPicCmd = ":SEND PIC:";
        public static string SendPicMsg = "server> send pic";
        public static string WhichRoomCmd = ":WHICH ROOM ";

        public static string GOTTEN_PIC_MSG(string name)
        {
            return "server> " + name + " gotten picture";
        }

        public static string PIC_NOT_FOUND_MSG(string picname)
        {
            return "server> " + picname + " not found";
        }

        public static string PIC_SEND_MSG(string sender)
        {
            return "server> " + sender + ", picture sent";
        }

        public static string PIC_FROM_MSG(string sender, string target)
        {
            return "server> " + target + ", picture from " + sender;
        }

        public static string GOTTEN_MEDIA_MSG(string name)
        {
            return "server> " + name + " gotten media";
        }

        public static string MEDIA_NOT_FOUND_MSG(string medianame)
        {
            return "server> " + medianame + " not found";
        }

        public static string MEDIA_SEND_MSG(string sender)
        {
            return "server> " + sender + ", media sent";
        }

        public static string MEDIA_FROM_MSG(string sender, string target)
        {
            return "server> " + target + ", media from " + sender;
        }

        public static string YOUR_ROOM_NO_MSG(int roomNo)
        {
            return "server> " + "You are in Room " + roomNo;
        }

        public static string NORMAL_MSG(string sender, string msg)
        {
            return sender + "> " + msg;
        }

        public static string USER_NOT_FOUND_MSG(string name)
        {
            return "server> " + name + " is not found";
        }

        public static string UNKNOWN_CMD_MSG(string cmd)
        {
            return "server> " + cmd + " is an unknown command";
        }

        public static string CONNECTION_MSG(string name)
        {
            return ConnectionHeaderMsg + name;
        }

        public static string MOVE_TO(string name, int room)
        {
            return "server> " + name + " moved to Room " + room;
        }

        public static string CHOOSE_ROOM(string name, int numRooms)
        {
            return "server> " + name + " Choose Room Number: 1 - " + numRooms;
        }

        public static string Welcome(string name, int roomNo)
        {
            return "server> Welcome " + name + " to Room " + roomNo;
        }

        public static string USER_LOG_OUT(string name, int roomNo)
        {
            return "server> " + name + " from Room " + roomNo + " has logged out";
        }
    }
}