using Denali.Models.Data.Alpaca;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services.Data
{
    public interface IMarketDataProvider
    {
        Task<Dictionary<string, List<Candle>>> GetBarData(string resolution, int limit = 0, string start = "", string end = "", params string[] symbols);
    }
}
