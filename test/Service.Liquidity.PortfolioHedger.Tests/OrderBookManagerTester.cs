using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private OrderBookSourceMock _orderBookSource;
        private ExternalMarketMock _externalMarket;
        
        [SetUp]
        public void Setup()
        {
            _orderBookSource = new OrderBookSourceMock()
            {
                OrderBooks = new Dictionary<string, List<LeOrderBook>>()
                {
                    {
                        "exchange1", new List<LeOrderBook>()
                        { new LeOrderBook()
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
            var orders = await _orderBookManager.GetAvailableOrdersAsync(StaticFieldsForTests.ExternalMarket1, StaticFieldsForTests.FromAsset, StaticFieldsForTests.ToAsset, StaticFieldsForTests.FromVolume, StaticFieldsForTests.ToVolume);
            
            Assert.AreEqual(StaticFieldsForTests.FromVolume, orders.Sum(e => e.NormalizeLevel.Volume));
            
            var balance = _externalMarket.Balances[StaticFieldsForTests.ExternalMarket1.ExchangeName]
                .First(e => e.Symbol == StaticFieldsForTests.ExternalMarket1.MarketInfo.BaseAsset).Balance;

            Assert.IsTrue((balance * 0.8m) > (decimal) orders.Sum(e => e.NormalizeLevel.Volume));
            
            foreach (var order in orders)
            {
                Console.WriteLine(JsonConvert.SerializeObject(order));
            }
        }
        
        [Test]
        public async Task Test2()
        {
            _externalMarket.Balances = new Dictionary<string, List<ExchangeBalance>>()
            {
                {
                    "exchange1", new List<ExchangeBalance>()
                    {
                        new ExchangeBalance()
                        {
                            Symbol = "BTC",
                            Balance = 0.1m
                        },
                        new ExchangeBalance()
                        {
                            Symbol = "USD",
                            Balance = 1000000
                        }
                    }
                }
            };

            var orders = await _orderBookManager.GetAvailableOrdersAsync(StaticFieldsForTests.ExternalMarket1, StaticFieldsForTests.FromAsset, StaticFieldsForTests.ToAsset, StaticFieldsForTests.FromVolume, StaticFieldsForTests.ToVolume);

            var balance = _externalMarket.Balances[StaticFieldsForTests.ExternalMarket1.ExchangeName]
                .First(e => e.Symbol == StaticFieldsForTests.ExternalMarket1.MarketInfo.BaseAsset).Balance;
            
            Assert.AreEqual(balance * 0.8m, orders.Sum(e => e.NormalizeLevel.Volume));

            foreach (var order in orders)
            {
                Console.WriteLine(JsonConvert.SerializeObject(order));
            }
        }
    }
}