using Alpaca.Markets;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public ThreeBarPlayAnalysis(FileService fileService, IOptions<ThreeBarPlaySettings> settings, IMapper mapper)
        {
            this._fileService = fileService;
            this._settings = settings.Value;
            this._mapper = mapper;
        }

        public async Task Process()
        {
            var strategy = new ThreeBarPlayStrategy(_settings);
            var premarketBars = await _fileService.LoadResourceFromFile<HistoricalBarsResponse>(Path.Combine("Resources", "bars_AAPL_4_21_2022.json"));
            var intradayBars = await _fileService.LoadResourceFromFile<HistoricalBarsResponse>(Path.Combine("Resources", "bars_AAPL_4_22_2022.json"));

            var mappedPremarketBars = _mapper.Map<List<AggregateBar>>(premarketBars.Bars);
            var mappedIntradayBars = _mapper.Map<List<AggregateBar>>(intradayBars.Bars);

            strategy.Initialize(mappedPremarketBars.Cast<IAggregateBar>().ToList());

            var allBars = new List<IAggregateBar>();
            allBars.AddRange(mappedPremarketBars);
            allBars.AddRange(mappedIntradayBars);
            allBars = allBars.Where(x => x.TotalRange() > 0).ToList();

            int start = premarketBars.Bars.Count + 1;
            int count = allBars.Count - 1;

            // Start analysis at start of day (not including premarket bars)
            for (int i = start; i < count; i++)
            {
                var bars = allBars.Take(i).ToList();
                strategy.ProcessTick(bars);         
            }

            await _fileService.WriteResourceToFile(Path.Combine("Resources", "AAPL_Elephants_4_22_2022.json"), strategy.ElephantBars.Elephants);
        }
    }
}
