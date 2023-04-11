using Denali.Models;
using Denali.TechnicalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.GapMomentum
{
    public class GapMomentumStrategy
    {
        private readonly GapMomentumSettings _settings;
        private readonly Gap _gap;
        private IAggregateBar _previousDailyBar;
        private decimal _latestPreMarketPrice;
        private BrokerAction _entry;
        private short _direction = 0;
        private bool _highWater = false;

        public GapMomentumStrategy(GapMomentumSettings settings)
        {
            _settings = settings;
            _gap = new Gap(_settings.FullGap);
        }

        public void SetPreviousDailyBar(IAggregateBar previousDailyBar) =>
            _previousDailyBar = previousDailyBar;

        public void OnPremarketTick(decimal price) =>
            _latestPreMarketPrice = price;

        public BrokerAction EvaluateTrade()
        {
            var bar = new AggregateBar(_latestPreMarketPrice);
            if (_gap.IsGapUp(bar, _previousDailyBar))
            {
                _direction = 1;
                return new BrokerAction(MarketAction.Trade, MarketSide.Buy, OrderType.Market);

            }

            if (_gap.IsGapDown(bar, _previousDailyBar))
            {
                _direction = -1;
                return new BrokerAction(MarketAction.Trade, MarketSide.Sell, OrderType.Market);
            }

            return new BrokerAction(MarketAction.None);
        }

        public void SetEntry(BrokerAction entry) =>
            _entry = entry;

        public BrokerAction OnTick(decimal price)
        {
            if (!_highWater && ((price + _settings.HighWaterMark) * _direction >= (_entry.price + _settings.HighWaterMark) * _direction))
            {
                _highWater = true;
                var stopLimitPrice = _entry.price + (_settings.HighWaterTakeProfit * _direction);
                return new BrokerAction(MarketAction.Trade, _entry.Close(), orderType: OrderType.StopLimit, price: stopLimitPrice);
            }

            if (price <= _entry.price + (_settings.StopLossMark * Math.Sign(_direction) * -1))
                return new BrokerAction(MarketAction.Trade, _entry.Close(), OrderType.Market);

            return new BrokerAction(MarketAction.None);
        }
    }
}
