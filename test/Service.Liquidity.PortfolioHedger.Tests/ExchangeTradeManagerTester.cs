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
                {StaticFieldsForTests.ExternalMarket} 
            };
            var trades = await _exchangeTradeManager.GetTradesByExternalMarkets(externalMarkets, StaticFieldsForTests.FromAsset, StaticFieldsForTests.ToAsset, StaticFieldsForTests.FromVolume, StaticFieldsForTests.ToVolume);

            Assert.AreEqual(StaticFieldsForTests.ExternalMarket.Exchange, trades.Select(e => e.ExchangeName).First());
            Assert.AreEqual(StaticFieldsForTests.ExternalMarket.MarketInfo.Market, trades.Select(e => e.Market).First());
            Assert.AreEqual(StaticFieldsForTests.ExternalMarket.MarketInfo.BaseAsset, trades.Select(e => e.BaseAsset).First());
            Assert.AreEqual(StaticFieldsForTests.ExternalMarket.MarketInfo.QuoteAsset, trades.Select(e => e.QuoteAsset).First());
            Assert.AreEqual(StaticFieldsForTests.FromVolume, trades.Sum(e => e.BaseVolume));
            
            foreach (var trade in trades)
            {
                Console.WriteLine(JsonConvert.SerializeObject(trade));
            }
        }
    }
}