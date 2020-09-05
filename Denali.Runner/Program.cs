using Denali.Runner.Processors;
using Denali.Services.Utility;
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
                var processor = scope.ServiceProvider.GetRequiredService<AnalyzeProcessor>();
                processor.Process().GetAwaiter().GetResult();
            }
        }
    }
}
