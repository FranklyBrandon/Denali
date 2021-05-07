using Denali.Models.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Processors
{
    public class DataAnalysisProcessor : IProcessor
    {
        private readonly IConfiguration _configuration;

        public DataAnalysisProcessor(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public void OnBarReceived(IAggregateData barData)
        {
            throw new NotImplementedException();
        }

        public async Task Process(DateTime startTime, CancellationToken stoppingToken)
        {
            var symbols = _configuration["symbols"].Split(',');
            var fromDate = DateTime.Parse(_configuration["from"]);
            var toDate = DateTime.Parse(_configuration["to"]);
        }

        public Task ShutDown(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
