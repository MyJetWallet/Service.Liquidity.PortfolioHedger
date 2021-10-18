using System.Threading.Tasks;
using MyJetWallet.Sdk.ServiceBus;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
{
    public class ExchangeTradeWriter : IExchangeTradeWriter
    {
        private readonly IServiceBusPublisher<TradeMessage> _publisher;

        public ExchangeTradeWriter(IServiceBusPublisher<TradeMessage> publisher)
        {
            _publisher = publisher;
        }
        
        public async Task PublishTrade(TradeMessage trade)
        {
            await _publisher.PublishAsync(trade);
        }
    }
}