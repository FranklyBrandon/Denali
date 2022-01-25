using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Reflection;

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
            var logPath = 
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "scalp_logs"))
                .CreateLogger();

            return new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddCommandLine(args);
                    configHost.AddJsonFile($"appsettings.{environment}.json", optional: false);
                    configHost.AddEnvironmentVariables();
                })
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    new DenaliConfiguration(hostContext.Configuration, services).ConfigureServices();
                })
                //For hosting on Linux
                .UseSystemd();            
        }
    }
}
