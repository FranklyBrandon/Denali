using Denali.Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => ContainerConfiguration.Configure(services))
    .Build();

await host.RunAsync();
