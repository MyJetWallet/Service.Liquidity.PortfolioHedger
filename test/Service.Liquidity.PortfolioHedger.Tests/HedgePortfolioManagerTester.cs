using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Services;
using Service.Liquidity.PortfolioHedger.Tests.Mock;

namespace Service.Liquidity.PortfolioHedger.Tests
{
    public class HedgePortfolioManagerTester : TesterBase
    {
        private IHedgePortfolioManager _hedgePortfolioManager;
        
        [SetUp]
        public void Setup()
        {
            IndexPricesClientMock = new IndexPricesClientMock();
            ExchangeTradeManager = new ExchangeTradeManagerMock();
            ExchangeTradeWriter = new ExchangeTradeWriterMock();
            ExternalExchangeManager = new ExternalExchangeManagerMock();
            ExternalMarket = new ExternalMarketMock(IndexPricesClientMock);
            
            _hedgePortfolioManager = new HedgePortfolioManager(ExchangeTradeManager, ExchangeTradeWriter,
                ExternalExchangeManager, ExternalMarket);
        }
        
        [Test]
        public async Task Test1()
        {
            SetExchanges(new List<string>(){"exchange1", "exchange2"});
            SetIndexPrice("BTC", 30000);
            SetIndexPrice("USD", 1);
            
            var trades = await _hedgePortfolioManager.GetTradesForExternalMarketAsync(TesterBase.FromAsset, 
                TesterBase.ToAsset, TesterBase.FromVolume, TesterBase.ToVolume);

            var executedVolumes = await _hedgePortfolioManager.ExecuteExternalMarketTrades(trades, TesterBase.FromAsset,
                TesterBase.ToAsset, "testBroker");


            Assert.AreEqual(TesterBase.FromAsset, executedVolumes.FromAsset);
            Assert.AreEqual(TesterBase.ToAsset, executedVolumes.ToAsset);
            Assert.AreEqual(TesterBase.FromVolume, executedVolumes.ExecutedFromVolume);
            
            Console.WriteLine(JsonConvert.SerializeObject(executedVolumes));
        }
    }
}