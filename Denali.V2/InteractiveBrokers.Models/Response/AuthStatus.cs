namespace InteractiveBrokers.Models.Response
{
    public class AuthStatus
    {
        public bool authenticated { get; set; }
        public bool competing { get; set; }
        public bool connected { get; set; }
        public string message { get; set; }
        public string MAC { get; set; }
        public ServerInfo serverInfo { get; set; }
        public string hardware_info { get; set; }
    }

    public class ServerInfo
    {
        public string serverName { get; set; }
        public string serverVersion { get; set; }
    }
}
