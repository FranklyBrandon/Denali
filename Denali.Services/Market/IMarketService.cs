using Denali.Models.Data.Alpaca;
using Denali.Models.Data.Trading;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Services.Market
{
    public interface IMarketService
    {
        public List<Position> Positions { get; set; }
        public List<StockAction> ManagePositions(Dictionary<string, List<Candle>> barData, List<StockAction> entryPositions);
    }
}
