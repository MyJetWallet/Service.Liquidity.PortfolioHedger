using System.Threading.Tasks;
using DotNetCoreDecorators;
using MyJetWallet.Domain.ExternalMarketApi.Models;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class ExchangeTradeWriter
    {
        public const string TopicName = "trade-hedger";

        private readonly IPublisher<ExchangeTrade> _publisher;

        public ExchangeTradeWriter(IPublisher<ExchangeTrade> publisher)
        {
            _publisher = publisher;
        }
        

        public async Task PublishTrade(ExchangeTrade trade)
        {
            await _publisher.PublishAsync(trade);
        }
    }
}