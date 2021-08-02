using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
{
    public interface IHedgePortfolioManager
    {
        public decimal GetOppositeVolume(string fromAsset, string toAsset, decimal fromVolume);

        public Task<List<ExternalMarket>> GetAvailableExchangesAsync(string fromAsset, string toAsset);

        public Task<List<ExternalMarketTrade>> GetTradesForExternalMarketAsync(List<ExternalMarket> externalMarkets, string fromAsset, 
            string toAsset, decimal fromVolume, decimal toVolume);

        public List<ExecutedTrade> ExecuteExternalMarketTrades(List<ExternalMarketTrade> externalMarketTrades);

        public ExecutedVolumes GetExecutedVolumesInRequestAssets(List<ExecutedTrade> executedTrades, string fromAsset, string toAsset);
    }
}