using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Services.Settings
{
    public class AuthenticationSettings
    {
        public const string Key = "Authentication";
        public string APIKey { get; set; }
        public string APISecretKey { get; set; }
    }
}
