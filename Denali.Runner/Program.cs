using Denali.Processors;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Denali.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = DenaliConfiguration.Startup();

            using (var scope = provider.CreateScope())
            {
                var processor = scope.ServiceProvider.GetRequiredService<HistoricAnalyze>();
                processor.Process().GetAwaiter().GetResult();
            }
        }
    }
}
