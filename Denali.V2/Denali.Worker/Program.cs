using Denali.Worker.Configuration;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) => ContainerConfiguration.Configure(hostContext.Configuration, services))
    .ConfigureHostConfiguration(configHost =>
    {
        configHost.SetBasePath(Directory.GetCurrentDirectory());
        configHost.AddCommandLine(args);
        configHost.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: false);
        configHost.AddEnvironmentVariables();
    })
    .Build();

await host.RunAsync();
