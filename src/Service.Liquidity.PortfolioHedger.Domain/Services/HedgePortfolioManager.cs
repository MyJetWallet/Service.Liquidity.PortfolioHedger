using System.Collections.Generic;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
{
    public class HedgePortfolioManager : IHedgePortfolioManager
    {
        private readonly IExchangeTradeManager _exchangeTradeManager;
        private readonly ExchangeTradeWriter _exchangeTradeWriter;
        
        public HedgePortfolioManager(IExchangeTradeManager exchangeTradeManager,
            ExchangeTradeWriter exchangeTradeWriter)
        {
            _exchangeTradeManager = exchangeTradeManager;
            _exchangeTradeWriter = exchangeTradeWriter;
        }

        public decimal GetOppositeVolume(string fromAsset, string toAsset, decimal fromVolume)
        {
            throw new System.NotImplementedException();
        }

        public List<string> GetAvailableExternalMarkets(string fromAsset, string toAsset)
        {
            throw new System.NotImplementedException();
        }

        public List<ExternalMarketTrade> GetTradesForExternalMarket(List<string> externalMarkets, string fromAsset, string toAsset, decimal fromVolume,
            decimal toVolume)
        {
            var tradesByExchanges = new List<ExternalMarketTrade>();
            foreach (var externalMarket in externalMarkets)
            {
                // получить с каждого рынка доступные ордера с учтом баланса и верхней граници сделки
                var availableOrders = _exchangeTradeManager.GetAvailableOrders(externalMarket, fromAsset, toAsset, fromVolume, toVolume);

                // Сортируем агрегированный ордербук по цене
                var sortedOrderBook = _exchangeTradeManager.GetSortedOrderBook(availableOrders);

                // Наберем нужный обьем по агрегированному ордербуку
                var tradeByExchange = _exchangeTradeManager.GetTradeByExchange(sortedOrderBook);
                
                tradesByExchanges.Add(tradeByExchange);
            }
            return tradesByExchanges;
        }

        public List<ExecutedTrade> ExecuteExternalMarketTrades(List<ExternalMarketTrade> externalMarketTrades)
        {
            
            //await _exchangeTradeWriter.PublishTrade(exchangeTradeMessage);
            
            throw new System.NotImplementedException();
        }

        public ExecutedVolumes GetExecutedVolumesInRequestAssets(List<ExecutedTrade> executedTrades, string fromAsset, string toAsset)
        {
            throw new System.NotImplementedException();
        }
    }
}