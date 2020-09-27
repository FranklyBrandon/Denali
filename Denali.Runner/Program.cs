
using Denali.Runner.ConsoleTools;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Denali.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = DenaliConfiguration.Startup();
            var consoleTool = new DenaliConsoleTool();
            consoleTool.RunCommand(provider, args);
        }
    }
}
