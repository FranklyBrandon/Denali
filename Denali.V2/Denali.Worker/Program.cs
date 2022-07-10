using Denali.Worker;
using Denali.Worker.Configuration;
using Serilog;
using Serilog.Events;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "elephant_logs.txt"), rollingInterval: RollingInterval.Day)
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) => ContainerConfiguration.Configure(hostContext.Configuration, hostContext.HostingEnvironment, services))
    .ConfigureHostConfiguration((configHost) =>
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        configHost.SetBasePath(Directory.GetCurrentDirectory());
        configHost.AddCommandLine(args);
        configHost.AddJsonFile($"appsettings.{environment}.json", optional: false);
        configHost.AddEnvironmentVariables();

        if (environment.Equals(Environments.Development))
            configHost.AddUserSecrets<Worker>(optional: false);
    })
    .UseSerilog()
    .Build();

await host.RunAsync();
