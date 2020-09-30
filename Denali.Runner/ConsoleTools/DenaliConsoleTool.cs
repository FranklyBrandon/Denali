using Denali.Processors;
using Denali.Processors.SignalAnalysis;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Denali.Runner.ConsoleTools
{
    public class DenaliConsoleTool
    { 
        public void RunCommand(IServiceProvider provider, string[] args)
        {
            var arguments = GetCommandLineArguments(args);
            var command = GetCommand(arguments);

            using (var scope = provider.CreateScope())
            {
                IProcessor processor = default;
                switch (command)
                {
                    case Command.SignalAnalysis:
                        processor = scope.ServiceProvider.GetService<HistoricSignalAnalysis>();
                        break;
                    default:
                        break;
                }

                processor.Process(arguments).GetAwaiter().GetResult();
                //var processor = scope.ServiceProvider.GetRequiredService<HistoricAnalysisPrcessor>();
                //processor.Process().GetAwaiter().GetResult();
            }
        }

        private Command GetCommand(Dictionary<string,string> arguments)
        {
            if (arguments.ContainsKey("bars"))
            {

            }
            else if (arguments.ContainsKey("signal"))
            {
                if (arguments.ContainsKey("analysis"))
                {
                    return Command.SignalAnalysis;
                }
            }

            throw new Exception("No Command found");
        }
        private Dictionary<string,string> GetCommandLineArguments(string[] args)
        {
            var arguments = new Dictionary<string, string>();

            foreach (string argument in args)
            {
                string[] splitted = argument.Split('=');

                if (splitted.Length == 1)
                {
                    arguments[splitted[0]] = "no value";
                }
                else if (splitted.Length == 2)
                {
                    arguments[splitted[0]] = splitted[1];
                }
            }

            return arguments;
        }
    }
}
