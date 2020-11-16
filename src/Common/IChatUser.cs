namespace Chat
{
    public interface IChatUser
    {
        string Password { get; set; }

        string UserId { get; set; }
    }
}