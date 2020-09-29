using Denali.Models.Data.Alpaca;
using Denali.Processors.SignalAnalysis.Signals;
using Denali.Services.Alpaca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.SignalAnalysis
{
    public class SignalAnalysisProcessor : IProcessor
    {
        private readonly AlpacaService _alpacaService;
        private readonly IEnumerable<ISignalAlgo> _signalAlgos;
        public SignalAnalysisProcessor(AlpacaService alpacaService, IEnumerable<ISignalAlgo> signalAlgos)
        {
            _alpacaService = alpacaService;
            _signalAlgos = signalAlgos;
        }

        public async Task Process(Dictionary<string, string> arguments)
        {
            var barChart = await GetBarData(arguments);

            foreach (var symbol in barChart)
            {
                var max = symbol.Value.Count;
                for (int i = 0; i < max; i++)
                {
                    foreach (var algo in _signalAlgos)
                    {
                        algo.Process(symbol.Value, i, (i == 0), (i == max));
                    }
                }
            }
        }

        private async Task<Dictionary<string, List<Candle>>> GetBarData(Dictionary<string, string> arguments)
        {
            var resolution = arguments["resolution"];
            var start = arguments["start"];
            var end = arguments["end"];
            var symbols = arguments["symbols"];
            return await _alpacaService.GetBarData(resolution: resolution, start: start, end: end, symbols: symbols);
        }
    }
}
