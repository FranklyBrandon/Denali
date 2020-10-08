using Denali.Services.Data.Alpaca;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Services.Market.Alpaca
{
    public class AlpacaMarketService
    {
        private readonly AlpacaDataClient _client;
        public AlpacaMarketService(AlpacaDataClient client)
        {
            this._client = client;
        }
    }
}
