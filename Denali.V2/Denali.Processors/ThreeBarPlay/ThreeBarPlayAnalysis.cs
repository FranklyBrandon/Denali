using Alpaca.Markets;
using Denali.Models;
using Denali.Services;
using Microsoft.Extensions.Options;

namespace Denali.Processors.ThreeBarPlay
{
    public class ThreeBarPlayAnalysis
    {
        private readonly FileService _fileService;
        private readonly ThreeBarPlaySettings _settings;

        public ThreeBarPlayAnalysis(FileService fileService, IOptions<ThreeBarPlaySettings> settings)
        {
            this._fileService = fileService;
            this._settings = settings.Value;
        }

        public async Task Process()
        {
            var strategy = new ThreeBarPlayStrategy(_settings);
            var premarketBars = await _fileService.LoadResourceFromFile<HistoricalBarsResponse>(Path.Combine("Resources", "bars_BIDU_4_04_2022_premarket.json"));
            var intradayBars = await _fileService.LoadResourceFromFile<HistoricalBarsResponse>(Path.Combine("Resources", "bars_BIDU_4_04_2022.json"));

            strategy.Initialize(premarketBars.Bars);

            for (int i = 1; i < intradayBars.Bars.Count - 1; i++)
            {
                var bars = intradayBars.Bars.GetRange(0, i);
                strategy.ProcessTick(bars);         
            }
        }
    }
}
