using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using NUnit.Framework;

namespace Service.Liquidity.PortfolioHedger.Tests
{
    public class SimulationTester
    {
        [SetUp]
        public void Setup()
        {
        }
        
        //[Test]
        public async Task Test1()
        {
            SetBalance("Binance", "BTC", 5);
            SetBalance("Binance", "USD", 1000000);
            SetBalance("FTX", "BTC", 5);
            SetBalance("FTX", "USD", 1000000);
            
            SetPrices("Binance", "BTC", "USD", 30000);
            SetPrices("Binance", "ETH", "USD", 3000);
            SetPrices("FTX", "BTC", "USD", 30000);
            SetPrices("FTX", "ETH", "USD", 3000);

            SetPortfolio("BTC", -1);
            SetPortfolio("USD", 30100);
            
            // Hedge +1 BTC ExecuteAutoConvert: FROM BTC, FROMVALUE -1
            
            Assert.AreEqual(0, GetPortfolioBalance("BTC"));
            Assert.AreEqual(100, GetPortfolioBalance("USD"));
            
            Assert.AreEqual(6, GetBalance("Binance", "BTC"));
        }

        private double GetPortfolioBalance(string asset)
        {
            throw new System.NotImplementedException();
        }

        private double GetBalance(string exchange, string asset)
        {
            throw new System.NotImplementedException();
        }

        private void SetPortfolio(string asset, decimal volume)
        {
            throw new System.NotImplementedException();
        }

        private void SetPrices(string exchange, string asset, string usd, int p3)
        {
            throw new System.NotImplementedException();
        }

        private void SetBalance(string exchange, string asset, int p2)
        {
            throw new System.NotImplementedException();
        }
    }
}