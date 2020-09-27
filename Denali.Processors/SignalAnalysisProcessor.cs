using Denali.Services.Alpaca;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class SignalAnalysisProcessor : IProcessor
    {
        private readonly AlpacaService _alpacaService;
        public SignalAnalysisProcessor(AlpacaService alpacaService)
        {
            _alpacaService = alpacaService;
        }

        public async Task Process(Dictionary<string, string> arguments)
        {
            var resolution = arguments["resolution"];
            var start = arguments["start"];
            var end = arguments["end"];
            var symbols = arguments["symbols"];
            var candles = await _alpacaService.GetBarData(resolution: resolution, start: start, end: end, symbols: symbols);
        }
    }
}
