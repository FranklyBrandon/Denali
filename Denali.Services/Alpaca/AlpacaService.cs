﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Services.Alpaca
{
    public class AlpacaService
    {
        private readonly AlpacaClient _alpacaClient;

        public AlpacaService(AlpacaClient alpacaClient)
        {
            _alpacaClient = alpacaClient;
        }

        public async Task<string> GetBarData(string resolution, int limit = 0, string start = "", string end = "", params string[] symbols)
        {
            return await _alpacaClient.GetBars(resolution, limit, start, end, symbols);
        }
    }
}