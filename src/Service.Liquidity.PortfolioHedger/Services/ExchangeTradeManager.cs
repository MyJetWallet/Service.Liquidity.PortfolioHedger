using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class ExchangeTradeManager : IExchangeTradeManager
    {
        private readonly IOrderBookManager _orderBookManager;
        private readonly IExternalExchangeManager _externalExchangeManager;
        private readonly IExternalMarket _externalMarket;

        public ExchangeTradeManager(IOrderBookManager orderBookManager,
            IExternalExchangeManager externalExchangeManager,
            IExternalMarket externalMarket)
        {
            _orderBookManager = orderBookManager;
            _externalExchangeManager = externalExchangeManager;
            _externalMarket = externalMarket;
        }

        public async Task<List<ExternalMarketTrade>> GetTradesByExternalMarkets(string fromAsset, string toAsset, decimal fromVolume, decimal toVolume)
        {
            
            // TODO: добавить в LEVEL Market + направление сделки, дальше при сортировке и наборе агрегированного ордербука корректно сложить уровни
            
            var externalMarkets = await GetAvailableExchangesAsync(fromAsset, toAsset);
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
            return orders.GroupBy(e => e.Exchange).Select(e =>
            {
                var baseVolume = 0m;
                var quoteVolume = 0m;
                
                if (e.First().NormalizeIsOriginal)
                {
                    baseVolume = e.Sum(q => (decimal) q.OriginalLevel.Volume);
                    quoteVolume = baseVolume > 0
                        ? -(decimal) Math.Abs(e.Sum(x => x.OriginalLevel.Price * x.OriginalLevel.Volume) / e.Count())
                        : (decimal) Math.Abs(e.Sum(x => x.OriginalLevel.Price * x.OriginalLevel.Volume) / e.Count());
                }
                else
                {
                    baseVolume = e.First().NormalizeLevel.Volume > 0
                        ? -Math.Abs(e.Sum(q => (decimal) q.OriginalLevel.Volume))
                        : Math.Abs(e.Sum(q => (decimal) q.OriginalLevel.Volume));
                    quoteVolume = baseVolume > 0
                        ? -(decimal) Math.Abs(e.Sum(x => x.OriginalLevel.Price * x.OriginalLevel.Volume) / e.Count())
                        : (decimal) Math.Abs(e.Sum(x => x.OriginalLevel.Price * x.OriginalLevel.Volume) / e.Count());
                }

                return new ExternalMarketTrade()
                {
                    ExchangeName = e.Key,
                    Market = externalMarkets.Where(x => x.ExchangeName == e.Key).Select(y => y.MarketInfo.Market)
                        .First(),
                    BaseAsset = externalMarkets.Where(x => x.ExchangeName == e.Key).Select(y => y.MarketInfo.BaseAsset)
                        .First(),
                    QuoteAsset = externalMarkets.Where(x => x.ExchangeName == e.Key)
                        .Select(y => y.MarketInfo.QuoteAsset).First(),
                    BaseVolume = baseVolume,
                    QuoteVolume = quoteVolume
                };
            }).ToList();
        }
        
        private async Task<List<ExternalMarket>> GetAvailableExchangesAsync(string fromAsset, string toAsset)
        {
            var exchanges = (await _externalExchangeManager.GetExternalExchangeCollectionAsync()).ExchangeNames;

            var availableExchanges = new List<ExternalMarket>();
            
            foreach (var exchange in exchanges)
            {
                var exchangeMarkets = await _externalMarket.GetMarketInfoListAsync(new GetMarketInfoListRequest()
                {
                    ExchangeName = exchange
                });

                var exchangeMarketInfo = exchangeMarkets.Infos
                    .FirstOrDefault(e => (e.AssociateBaseAsset == fromAsset && e.AssociateQuoteAsset == toAsset) ||
                                         (e.AssociateBaseAsset == toAsset && e.AssociateQuoteAsset == fromAsset));
                
                if (exchangeMarketInfo != null)
                    availableExchanges.Add(new ExternalMarket()
                    {
                        ExchangeName = exchange,
                        MarketInfo = exchangeMarketInfo
                    });
            }
            return availableExchanges;
        }
    }
}