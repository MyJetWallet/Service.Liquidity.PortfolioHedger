using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
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
        
        public async Task<List<Level>> GetAvailableOrdersAsync(ExternalMarket externalMarket, string fromAsset, string toAsset, decimal fromVolume,
            decimal toVolume)
        {
            var orderbook = await GetOrderBookFromExchangeAsync(externalMarket);
            
            var availableLevels = new List<Level>();
            
            if (externalMarket.MarketInfo.AssociateBaseAsset == fromAsset)
            {
                // тратим BASE ассет пока не потратим FROM
                var balance = await GetAvailableBalance(externalMarket.Exchange, fromAsset);
                var budget = Convert.ToDouble(Math.Min(balance, fromVolume));
                var allLevels = orderbook.Bids.OrderByDescending(e => e.Price).ToList();
                
                for (var i = 0; i < allLevels.Count; i++)
                {
                    var level = allLevels[i];
                    
                    if (budget == 0)
                        break;
                    if (level.Volume <= budget)
                    {
                        budget -= level.Volume;
                        availableLevels.Add(new Level()
                        {
                            OriginalLevel = level,
                            NormalizeLevel = level,
                            NormalizeIsOriginal = true,
                            Exchange = externalMarket.Exchange
                        });
                    } 
                    else 
                    {
                        level.Volume = budget;
                        budget = 0;

                        if (level.Volume >= externalMarket.MarketInfo.MinVolume)
                        {
                            availableLevels.Add(new Level()
                            {
                                OriginalLevel = level,
                                NormalizeLevel = level,
                                NormalizeIsOriginal = true,
                                Exchange = externalMarket.Exchange
                            });
                        }
                    }
                }
            }
            else
            {
                // покупаем BASE ассет пока не купим TO
                
                var balance = await GetAvailableBalance(externalMarket.Exchange, toAsset);
                var budget = Convert.ToDouble(Math.Min(balance, toVolume));
                var allLevels = orderbook.Asks.OrderBy(e => e.Price).ToList();
                
                for (var i = 0; i < allLevels.Count; i++)
                {
                    var level = allLevels[i];
                    
                    if (budget == 0)
                        break;
                    
                    if (Math.Abs(level.Volume) <= budget)
                    {
                        budget += level.Volume;
                        availableLevels.Add(new Level()
                        {
                            OriginalLevel = level,
                            NormalizeLevel = new LeOrderBookLevel()
                            {
                                Price = 1 / level.Price,
                                Volume = -(level.Volume * level.Price)
                            },
                            NormalizeIsOriginal = false,
                            Exchange = externalMarket.Exchange
                        });
                    } 
                    else 
                    {
                        level.Volume = -budget;
                        budget = 0;

                        if (Math.Abs(level.Volume) >= externalMarket.MarketInfo.MinVolume)
                        {
                            availableLevels.Add(new Level()
                            {
                                OriginalLevel = level,
                                NormalizeLevel = new LeOrderBookLevel()
                                {
                                    Price = 1 / level.Price,
                                    Volume = -(level.Volume * level.Price)
                                },
                                NormalizeIsOriginal = false,
                                Exchange = externalMarket.Exchange
                            });
                        }
                    }
                }
            }
            return availableLevels;
        }
        
        private async Task<LeOrderBook> GetOrderBookFromExchangeAsync(ExternalMarket externalMarket)
        {
            var response = await _orderBookSource.GetOrderBookAsync(new MarketRequest()
            {
                ExchangeName = externalMarket.Exchange,
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
            var availableBalance = (balanceByMarket?.Balance ?? 0) * 0.8m; // todo: GO TO NOSQL

            return availableBalance;
        }
    }
}