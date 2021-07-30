using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
{
    public class ExchangeTradeManager : IExchangeTradeManager
    {
        public bool GetAvailableOrders(string externalMarket, string fromAsset, string toAsset, decimal fromVolume, decimal toVolume)
        {
            // берем ордербук
            var orderbook = GetOrderBookFromExchange(externalMarket, fromAsset, toAsset);

            //обрезаем ордербук по имеющимся балансам
            var sortedByBalance = GetSortedBookByBalance(orderbook);
            
            //обрезаем ордербук по требуемому полному обьему сделки
            var sortedByVolume = GetSortedBookByVolume(sortedByBalance);
            
            return false;
        }

        private object GetSortedBookByVolume(object sortedByBalance)
        {
            throw new System.NotImplementedException();
        }

        private object GetSortedBookByBalance(bool orderbook)
        {
            throw new System.NotImplementedException();
        }

        private bool GetOrderBookFromExchange(string externalMarket, string fromAsset, string toAsset)
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