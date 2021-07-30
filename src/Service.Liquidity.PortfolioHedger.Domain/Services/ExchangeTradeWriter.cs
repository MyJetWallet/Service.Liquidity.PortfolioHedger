using System.Threading.Tasks;
using DotNetCoreDecorators;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
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