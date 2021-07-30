using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
{
    public class ExchangeTradeManager : IExchangeTradeManager
    {
        public bool GetAvailableOrders(string externalMarket, string fromAsset, string toAsset, decimal fromVolume, decimal toVolume)
        {
            throw new System.NotImplementedException();
        }

        public bool GetSortedOrderBook(bool availableOrders)
        {
            throw new System.NotImplementedException();
        }

        public ExternalMarketTrade GetTradeByExchange(bool orderBook)
        {
            throw new System.NotImplementedException();
        }
    }
}