using Denali.Services.Data.Alpaca;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Services.Market
{
    public class AlpacaMarketService
    {
        private readonly AlpacaClient _client;
        public AlpacaMarketService(AlpacaClient client)
        {
            this._client = client;
        }
    }
}
