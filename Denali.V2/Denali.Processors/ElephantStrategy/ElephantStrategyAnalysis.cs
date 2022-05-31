using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Models.Alpaca;
using Denali.Services;
using Microsoft.Extensions.Options;

namespace Denali.Processors.ElephantStrategy
{
    public class ElephantStrategyAnalysis
    {
        private readonly FileService _fileService;
        private readonly ElephantStrategySettings _settings;
        private readonly IMapper _mapper;

        public ElephantStrategyAnalysis(FileService fileService, IOptions<ElephantStrategySettings> settings, IMapper mapper)
        {
            this._fileService = fileService;
            this._settings = settings.Value;
            this._mapper = mapper;
        }

        public async Task Process()
        {
            var strategy = new ElephantStrategy(_settings);
            var premarketBars = await _fileService.LoadJSONResourceFromFile<HistoricalBarsResponse>(Path.Combine("Resources", "5_11_2022_AAPL_1Min.json"));
            var intradayBars = await _fileService.LoadJSONResourceFromFile<HistoricalBarsResponse>(Path.Combine("Resources", "5_12_2022_AAPL_1Min.json"));

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

            await _fileService.WriteJSONResourceToFile(Path.Combine("Resources", "AAPL_Elephants_5_12_2022.json"), strategy.ElephantBars.Elephants);
            await _fileService.WriteJSONResourceToFile(Path.Combine("Resources", "AAPL_Retracements_5_12_2022.json"), strategy.Retracements);
        }
    }
}
