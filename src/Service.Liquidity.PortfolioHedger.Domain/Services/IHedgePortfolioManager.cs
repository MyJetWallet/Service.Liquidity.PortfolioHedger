using System.Collections.Generic;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
{
    public interface IHedgePortfolioManager
    {
        public decimal GetOppositeVolume(string fromAsset, string toAsset, decimal fromVolume);

        public List<string> GetAvailableExternalMarkets(string fromAsset, string toAsset);

        public List<ExternalMarketTrade> GetTradesForExternalMarket(List<string> externalMarkets, string fromAsset, 
            string toAsset, decimal fromVolume, decimal toVolume);

        public List<ExecutedTrade> ExecuteExternalMarketTrades(List<ExternalMarketTrade> externalMarketTrades);

        public ExecutedVolumes GetExecutedVolumesInRequestAssets(List<ExecutedTrade> executedTrades, string fromAsset, string toAsset);
    }
}