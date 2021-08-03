using System.Collections.Generic;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Tests.Mock
{
    public class OrderBookManagerMock : IOrderBookManager
    {
        public async Task<List<Level>> GetAvailableOrdersAsync(ExternalMarket externalMarket, string fromAsset, string toAsset, decimal fromVolume,
            decimal toVolume)
        {
            if (externalMarket.Exchange == "exchange1" && externalMarket.MarketInfo.Market == "BTCUSD")
            {
                return new List<Level>()
                {
                    new Level()
                    {
                        Exchange = "exchange1",
                        OriginalLevel = new LeOrderBookLevel()
                        {
                            Price = 39000,
                            Volume = 0.1
                        },
                        NormalizeLevel = new LeOrderBookLevel()
                        {
                            Price = 39000,
                            Volume = 0.1
                        },
                        NormalizeIsOriginal = true
                    },
                    new Level()
                    {
                        Exchange = "exchange1",
                        OriginalLevel = new LeOrderBookLevel()
                        {
                            Price = 38000,
                            Volume = 0.1
                        },
                        NormalizeLevel = new LeOrderBookLevel()
                        {
                            Price = 38000,
                            Volume = 0.1
                        },
                        NormalizeIsOriginal = true
                    },
                    new Level()
                    {
                        Exchange = "exchange1",
                        OriginalLevel = new LeOrderBookLevel()
                        {
                            Price = 37000,
                            Volume = 0.05
                        },
                        NormalizeLevel = new LeOrderBookLevel()
                        {
                            Price = 37000,
                            Volume = 0.05
                        },
                        NormalizeIsOriginal = true
                    }
                };
            }
            return null;
        }
    }
}