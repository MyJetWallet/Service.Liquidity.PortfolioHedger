using System.Collections.Generic;
using System.Threading.Tasks;
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
            var tradesByExchanges = new List<ExternalMarketTrade>();
            foreach (var externalMarket in externalMarkets)
            {
                // получить с каждого рынка доступные ордера с учтом баланса и верхней граници сделки
                var availableOrders = await GetAvailableOrdersAsync(externalMarket, fromAsset, toAsset, fromVolume, toVolume);

                // Сортируем агрегированный ордербук по цене
                var sortedOrderBook = GetSortedOrderBook(availableOrders);

                // Наберем нужный обьем по агрегированному ордербуку
                var tradeByExchange = GetTradeByExchange(sortedOrderBook);
                
                tradesByExchanges.Add(tradeByExchange);
            }
            return tradesByExchanges;
        }
        
        private async Task<bool> GetAvailableOrdersAsync(ExternalMarket externalMarket, string fromAsset, string toAsset, decimal fromVolume, decimal toVolume)
        {
            return await _orderBookManager.GetAvailableOrdersAsync(externalMarket, fromAsset, toAsset, fromVolume,
                toVolume);

        }

        private bool GetSortedOrderBook(bool availableOrders)
        {
            throw new System.NotImplementedException();
        }

        private ExternalMarketTrade GetTradeByExchange(bool orderBook)
        {
            throw new System.NotImplementedException();
        }
    }
}