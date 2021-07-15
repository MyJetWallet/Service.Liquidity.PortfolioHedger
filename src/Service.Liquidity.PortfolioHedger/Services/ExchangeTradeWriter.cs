using System.Threading.Tasks;
using DotNetCoreDecorators;
using Service.Liquidity.PortfolioHedger.ServiceBus;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class ExchangeTradeWriter
    {
        private readonly IPublisher<ExchangeTradeMessage> _publisher;

        public ExchangeTradeWriter(IPublisher<ExchangeTradeMessage> publisher)
        {
            _publisher = publisher;
        }
        
        public async Task PublishTrade(ExchangeTradeMessage trade)
        {
            await _publisher.PublishAsync(trade);
        }
    }
}