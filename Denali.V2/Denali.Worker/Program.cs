using Denali.Worker;
using Denali.Worker.Configuration;

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
    .Build();

await host.RunAsync();
