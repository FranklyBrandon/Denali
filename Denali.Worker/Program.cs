using Microsoft.Extensions.Hosting;

namespace Denali.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //Linux support
                .UseSystemd()
                .ConfigureServices((hostContext, services) =>
                {
                    DenaliConfiguration.ConfigureServices(hostContext.Configuration, services);
                });
    }
}
