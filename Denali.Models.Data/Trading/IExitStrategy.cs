using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Models.Trading
{
    public interface IExitStrategy
    {
        public ExitStrategyType ExitStrategyType { get; set; }
    }

    public enum ExitStrategyType
    {
        Static,
        Dynamic
    }
}
