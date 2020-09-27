using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public interface IProcessor
    {
        Task Process(Dictionary<string, string> arguments);
    }
}
