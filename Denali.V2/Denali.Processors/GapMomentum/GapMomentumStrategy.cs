using Denali.Models;
using Denali.TechnicalAnalysis;

namespace Denali.Processors.GapMomentum
{
    public class GapMomentumStrategy
    {
        private readonly GapMomentumSettings _settings;
        private readonly Gap _gap;
        private IAggregateBar _previousDailyBar;
        private decimal _latestPreMarketPrice;
        public GapMomentumTrade Trade;

        public GapMomentumStrategy(GapMomentumSettings settings)
        {
            _settings = settings;
            _gap = new Gap(_settings.FullGap);
        }

        public void SetPreviousDailyBar(IAggregateBar previousDailyBar) =>
            _previousDailyBar = previousDailyBar;

        public void OnPremarketTick(decimal price) =>
            _latestPreMarketPrice = price;

        public GapMomentumTrade EvaluateTrade()
        {
            var bar = new AggregateBar(_latestPreMarketPrice);

            if (_gap.IsGapUp(bar, _previousDailyBar))
                return Trade = new GapMomentumTrade(MarketAction.Trade, direction: 1);          

            if (_gap.IsGapDown(bar, _previousDailyBar))
                return Trade = new GapMomentumTrade(MarketAction.Trade, direction: -1);

            return new GapMomentumTrade(MarketAction.None);
        }

        public BrokerAction OnTick(decimal price)
        {
            if (!Trade.HighWaterMet && OverPriceLimit(price, Trade.EntryPrice, _settings.HighWaterMark, Trade.direction))
            {
                Trade.HighWaterMet = true;
                var stopLimitPrice = Trade.EntryPrice + (_settings.HighWaterTakeProfit * Trade.direction);
                return new BrokerAction(MarketAction.Trade, Trade.direction > 0 ? MarketSide.Sell : MarketSide.Buy, orderType: OrderType.StopLimit, price: stopLimitPrice);
            }

            if (PastStop(price, Trade.EntryPrice, _settings.StopLossMark, Trade.direction))
                return new BrokerAction(MarketAction.Trade, Trade.direction > 0 ? MarketSide.Sell : MarketSide.Buy, OrderType.Market);

            return new BrokerAction(MarketAction.None);
        }

        public bool OverPriceLimit(decimal price, decimal entryPrice, decimal priceDifference, int direction) =>
            price * direction >= (entryPrice + (priceDifference * direction)) * direction;

        public bool UnderPriceLimit(decimal price, decimal entryPrice, decimal priceDifference, int direction) =>
            price * direction <= (entryPrice + (priceDifference * direction)) * direction;

        public bool PastStop(decimal price, decimal entryPrice, decimal priceDifference, int direction) =>
            price * direction * -1 >= (entryPrice - (priceDifference * direction)) * direction * -1;
    }
}
