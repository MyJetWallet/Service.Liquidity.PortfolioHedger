using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Models;
using Service.Liquidity.PortfolioHedger.Domain.Services;
using Service.Liquidity.PortfolioHedger.Tests.Mock;

namespace Service.Liquidity.PortfolioHedger.Tests
{
    public class ExchangeTradeManagerTester
    {
        private IExchangeTradeManager _exchangeTradeManager;
        private OrderBookManagerMock _orderBookManagerMock;
        
        [SetUp]
        public void Setup()
        {
            _orderBookManagerMock = new OrderBookManagerMock();
            _exchangeTradeManager = new ExchangeTradeManager(_orderBookManagerMock);
        }
        
        [Test]
        public async Task Test1()
        {
            var externalMarkets = new List<ExternalMarket>()
            {
                {StaticFieldsForTests.ExternalMarket1} 
            };
            var trades = await _exchangeTradeManager.GetTradesByExternalMarkets(externalMarkets, StaticFieldsForTests.FromAsset, StaticFieldsForTests.ToAsset, StaticFieldsForTests.FromVolume, StaticFieldsForTests.ToVolume);

            Assert.AreEqual(1, trades.Count);
            
            var trade = trades.First();
            
            Assert.AreEqual(StaticFieldsForTests.ExternalMarket1.ExchangeName, trade.ExchangeName);
            Assert.AreEqual(StaticFieldsForTests.ExternalMarket1.MarketInfo.Market, trade.Market);
            Assert.AreEqual(StaticFieldsForTests.ExternalMarket1.MarketInfo.BaseAsset, trade.BaseAsset);
            Assert.AreEqual(StaticFieldsForTests.ExternalMarket1.MarketInfo.QuoteAsset, trade.QuoteAsset);
            Assert.AreEqual(StaticFieldsForTests.FromVolume, trade.BaseVolume);
            
            foreach (var e in trades)
            {
                Console.WriteLine(JsonConvert.SerializeObject(e));
            }
        }
        
        [Test]
        public async Task Test2()
        {
            var externalMarkets = new List<ExternalMarket>()
            {
                {StaticFieldsForTests.ExternalMarket1},
                {StaticFieldsForTests.ExternalMarket2}
            };
            var trades = await _exchangeTradeManager.GetTradesByExternalMarkets(externalMarkets, StaticFieldsForTests.FromAsset, StaticFieldsForTests.ToAsset, StaticFieldsForTests.FromVolume, StaticFieldsForTests.ToVolume);

            Assert.AreEqual(2, trades.Count);
            
            var trade1 = trades.First(e => e.ExchangeName == StaticFieldsForTests.ExternalMarket1.ExchangeName);

            var volumeFromFirstExchange = 0.1m;
            Assert.AreEqual(StaticFieldsForTests.ExternalMarket1.ExchangeName, trade1.ExchangeName);
            Assert.AreEqual(StaticFieldsForTests.ExternalMarket1.MarketInfo.Market, trade1.Market);
            Assert.AreEqual(StaticFieldsForTests.ExternalMarket1.MarketInfo.BaseAsset, trade1.BaseAsset);
            Assert.AreEqual(StaticFieldsForTests.ExternalMarket1.MarketInfo.QuoteAsset, trade1.QuoteAsset);
            Assert.AreEqual(volumeFromFirstExchange, trade1.BaseVolume);
            
            var trade2 = trades.First(e => e.ExchangeName == StaticFieldsForTests.ExternalMarket2.ExchangeName);
            
            Assert.AreEqual(StaticFieldsForTests.ExternalMarket2.ExchangeName, trade2.ExchangeName);
            Assert.AreEqual(StaticFieldsForTests.ExternalMarket2.MarketInfo.Market, trade2.Market);
            Assert.AreEqual(StaticFieldsForTests.ExternalMarket2.MarketInfo.BaseAsset, trade2.BaseAsset);
            Assert.AreEqual(StaticFieldsForTests.ExternalMarket2.MarketInfo.QuoteAsset, trade2.QuoteAsset);
            Assert.AreEqual(StaticFieldsForTests.FromVolume - volumeFromFirstExchange, trade2.BaseVolume);
            
            
            foreach (var e in trades)
            {
                Console.WriteLine(JsonConvert.SerializeObject(e));
            }
        }
    }
}