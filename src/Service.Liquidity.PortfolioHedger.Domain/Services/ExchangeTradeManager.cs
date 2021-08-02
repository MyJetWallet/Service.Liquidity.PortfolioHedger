using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
{
    public class ExchangeTradeManager : IExchangeTradeManager
    {
        private readonly IOrderBookSource _orderBookSource;

        public ExchangeTradeManager(IOrderBookSource orderBookSource)
        {
            _orderBookSource = orderBookSource;
        }

        public async Task<bool> GetAvailableOrdersAsync(ExternalMarket externalMarket, string fromAsset, string toAsset, decimal fromVolume, decimal toVolume)
        {
            // берем ордербук
            var orderbook = await GetOrderBookFromExchangeAsync(externalMarket, fromAsset, toAsset);

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

        private object GetSortedBookByBalance(LeOrderBook orderBook)
        {
            throw new System.NotImplementedException();
        }

        private async Task<LeOrderBook> GetOrderBookFromExchangeAsync(ExternalMarket externalMarket, string fromAsset, string toAsset)
        {
            var response = await _orderBookSource.GetOrderBookAsync(new MarketRequest()
            {
                ExchangeName = externalMarket.Exchange,
                Market = externalMarket.MarketInfo.Market
            });

            return response.OrderBook;
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