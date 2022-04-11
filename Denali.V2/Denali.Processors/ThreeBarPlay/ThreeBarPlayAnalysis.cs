using Alpaca.Markets;
using Denali.Models;
using Denali.Services;
using Denali.TechnicalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.ThreeBarPlay
{
    public class ThreeBarPlayAnalysis
    {
        private readonly FileService _fileService;

        public ThreeBarPlayAnalysis(FileService fileService)
        {
            this._fileService = fileService;
        }

        public async Task Process()
        {
            var la = await _fileService.LoadResourceFromFile<HistoricalBarsResponse>(Path.Combine("Resources", "bars.json"));
            for (int i = 1; i < la.Bars.Count - 1; i++)
            {
                var bars = la.Bars.GetRange(0, i);
                var currentBar = bars.Last();

              
            }
        }
    }
}
