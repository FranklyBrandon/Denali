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

            var la = await _fileService.LoadResourceFromFile<HistoricalBarsResponse>(Path.Combine("Resources", "bars.json"));
            for (int i = 1; i < la.Bars.Count - 1; i++)
            {
                var bars = la.Bars.GetRange(0, i);
                var currentBar = bars.Last();

              
            }
        }
    }
}
