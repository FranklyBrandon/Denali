using Alpaca.Markets;
using AutoMapper;
using Denali.Models;
using Denali.Services;
using Microsoft.AspNetCore.Mvc;

namespace Denali.WebAPI.Controllers
{
    [ApiController]
    [Route("stocks")]
    public class StockDataController : ControllerBase
    {
        private readonly AlpacaService _alpacaService;
        private readonly IMapper _mapper;
        public StockDataController(AlpacaService alpacaService, IMapper mapper)
        {
            _alpacaService = alpacaService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("{ticker}/aggregate")]
        public async Task<IEnumerable<IBar>> GetHistoricAggregateData(string ticker, DateTime startDateTime, DateTime endDateTime, AggregateTimeFrame timeFrame)
        {
            return await _alpacaService.GetAggregateData(ticker, startDateTime, endDateTime, _mapper.Map<BarTimeFrame>(timeFrame));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<int>>> GetTest()
        {
            return new List<int> { 1, 2, 3 };
            //return Ok(new object());
        }
    }
}
