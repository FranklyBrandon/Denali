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
            var environment = ConsoleUtilities.GetArgument(args, "--env");

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
    }
}
