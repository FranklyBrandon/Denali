using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Models.Data.Trading
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
