using System.Collections.Generic;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using NUnit.Framework;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Services;
using Service.Liquidity.PortfolioHedger.Grpc.Models;
using Service.Liquidity.PortfolioHedger.Services;
using Service.Liquidity.PortfolioHedger.Services.Grpc;
using Service.Liquidity.PortfolioHedger.Tests.Mock;

namespace Service.Liquidity.PortfolioHedger.Tests
{
    public class SimulationTester : TesterBase
    {
        private HedgePortfolioService _hedgePortfolioService;
        
        [SetUp]
        public void Setup()
        {
            _hedgePortfolioService = new HedgePortfolioService(HedgePortfolioCalculator, PortfolioHandler, ExternalMarketTradesExecutor);
        }

        [Test]
        public async Task Test1()
        {
            SetExchanges(new List<string>(){"Binance", "FTX"});

            SetMarketInfos(new Dictionary<string, List<ExchangeMarketInfo>>()
            {{"Binance", new List<ExchangeMarketInfo>()
                {TesterBase.ExternalMarket1.MarketInfo,
                    TesterBase.ExternalMarket2.MarketInfo}},
                {"FTX", new List<ExchangeMarketInfo>()
                {TesterBase.ExternalMarket1.MarketInfo,
                    TesterBase.ExternalMarket2.MarketInfo}}});
            
            SetBalance("Binance", "BTC", 5);
            SetBalance("Binance", "USD", 1000000);
            SetBalance("FTX", "BTC", 5);
            SetBalance("FTX", "USD", 1000000);
            
            SetPrices("Binance", "BTC", "USD", 30000);
            SetPrices("Binance", "ETH", "USD", 3000);
            SetPrices("FTX", "BTC", "USD", 30000);
            SetPrices("FTX", "ETH", "USD", 3000);

            SetIndexPrice("BTC", 30000);
            SetIndexPrice("USD", 1);
            
            SetPortfolio("BTC", -1);
            SetPortfolio("USD", 30100);
            
            // Hedge +1 BTC ExecuteAutoConvert: FROM BTC, FROMVALUE -1
            await _hedgePortfolioService.ExecuteAutoConvert(new ExecuteAutoConvertRequest()
            {
                PortfolioSnapshot = GetPortfolioSnapshot()
            });
            
            Assert.AreEqual(0, GetPortfolioBalance("BTC"));
            Assert.AreEqual(100, GetPortfolioBalance("USD"));
            
            //Assert.AreEqual(6, GetBalance("Binance", "BTC"));
        }
    }
}