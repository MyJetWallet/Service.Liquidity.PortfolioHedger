using System.Collections.Generic;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
{
    public class ExchangeTradeManager : IExchangeTradeManager
    {
        private readonly IOrderBookManager _orderBookManager;

        public ExchangeTradeManager(IOrderBookManager orderBookManager)
        {
            _orderBookManager = orderBookManager;
        }

        public async Task<List<ExternalMarketTrade>> GetTradesByExternalMarkets(List<ExternalMarket> externalMarkets, string fromAsset, string toAsset, decimal fromVolume, decimal toVolume)
        {
            var orderBookLevels = new List<Level>();
            foreach (var externalMarket in externalMarkets)
            {
                // получить с каждого рынка доступные ордера с учтом баланса и верхней граници сделки
                var levels = await GetAvailableOrdersAsync(externalMarket, fromAsset, toAsset, fromVolume, toVolume);
                orderBookLevels.AddRange(levels);
            }               
            
            // Сортируем агрегированный ордербук по цене
            var sortedOrderBook = GetSortedOrderBook(orderBookLevels);

            // отрезаем по объему 
            var ordersToExecute = GetOrdersToExecute(sortedOrderBook);
            
            // Наберем нужный обьем по агрегированному ордербуку
            var tradesByExchange = GetTradeByExchange(ordersToExecute);
            
            return tradesByExchange;
        }

        private List<Level> GetOrdersToExecute(List<LeOrderBookLevel> sortedOrderBook)
        {
            throw new System.NotImplementedException();
        }

        private async Task<List<Level>> GetAvailableOrdersAsync(ExternalMarket externalMarket, string fromAsset, string toAsset, decimal fromVolume, decimal toVolume)
        {
            return await _orderBookManager.GetAvailableOrdersAsync(externalMarket, fromAsset, toAsset, fromVolume,
                toVolume);
        }

        private List<LeOrderBookLevel> GetSortedOrderBook(List<Level> availableOrders)
        {
            throw new System.NotImplementedException();
        }

        private List<ExternalMarketTrade> GetTradeByExchange(List<Level> orders)
        {
            throw new System.NotImplementedException();
        }
    }
}