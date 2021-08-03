using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Services;
using Service.Liquidity.PortfolioHedger.Tests.Mock;

namespace Service.Liquidity.PortfolioHedger.Tests
{
    public class HedgePortfolioManagerTester
    {
        private IHedgePortfolioManager _hedgePortfolioManager;
        private ExchangeTradeManagerMock _exchangeTradeManager;
        private ExchangeTradeWriterMock _exchangeTradeWriter;
        private ExternalExchangeManagerMock _externalExchangeManager;
        private ExternalMarketMock _externalMarket;
        
        [SetUp]
        public void Setup()
        {
            _exchangeTradeManager = new ExchangeTradeManagerMock();
            _exchangeTradeWriter = new ExchangeTradeWriterMock();
            _externalExchangeManager = new ExternalExchangeManagerMock();
            _externalMarket = new ExternalMarketMock();
            
            _hedgePortfolioManager = new HedgePortfolioManager(_exchangeTradeManager, _exchangeTradeWriter,
                _externalExchangeManager, _externalMarket);
        }
        
        [Test]
        public async Task Test1()
        {
            var response = await _hedgePortfolioManager.ExecuteHedgeConvert("testBroker", StaticFieldsForTests.FromAsset,
                StaticFieldsForTests.ToAsset, StaticFieldsForTests.FromVolume, StaticFieldsForTests.ToVolume);
            
            
            Assert.AreEqual(StaticFieldsForTests.FromAsset, response.FromAsset);
            Assert.AreEqual(StaticFieldsForTests.ToAsset, response.ToAsset);
            Assert.AreEqual(StaticFieldsForTests.FromVolume, response.ExecutedFromVolume);
            Assert.AreEqual(StaticFieldsForTests.FromVolume * StaticFieldsForTests.ToVolume, response.ExecutedToVolume);
            
            Console.WriteLine(JsonConvert.SerializeObject(response));
        }
    }
}