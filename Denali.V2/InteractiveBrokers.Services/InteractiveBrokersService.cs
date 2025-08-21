using InteractiveBrokers.Models.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveBrokers.Services
{
    public interface IInteractiveBrokersService
    {
        Task PingServer();
    }

    public class InteractiveBrokersService : IInteractiveBrokersService
    {
        private readonly IInteractiveBrokersClient _httpClient;
        private readonly ILogger _logger;

        public InteractiveBrokersService(IInteractiveBrokersClient httpClient, ILogger<InteractiveBrokersService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task PingServer()
        {
            await _httpClient.Ping();
        }
    }
}
