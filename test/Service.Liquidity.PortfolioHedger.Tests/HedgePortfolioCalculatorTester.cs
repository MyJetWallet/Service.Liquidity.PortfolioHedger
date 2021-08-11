using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Service.Liquidity.PortfolioHedger.Tests
{
    public class HedgePortfolioCalculatorTester : TesterBase
    {
        [SetUp]
        public void Setup()
        {
            
        }
        
        //[Test]
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
    
            var portfolioSnapshot = GetPortfolioSnapshot();
    
            FromAsset = "BTC";
            FromVolume = 1m;
            
            var calculationForHedge = await HedgePortfolioCalculator.GetCalculationForHedge(portfolioSnapshot, FromAsset,  FromVolume);
    
            Console.WriteLine(JsonConvert.SerializeObject(calculationForHedge));
            
            Assert.AreEqual(1, calculationForHedge.Trades.FirstOrDefault()?.BaseVolume);
            Assert.AreEqual(-30000, calculationForHedge.Trades.FirstOrDefault().QuoteVolume);
            
            var balance1 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "BTC");
            var balance2 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "USD");
            
            Assert.AreEqual(0, balance1.NetVolume);
            Assert.AreEqual(100, balance2.NetVolume);
        }
        
        //[Test]
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
    
            var portfolioSnapshot = GetPortfolioSnapshot();
    
            FromAsset = "BTC";
            FromVolume = -1m;
            
            var calculationForHedge = await HedgePortfolioCalculator.GetCalculationForHedge(portfolioSnapshot, FromAsset,  FromVolume);
    
            Console.WriteLine(JsonConvert.SerializeObject(calculationForHedge));
            
            Assert.AreEqual(-1, calculationForHedge.Trades.FirstOrDefault()?.BaseVolume);
            Assert.AreEqual(30000, calculationForHedge.Trades.FirstOrDefault()?.QuoteVolume);
            
            var balance1 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "BTC");
            var balance2 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "USD");
            
            Assert.AreEqual(0, balance1.NetVolume);
            Assert.AreEqual(-100, balance2.NetVolume);
        }
        
        //Test]
        public async Task Test3()
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
    
            var portfolioSnapshot = GetPortfolioSnapshot();
    
            FromAsset = "USD";
            FromVolume = 30000m;
            
            var calculationForHedge = await HedgePortfolioCalculator.GetCalculationForHedge(portfolioSnapshot, FromAsset,  FromVolume);
    
            Console.WriteLine(JsonConvert.SerializeObject(calculationForHedge));
            
            Assert.AreEqual(-1, calculationForHedge.Trades.FirstOrDefault()?.BaseVolume);
            Assert.AreEqual(30000, calculationForHedge.Trades.FirstOrDefault()?.QuoteVolume);
            
            var balance1 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "BTC");
            var balance2 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "USD");
            
            Assert.AreEqual(0, balance1.NetVolume);
            Assert.AreEqual(-100, balance2.NetVolume);
        }
        
        //[Test]
        public async Task Test3_()
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
    
            var portfolioSnapshot = GetPortfolioSnapshot();
    
            FromAsset = "USD";
            FromVolume = 30100m;
            
            var calculationForHedge = await HedgePortfolioCalculator.GetCalculationForHedge(portfolioSnapshot, FromAsset,  FromVolume);
    
            Console.WriteLine(JsonConvert.SerializeObject(calculationForHedge));
            
            Assert.AreEqual(-1, calculationForHedge.Trades.FirstOrDefault()?.BaseVolume);
            Assert.AreEqual(30000, calculationForHedge.Trades.FirstOrDefault()?.QuoteVolume);
            
            var balance1 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "BTC");
            var balance2 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "USD");
            
            Assert.AreEqual(0, balance1.NetVolume);
            Assert.AreEqual(-100, balance2.NetVolume);
        }
        
        //[Test]
        public async Task Test4()
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
    
            var portfolioSnapshot = GetPortfolioSnapshot();
    
            FromAsset = "USD";
            FromVolume = -30000m;
            
            var calculationForHedge = await HedgePortfolioCalculator.GetCalculationForHedge(portfolioSnapshot, FromAsset,  FromVolume);
    
            Console.WriteLine(JsonConvert.SerializeObject(calculationForHedge));
            
            Assert.AreEqual(1, calculationForHedge.Trades.FirstOrDefault()?.BaseVolume);
            Assert.AreEqual(-30000, calculationForHedge.Trades.FirstOrDefault()?.QuoteVolume);
            
            var balance1 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "BTC");
            var balance2 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "USD");
            
            Assert.AreEqual(0, balance1.NetVolume);
            Assert.AreEqual(100, balance2.NetVolume);
        }
        
        //[Test]
        public async Task Test4_1()
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
    
            var portfolioSnapshot = GetPortfolioSnapshot();
    
            FromAsset = "USD";
            FromVolume = -30100m;
            
            var calculationForHedge = await HedgePortfolioCalculator.GetCalculationForHedge(portfolioSnapshot, FromAsset,  FromVolume);
    
            Console.WriteLine(JsonConvert.SerializeObject(calculationForHedge));
            
            Assert.AreEqual(1, calculationForHedge.Trades.FirstOrDefault()?.BaseVolume);
            Assert.AreEqual(-30000, calculationForHedge.Trades.FirstOrDefault()?.QuoteVolume);
            
            var balance1 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "BTC");
            var balance2 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "USD");
            
            Assert.AreEqual(0, balance1.NetVolume);
            Assert.AreEqual(100, balance2.NetVolume);
        }
        
        //[Test]
        public async Task Test5()
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
            SetPortfolio("USD", 25000);
    
            var portfolioSnapshot = GetPortfolioSnapshot();
    
            FromAsset = "BTC";
            FromVolume = 1m;
            
            var calculationForHedge = await HedgePortfolioCalculator.GetCalculationForHedge(portfolioSnapshot, FromAsset,  FromVolume);
    
            Console.WriteLine(JsonConvert.SerializeObject(calculationForHedge));
            
            Assert.AreEqual(1, calculationForHedge.Trades.FirstOrDefault()?.BaseVolume);
            Assert.AreEqual(-30000, calculationForHedge.Trades.FirstOrDefault()?.QuoteVolume);
            
            var balance1 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "BTC");
            var balance2 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "USD");
            
            Assert.AreEqual(0, balance1.NetVolume);
            Assert.AreEqual(-5000, balance2.NetVolume);
        }
        
        //[Test]
        public async Task Test5_1()
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
            SetPortfolio("USD", -25000);
    
            var portfolioSnapshot = GetPortfolioSnapshot();
    
            FromAsset = "BTC";
            FromVolume = -1m;
            
            var calculationForHedge = await HedgePortfolioCalculator.GetCalculationForHedge(portfolioSnapshot, FromAsset,  FromVolume);
    
            Console.WriteLine(JsonConvert.SerializeObject(calculationForHedge));
            
            Assert.AreEqual(-1, calculationForHedge.Trades.FirstOrDefault()?.BaseVolume);
            Assert.AreEqual(30000, calculationForHedge.Trades.FirstOrDefault()?.QuoteVolume);
            
            var balance1 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "BTC");
            var balance2 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "USD");
            
            Assert.AreEqual(0, balance1.NetVolume);
            Assert.AreEqual(5000, balance2.NetVolume);
        }
        
        //[Test]
        public async Task Test6_1()
        { 
            SetExchanges(new List<string>(){"Binance", "FTX"});
            
            SetMarketInfos(new Dictionary<string, List<ExchangeMarketInfo>>()
            {{"Binance", new List<ExchangeMarketInfo>()
                {TesterBase.ExternalMarket1.MarketInfo,
                    TesterBase.ExternalMarket2.MarketInfo,
                    TesterBase.ExternalMarket3.MarketInfo
                }},
                {"FTX", new List<ExchangeMarketInfo>()
                {TesterBase.ExternalMarket1.MarketInfo,
                    TesterBase.ExternalMarket2.MarketInfo,
                    TesterBase.ExternalMarket3.MarketInfo
                }}});

            SetBalance("Binance", "BTC", 5);
            SetBalance("Binance", "USD", 1000000);
            SetBalance("Binance", "ETH", 1000000);
            SetBalance("FTX", "BTC", 5);
            SetBalance("FTX", "USD", 1000000);
            SetBalance("FTX", "ETH", 1000000);
            
            SetPrices("Binance", "BTC", "USD", 30000);
            SetPrices("Binance", "ETH", "USD", 3000);
            SetPrices("FTX", "BTC", "USD", 30000);
            SetPrices("FTX", "ETH", "USD", 3000);
            
            SetIndexPrice("BTC", 30000);
            SetIndexPrice("USD", 1);
            SetIndexPrice("ETH", 3000);
            
            SetPortfolio("BTC", 1);
            SetPortfolio("USD", -15000);
            SetPortfolio("ETH", -5);
    
            var portfolioSnapshot = GetPortfolioSnapshot();
    
            FromAsset = "BTC";
            FromVolume = -1m;
            
            var calculationForHedge = await HedgePortfolioCalculator.GetCalculationForHedge(portfolioSnapshot, FromAsset,  FromVolume);
    
            Console.WriteLine(JsonConvert.SerializeObject(calculationForHedge));
            
            //Assert.AreEqual(-1, calculationForHedge.Trades.FirstOrDefault()?.BaseVolume);
            //Assert.AreEqual(30000, calculationForHedge.Trades.FirstOrDefault()?.QuoteVolume);
            
            var balance1 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "BTC");
            var balance2 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "USD");
            var balance3 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "ETH");
            
            Assert.AreEqual(0, balance1.NetVolume);
            Assert.AreEqual(0, balance2.NetVolume);
            Assert.AreEqual(0, balance3.NetVolume);
        }
        
        //Test]
        public async Task Test6_2()
        { 
            SetExchanges(new List<string>(){"Binance", "FTX"});
            
            SetMarketInfos(new Dictionary<string, List<ExchangeMarketInfo>>()
            {{"Binance", new List<ExchangeMarketInfo>()
                {TesterBase.ExternalMarket1.MarketInfo,
                    TesterBase.ExternalMarket2.MarketInfo,
                    TesterBase.ExternalMarket3.MarketInfo
                }},
                {"FTX", new List<ExchangeMarketInfo>()
                {TesterBase.ExternalMarket1.MarketInfo,
                    TesterBase.ExternalMarket2.MarketInfo,
                    TesterBase.ExternalMarket3.MarketInfo
                }}});

            SetBalance("Binance", "BTC", 5);
            SetBalance("Binance", "USD", 1000000);
            SetBalance("Binance", "ETH", 1000000);
            SetBalance("FTX", "BTC", 5);
            SetBalance("FTX", "USD", 1000000);
            SetBalance("FTX", "ETH", 1000000);
            
            SetPrices("Binance", "BTC", "USD", 30000);
            SetPrices("Binance", "ETH", "USD", 3000);
            SetPrices("FTX", "BTC", "USD", 30000);
            SetPrices("FTX", "ETH", "USD", 3000);
            
            SetIndexPrice("BTC", 30000);
            SetIndexPrice("USD", 1);
            SetIndexPrice("ETH", 3000);
            
            SetPortfolio("BTC", 1);
            SetPortfolio("USD", -10000);
            SetPortfolio("ETH", -7.5m);
    
            var portfolioSnapshot = GetPortfolioSnapshot();
    
            FromAsset = "BTC";
            FromVolume = -1m;
            
            var calculationForHedge = await HedgePortfolioCalculator.GetCalculationForHedge(portfolioSnapshot, FromAsset,  FromVolume);
    
            Console.WriteLine(JsonConvert.SerializeObject(calculationForHedge));
            
            //Assert.AreEqual(-1, calculationForHedge.Trades.FirstOrDefault()?.BaseVolume);
            //Assert.AreEqual(30000, calculationForHedge.Trades.FirstOrDefault()?.QuoteVolume);
            
            var balance1 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "BTC");
            var balance2 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "USD");
            var balance3 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "ETH");
            
            Assert.AreEqual(0, balance1.NetVolume);
            Assert.AreEqual(-2500, balance2.NetVolume);
            Assert.AreEqual(0, balance3.NetVolume);
        }
        
        //[Test]
        public async Task Test6_3()
        { 
            SetExchanges(new List<string>(){"Binance", "FTX"});
            
            SetMarketInfos(new Dictionary<string, List<ExchangeMarketInfo>>()
            {{"Binance", new List<ExchangeMarketInfo>()
                {TesterBase.ExternalMarket1.MarketInfo,
                    TesterBase.ExternalMarket2.MarketInfo,
                    TesterBase.ExternalMarket3.MarketInfo
                }},
                {"FTX", new List<ExchangeMarketInfo>()
                {TesterBase.ExternalMarket1.MarketInfo,
                    TesterBase.ExternalMarket2.MarketInfo,
                    TesterBase.ExternalMarket3.MarketInfo,
                    TesterBase.ExternalMarket4.MarketInfo
                }}});

            SetBalance("Binance", "BTC", 5);
            SetBalance("Binance", "USD", 1000000);
            SetBalance("Binance", "ETH", 1000000);
            SetBalance("FTX", "BTC", 5);
            SetBalance("FTX", "USD", 1000000);
            SetBalance("FTX", "ETH", 1000000);
            SetBalance("FTX", "XLM", 0);
            
            SetPrices("Binance", "BTC", "USD", 30000);
            SetPrices("Binance", "ETH", "USD", 3000);
            SetPrices("FTX", "BTC", "USD", 30000);
            SetPrices("FTX", "ETH", "USD", 3000);
            SetPrices("FTX", "XLM", "USD", 2);
            
            SetIndexPrice("BTC", 30000);
            SetIndexPrice("USD", 1);
            SetIndexPrice("ETH", 3000);
            SetIndexPrice("XLM", 2);
            
            SetPortfolio("BTC", 1);
            SetPortfolio("USD", 0);
            SetPortfolio("ETH", -5m);
    
            var portfolioSnapshot = GetPortfolioSnapshot();
    
            FromAsset = "BTC";
            FromVolume = -1m;
            
            var calculationForHedge = await HedgePortfolioCalculator.GetCalculationForHedge(portfolioSnapshot, FromAsset,  FromVolume);
    
            Console.WriteLine(JsonConvert.SerializeObject(calculationForHedge));
            
            //Assert.AreEqual(-1, calculationForHedge.Trades.FirstOrDefault()?.BaseVolume);
            //Assert.AreEqual(30000, calculationForHedge.Trades.FirstOrDefault()?.QuoteVolume);
            
            var balance1 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "BTC");
            var balance2 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "USD");
            var balance3 = calculationForHedge.PortfolioAfterTrades.BalanceByAsset.First(e => e.Asset == "ETH");
            
            //Assert.AreEqual(0, balance1.NetVolume);
            //Assert.AreEqual(15000, balance2.NetVolume);
            //Assert.AreEqual(0, balance3.NetVolume);
        }
    }
}