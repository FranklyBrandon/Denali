namespace InteractiveBrokers.Models.Response
{
    public class Ping
    {
        public string session { get; set; }
        public int ssoExpires { get; set; }
        public bool collission { get; set; }
        public int userId { get; set; }
        public Hmds hmds { get; set; }
        public Iserver iserver { get; set; }
    }

    public class Hmds
    {
        public AuthStatus authStatus { get; set; }
        public string error { get; set; }
    }

    public class Iserver
    {
        public AuthStatus authStatus { get; set; }
    }
}
