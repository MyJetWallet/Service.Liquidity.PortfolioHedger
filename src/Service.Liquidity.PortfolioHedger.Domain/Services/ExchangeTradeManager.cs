using System.Collections.Generic;
using System.Linq;
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
            var orderBookLevels = new List<Level>();
            foreach (var externalMarket in externalMarkets)
            {
                var levels = await GetAvailableOrdersAsync(externalMarket, fromAsset, toAsset, fromVolume, toVolume);
                orderBookLevels.AddRange(levels);
            }
            var sortedOrderBook = GetSortedOrderBook(orderBookLevels);
            var ordersToExecute = GetOrdersToExecute(sortedOrderBook, fromVolume);
            var tradesByExchange = GetTradeByExchange(ordersToExecute, externalMarkets);
            
            return tradesByExchange;
        }

        private IEnumerable<Level> GetOrdersToExecute(IReadOnlyList<Level> sortedOrderBook, decimal budget)
        {
            var ordersToExecute = new List<Level>();
            
            for (var i = 0; i < sortedOrderBook.Count; i++)
            {
                var level = sortedOrderBook[i];
                    
                if (budget == 0)
                    break;
                if ((decimal)level.NormalizeLevel.Volume <= budget)
                {
                    budget -= (decimal) level.NormalizeLevel.Volume;
                    ordersToExecute.Add(level);
                } 
                else
                {
                    if (level.NormalizeIsOriginal)
                    {
                        level.NormalizeLevel.Volume = (double) budget;
                        level.OriginalLevel.Volume = (double) budget;
                    }
                    else
                    {
                        level.NormalizeLevel.Volume = (double) budget;
                        level.OriginalLevel.Volume = (double) (budget / (decimal) level.OriginalLevel.Price);
                    }
                    budget = 0;
                    ordersToExecute.Add(level);
                }
            }
            return ordersToExecute;
        }

        private async Task<List<Level>> GetAvailableOrdersAsync(ExternalMarket externalMarket, string fromAsset, string toAsset, decimal fromVolume, decimal toVolume)
        {
            return await _orderBookManager.GetAvailableOrdersAsync(externalMarket, fromAsset, toAsset, fromVolume,
                toVolume);
        }

        private List<Level> GetSortedOrderBook(IEnumerable<Level> availableOrders)
        {
            return availableOrders.OrderByDescending(e => e.NormalizeLevel.Price).ToList();
        }

        private List<ExternalMarketTrade> GetTradeByExchange(IEnumerable<Level> orders, IReadOnlyCollection<ExternalMarket> externalMarkets)
        {
            return orders.GroupBy(e => e.Exchange).Select(e => new ExternalMarketTrade()
            {
                ExchangeName = e.Key,
                Market = externalMarkets.Where(x => x.ExchangeName == e.Key).Select(y=> y.MarketInfo.Market).First(),
                BaseAsset = externalMarkets.Where(x => x.ExchangeName == e.Key).Select(y=> y.MarketInfo.BaseAsset).First(),
                QuoteAsset = externalMarkets.Where(x => x.ExchangeName == e.Key).Select(y=> y.MarketInfo.QuoteAsset).First(),
                BaseVolume = e.Sum(q => (decimal) q.OriginalLevel.Volume)
            }).ToList();
        }
    }
}