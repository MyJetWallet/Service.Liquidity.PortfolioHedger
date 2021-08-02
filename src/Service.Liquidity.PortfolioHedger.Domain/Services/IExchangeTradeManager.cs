using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
{
    public interface IExchangeTradeManager
    {
        // set return value
        public Task<bool> GetAvailableOrdersAsync(ExternalMarket externalMarket, string fromAsset, string toAsset, decimal fromVolume,
            decimal toVolume);

        // set return and input value
        public bool GetSortedOrderBook(bool availableOrders);

        public ExternalMarketTrade GetTradeByExchange(bool orderBook);
    }
}