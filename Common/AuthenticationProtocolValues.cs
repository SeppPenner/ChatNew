namespace Chat
{
    public static class AuthenticationProtocolValues
    {
        public static string AutenticatedMsg = "> AUTHENTICATED\n";
        public static string CommandPrompt = "> Command:";
        public static string LoginCommand = "LOGIN";

        public static string PasswordAtLeast4CharMsg =
            "> Password should be at least 4 characters\n";

        public static string PasswordPrompt = "> Password:";
        public static string QuitMsg = "server> quit";
        public static string RegisterCommand = "REGISTER";

        public static string UserAlreadyLoggedIn =
            "> User already logged in\n";

        public static string UserAlreadyRegisteredMsg =
            "> User already registered\n";

        public static string UseridPrompt = "> UserID:";

        public static string UseridStartWithWordcharMsg =
            "> User ID should start with an word character" +
            " and contain no spaces\n";

        public static string UserNotRegisteredMsg =
            "> User not registered\n";

        public static string ValidComandsMsg =
            "Valid commands are login and register\n";
    }
}