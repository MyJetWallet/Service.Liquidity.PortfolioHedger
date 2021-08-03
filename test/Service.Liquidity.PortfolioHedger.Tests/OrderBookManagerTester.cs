using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Models;
using Service.Liquidity.PortfolioHedger.Domain.Services;
using Service.Liquidity.PortfolioHedger.Tests.Mock;

namespace Service.Liquidity.PortfolioHedger.Tests
{
    public class OrderBookManagerTester
    {
        private IOrderBookManager _orderBookManager;
        private IOrderBookSource _orderBookSource;
        private IExternalMarket _externalMarket;
        
        [SetUp]
        public void Setup()
        {
            _orderBookSource = new OrderBookSourceMock()
            {
                OrderBooks = new Dictionary<string, LeOrderBook>()
                {
                    {
                        "exchange1", new LeOrderBook()
                        {
                            Symbol = "BTCUSD",
                            Asks = new List<LeOrderBookLevel>()
                            {
                                new LeOrderBookLevel(41000, 0.1d),
                                new LeOrderBookLevel(42000, 0.1d),
                                new LeOrderBookLevel(43000, 0.1d)
                            },
                            Bids = new List<LeOrderBookLevel>()
                            {
                                new LeOrderBookLevel(39000, 0.1d),
                                new LeOrderBookLevel(38000, 0.1d),
                                new LeOrderBookLevel(37000, 0.1d)
                            }
                        }
                    }
                }
            };
            _externalMarket = new ExternalMarketMock()
            {
                Balances = new Dictionary<string, List<ExchangeBalance>>()
                {
                    {
                        "exchange1", new List<ExchangeBalance>()
                        {
                            new ExchangeBalance()
                            {
                                Symbol = "BTC",
                                Balance = 10
                            },
                            new ExchangeBalance()
                            {
                                Symbol = "USD",
                                Balance = 1000000
                            }
                        }
                    }
                }
            };
            _orderBookManager = new OrderBookManager(_orderBookSource, _externalMarket);
        }
        
        [Test]
        public async Task Test1()
        {
            var externalMarket = new ExternalMarket()
            {
                Exchange = "exchange1",
                MarketInfo = new ExchangeMarketInfo()
                {
                    AssociateBaseAsset = "BTC",
                    AssociateInstrument = "BTCUSD",
                    AssociateQuoteAsset = "USD",
                    BaseAsset = "BTC",
                    QuoteAsset = "USD",
                    Market = "BTCUSD",
                    MinVolume = 0.01,
                    PriceAccuracy = 2,
                    VolumeAccuracy = 8
                }
            };
            var fromAsset = "BTC";
            var toAsset = "USD";
            var fromVolume = 0.25m;
            var toVolume = 40000m;

            var orders = await _orderBookManager.GetAvailableOrdersAsync(externalMarket, fromAsset, toAsset, fromVolume, toVolume);
            
            Assert.AreEqual(fromVolume, orders.Sum(e => e.NormalizeLevel.Volume));
            
            var balances = await _externalMarket.GetBalancesAsync(new GetBalancesRequest()
            {
                ExchangeName = externalMarket.Exchange
            });
            var balanceByMarket = balances.Balances.FirstOrDefault(e => e.Symbol == fromAsset);

            Assert.IsTrue((balanceByMarket.Balance * 0.8m) > (decimal) orders.Sum(e => e.NormalizeLevel.Volume));
            
            foreach (var order in orders)
            {
                Console.WriteLine(JsonConvert.SerializeObject(order));
            }
        }
    }
}