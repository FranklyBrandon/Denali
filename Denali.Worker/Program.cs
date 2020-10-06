using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Denali.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var environment = GetEnvironment(args);

            return new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddCommandLine(args);
                    configHost.AddJsonFile($"appsettings.{environment}.json", optional: false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    new DenaliConfiguration(hostContext.Configuration, services).ConfigureServices();
                })
                //For hosting on Linux
                .UseSystemd();    
            
        }

        private static string GetEnvironment(string[] args)
        {
            var index = Array.IndexOf(args, "--env");

            if (index < 0)
                throw new ArgumentException("No Enviornment provided");

            return args[index + 1];
        }
    }
}
