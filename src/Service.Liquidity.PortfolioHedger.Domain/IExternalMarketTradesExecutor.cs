using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain
{
    public interface IExternalMarketTradesExecutor
    {
        public Task<ExecutedVolumes> ExecuteExternalMarketTrades(IEnumerable<ExternalMarketTrade> externalMarketTrades, string fromAsset, string brokerId);
    }
}