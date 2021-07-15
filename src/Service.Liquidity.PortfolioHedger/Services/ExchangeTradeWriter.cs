using System.Threading.Tasks;
using DotNetCoreDecorators;
using Service.Liquidity.PortfolioHedger.ServiceBus;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class ExchangeTradeWriter
    {
        private readonly IPublisher<TradeMessage> _publisher;

        public ExchangeTradeWriter(IPublisher<TradeMessage> publisher)
        {
            _publisher = publisher;
        }
        
        public async Task PublishTrade(TradeMessage trade)
        {
            await _publisher.PublishAsync(trade);
        }
    }
}