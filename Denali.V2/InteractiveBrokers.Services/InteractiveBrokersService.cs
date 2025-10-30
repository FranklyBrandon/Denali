using InteractiveBrokers.Models.Response;
using Microsoft.Extensions.Logging;

namespace InteractiveBrokers.Services
{
    public interface IInteractiveBrokersService
    {
        Task PingServer();
        Task InitializeHttpAuth();
        Task<HistoricAggregateResponse> GetHistoricAggregates(Contract contract, DateTime startDateTime);
        Task<HistoricAggregateResponse> GetHistoricAggregatesBeta(Contract contract, DateTime startDateTime, string period = "1d", string bar = "mins", string barType = "Last", bool outsideRth = false);
        Task<List<Contract>> GetContractsByExchanges(params string[] exchanges);
        Task<List<MarketSnapshot>> GetShortableStatusSnapshot(IEnumerable<int> conIds);
        Task<Accounts> GetAccounts();
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

        public async Task InitializeHttpAuth()
        {
            _logger.LogInformation("=== Initializing IB gateway ===");
            _logger.LogInformation("Initializng brokerage session...");
            await _httpClient.BrokerageInit();
            _logger.LogInformation("Initializng HMDS bridge...");
            await _httpClient.HMDSInit();
            _logger.LogInformation("Pinging gateway for auth status...");
            var ping = await _httpClient.Ping();
            _logger.LogInformation($"Session Id: {ping.session}");
            _logger.LogInformation($"IsServer Status: [authenticated: {ping.iserver.authStatus.authenticated}, connected: {ping.iserver.authStatus.connected}]");
            _logger.LogInformation($"HMDS Bridge Status: [authenticated: {ping.hmds.authStatus.authenticated}, connected: {ping.hmds.authStatus.connected}]");
            _logger.LogInformation("=== Completed IB gateway authentication ===");
        }

        public async Task<HistoricAggregateResponse> GetHistoricAggregates(Contract contract, DateTime startDateTime) =>
            await _httpClient.GetHistoricAggregates(contract, startDateTime);


        public async Task<HistoricAggregateResponse> GetHistoricAggregatesBeta(Contract contract, DateTime startDateTime, string period = "1d", string bar = "mins", string barType = "Last", bool outsideRth = false)
        {
            var result = await _httpClient.GetHistoricAggregatesBeta(contract.conid, startDateTime, period = "1d", bar = "mins", barType = "Last", outsideRth = false);
            result.ticker = contract.ticker;
            return result;
        }

        public async Task<List<Contract>> GetContractsByExchanges(params string[] exchanges)
        {
            List<Contract> contracts = new();
            foreach (var ex in exchanges)
            {
                var cons = await _httpClient.GetAllContractsByExchange(ex);
                contracts.AddRange(cons);
            }

           return contracts.DistinctBy( x => new {x.conid, x.ticker}).ToList();
        }
        public async Task PingServer() => await _httpClient.Ping();

        public async Task<List<MarketSnapshot>> GetShortableStatusSnapshot(IEnumerable<int> conIds) => await _httpClient.GetShortableStatusSnapshot(conIds);

        public async Task<Accounts> GetAccounts() => await _httpClient.GetAccounts();
    }
}
