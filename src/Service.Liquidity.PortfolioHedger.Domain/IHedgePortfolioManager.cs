using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain
{
    public interface IHedgePortfolioManager
    {
        public Task<List<ExternalMarketTrade>> GetTradesForExternalMarketAsync(string fromAsset, string toAsset,
            decimal fromVolume, decimal toVolume);

        public Task<ExecutedVolumes> ExecuteExternalMarketTrades(
            IEnumerable<ExternalMarketTrade> externalMarketTrades, string fromAsset, string toAsset, string brokerId);
    }
}