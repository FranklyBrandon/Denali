using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Worker
{
    public static class ConsoleUtilities
    {
        public static string GetArgument(string[] args, string key)
        {
            var index = Array.IndexOf(args, key);

            if (index < 0)
                throw new ArgumentException($"No argument providded for {key}");

            return args[index + 1];
        }
    }
}
