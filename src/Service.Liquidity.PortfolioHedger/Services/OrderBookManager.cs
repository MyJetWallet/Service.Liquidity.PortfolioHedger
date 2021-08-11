using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class OrderBookManager : IOrderBookManager
    {
        private readonly IOrderBookSource _orderBookSource;
        private readonly IExternalMarket _externalMarket;

        public OrderBookManager(IOrderBookSource orderBookSource,
            IExternalMarket externalMarket)
        {
            _orderBookSource = orderBookSource;
            _externalMarket = externalMarket;
        }
        
        public async Task<List<Level>> GetAvailableOrdersAsync(ExternalMarket externalMarket, string fromAsset, string toAsset,
            decimal fromVolume, decimal toVolume)
        {
            var orderbook = await GetOrderBookFromExchangeAsync(externalMarket);
            
            var availableLevels = new List<Level>();
            
            if (externalMarket.MarketInfo.AssociateBaseAsset == fromAsset && fromVolume < 0)
            {
                var balance = await GetAvailableBalance(externalMarket.ExchangeName, fromAsset);

                var budgetBase = Math.Min(balance, Math.Abs(fromVolume));

                if (budgetBase <= 0)
                {
                    return availableLevels;
                }
                
                var budgetQuote = toVolume;
                
                if (budgetQuote <= 0)
                {
                    return availableLevels;
                }
                
                var allLevels = orderbook.Bids.OrderByDescending(e => e.Price).ToList();
                
                for (var i = 0; i < allLevels.Count; i++)
                {
                    var level = allLevels[i];

                    var levelVolume = (decimal) level.Volume;
                    var levelPrice = (decimal) level.Price;
                    
                    if (budgetBase == 0 || budgetQuote == 0)
                        break;

                    if (levelVolume > budgetBase)
                    {
                        levelVolume = budgetBase;
                    }
                    if (levelVolume * levelPrice > budgetQuote)
                    {
                        levelVolume = budgetQuote / levelPrice;
                    }
                    if (levelVolume < (decimal) externalMarket.MarketInfo.MinVolume)
                    {
                        levelVolume = 0;
                    }
                    if (levelVolume > 0)
                    {
                        budgetBase -= levelVolume;
                        budgetQuote -= levelVolume * levelPrice;

                        level.Volume = (double) levelVolume;
                        
                        availableLevels.Add(new Level()
                        {
                            OriginalLevel = level,
                            NormalizeLevel = level,
                            NormalizeIsOriginal = true,
                            Exchange = externalMarket.ExchangeName
                        });
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (externalMarket.MarketInfo.AssociateBaseAsset == fromAsset && fromVolume > 0)
            {
                var balance = await GetAvailableBalance(externalMarket.ExchangeName, toAsset);
                
                var budgetBase = fromVolume;

                if (budgetBase <= 0)
                {
                    return availableLevels;
                }
                
                var budgetQuote = Math.Min(balance, Math.Abs(toVolume));
                
                if (budgetQuote <= 0)
                {
                    return availableLevels;
                }
                
                var allLevels = orderbook.Asks.OrderBy(e => e.Price).ToList();
                
                for (var i = 0; i < allLevels.Count; i++)
                {
                    var level = allLevels[i];

                    var levelVolume = (decimal) level.Volume;
                    var levelPrice = (decimal) level.Price;
                    
                    if (budgetBase == 0 || budgetQuote == 0)
                        break;

                    if (levelVolume > budgetBase)
                    {
                        levelVolume = budgetBase;
                    }
                    if (levelVolume * levelPrice > budgetQuote)
                    {
                        levelVolume = budgetQuote / levelPrice;
                    }
                    if (levelVolume < (decimal) externalMarket.MarketInfo.MinVolume)
                    {
                        levelVolume = 0;
                    }
                    if (levelVolume > 0)
                    {
                        budgetBase -= levelVolume;
                        budgetQuote -= levelVolume * levelPrice;

                        level.Volume = (double) levelVolume;
                        
                        availableLevels.Add(new Level()
                        {
                            OriginalLevel = level,
                            NormalizeLevel = level,
                            NormalizeIsOriginal = true,
                            Exchange = externalMarket.ExchangeName
                        });
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (externalMarket.MarketInfo.AssociateBaseAsset == toAsset && toVolume < 0)
            {
                var balance = await GetAvailableBalance(externalMarket.ExchangeName, toAsset);
                
                var budgetBase = Math.Min(balance, Math.Abs(toVolume));

                if (budgetBase <= 0)
                {
                    return availableLevels;
                }
                
                var budgetQuote = fromVolume;
                
                if (budgetQuote <= 0)
                {
                    return availableLevels;
                }
                
                var allLevels = orderbook.Bids.OrderByDescending(e => e.Price).ToList();
                
                for (var i = 0; i < allLevels.Count; i++)
                {
                    var level = allLevels[i];

                    var levelVolume = (decimal) level.Volume;
                    var levelPrice = (decimal) level.Price;
                    
                    if (budgetBase == 0 || budgetQuote == 0)
                        break;

                    if (levelVolume > budgetBase)
                    {
                        levelVolume = budgetBase;
                    }
                    if (levelVolume * levelPrice > budgetQuote)
                    {
                        levelVolume = budgetQuote / levelPrice;
                    }
                    if (levelVolume < (decimal) externalMarket.MarketInfo.MinVolume)
                    {
                        levelVolume = 0;
                    }
                    if (levelVolume > 0)
                    {
                        budgetBase -= levelVolume;
                        budgetQuote -= levelVolume * levelPrice;

                        level.Volume = (double) levelVolume;
                        
                        availableLevels.Add(new Level()
                        {
                            OriginalLevel = level,
                            NormalizeLevel = level,
                            NormalizeIsOriginal = true,
                            Exchange = externalMarket.ExchangeName
                        });
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (externalMarket.MarketInfo.AssociateBaseAsset == toAsset && toVolume > 0)
            {
                var balance = await GetAvailableBalance(externalMarket.ExchangeName, fromAsset);
                
                var budgetBase = toVolume;

                if (budgetBase <= 0)
                {
                    return availableLevels;
                }
                
                var budgetQuote = Math.Min(balance, Math.Abs(fromVolume));
                
                if (budgetQuote <= 0)
                {
                    return availableLevels;
                }

                var allLevels = orderbook.Asks.OrderBy(e => e.Price).ToList();
                
                for (var i = 0; i < allLevels.Count; i++)
                {
                    var level = allLevels[i];

                    var levelVolume = (decimal) level.Volume;
                    var levelPrice = (decimal) level.Price;
                    
                    if (budgetBase == 0 || budgetQuote == 0)
                        break;

                    if (levelVolume > budgetBase)
                    {
                        levelVolume = budgetBase;
                    }
                    if (levelVolume * levelPrice > budgetQuote)
                    {
                        levelVolume = budgetQuote / levelPrice;
                    }
                    if (levelVolume < (decimal) externalMarket.MarketInfo.MinVolume)
                    {
                        levelVolume = 0;
                    }
                    if (levelVolume > 0)
                    {
                        budgetBase -= levelVolume;
                        budgetQuote -= levelVolume * levelPrice;

                        level.Volume = (double) levelVolume;
                        
                        availableLevels.Add(new Level()
                        {
                            OriginalLevel = level,
                            NormalizeLevel = level,
                            NormalizeIsOriginal = true,
                            Exchange = externalMarket.ExchangeName
                        });
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return availableLevels;
        }
        
        private async Task<LeOrderBook> GetOrderBookFromExchangeAsync(ExternalMarket externalMarket)
        {
            var response = await _orderBookSource.GetOrderBookAsync(new MarketRequest()
            {
                ExchangeName = externalMarket.ExchangeName,
                Market = externalMarket.MarketInfo.Market
            });

            return response.OrderBook;
        }
        
        private async Task<decimal> GetAvailableBalance(string exchange, string asset)
        {
            var balances = await _externalMarket.GetBalancesAsync(new GetBalancesRequest()
            {
                ExchangeName = exchange
            });
            var balanceByMarket = balances.Balances.FirstOrDefault(e => e.Symbol == asset);
            var availableBalance = (balanceByMarket?.Free ?? 0) * 0.8m; // todo: GO TO NOSQL

            return availableBalance;
        }
    }
}