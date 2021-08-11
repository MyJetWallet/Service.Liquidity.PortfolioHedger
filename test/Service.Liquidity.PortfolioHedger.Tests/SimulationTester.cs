using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private ILoggerFactory _loggerFactory;
        
        [SetUp]
        public void Setup()
        {
            _loggerFactory =
                LoggerFactory.Create(builder =>
                    builder.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "hh:mm:ss ";
                    }));
            _hedgePortfolioService = new HedgePortfolioService(HedgePortfolioCalculator, PortfolioHandler, ExternalMarketTradesExecutor, _loggerFactory.CreateLogger<HedgePortfolioService>());
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
            var newPortfolio = await _hedgePortfolioService.ExecuteAutoConvert(new ExecuteAutoConvertRequest()
            {
                PortfolioSnapshot = GetPortfolioSnapshot()
            });

            var balance1 = newPortfolio.BalanceByAsset.First(e => e.Asset == "BTC");
            var balance2 = newPortfolio.BalanceByAsset.First(e => e.Asset == "USD");

            Assert.AreEqual(99.9999999999m, balance1.NetUsdVolume);
            Assert.AreEqual(0, balance2.NetVolume);
        }
        
        [Test]
        public async Task Test2()
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
            
            SetPortfolio("BTC", 1);
            SetPortfolio("USD", -30100);
            
            // Hedge +1 BTC ExecuteAutoConvert: FROM BTC, FROMVALUE -1
            var newPortfolio = await _hedgePortfolioService.ExecuteAutoConvert(new ExecuteAutoConvertRequest()
            {
                PortfolioSnapshot = GetPortfolioSnapshot()
            });

            var balance1 = newPortfolio.BalanceByAsset.First(e => e.Asset == "BTC");
            var balance2 = newPortfolio.BalanceByAsset.First(e => e.Asset == "USD");

            Assert.AreEqual(-99.9999999999m, balance1.NetUsdVolume);
            Assert.AreEqual(0, balance2.NetVolume);
        }
    }
}