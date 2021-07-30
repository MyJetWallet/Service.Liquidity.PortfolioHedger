using System.Collections.Generic;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
{
    public class HedgePortfolioManager : IHedgePortfolioManager
    {
        public decimal GetOppositeVolume(string fromAsset, string toAsset, decimal fromVolume)
        {
            throw new System.NotImplementedException();
        }

        public List<string> GetAvailableExternalMarkets(string fromAsset, string toAsset)
        {
            throw new System.NotImplementedException();
        }

        public List<ExternalMarketTrade> GetTradesForExternalMarket(List<string> externalMarkets, string fromAsset, string toAsset, decimal fromVolume,
            decimal toVolume)
        {
            throw new System.NotImplementedException();
        }

        public ExecuteTradesResult ExecuteExternalMarketTrades(List<ExternalMarketTrade> externalMarketTrades)
        {
            throw new System.NotImplementedException();
        }
    }
}