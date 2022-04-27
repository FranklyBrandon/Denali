using Alpaca.Markets;
using Denali.Models;
using Denali.Models.Alpaca;
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
            var premarketBars = await _fileService.LoadResourceFromFile<HistoricalBarsResponse>(Path.Combine("Resources", "bars_AAPL_4_21_2022.json"));
            var intradayBars = await _fileService.LoadResourceFromFile<HistoricalBarsResponse>(Path.Combine("Resources", "bars_AAPL_4_22_2022.json"));

            // TODO: use mapper to map to denali models
            //strategy.Initialize(premarketBars.Bars);

            var allBars = new List<Bar>();
            allBars.AddRange(premarketBars.Bars);
            allBars.AddRange(intradayBars.Bars);

            int start = premarketBars.Bars.Count + 1;
            int count = allBars.Count - 1;

            // Start analysis at start of day (not including premarket bars)
            for (int i = start; i < count; i++)
            {
                var bars = allBars.Take(i).ToList();
                // TODO: use mapper to map to denali models
                //strategy.ProcessTick(bars);         
            }
        }
    }
}
