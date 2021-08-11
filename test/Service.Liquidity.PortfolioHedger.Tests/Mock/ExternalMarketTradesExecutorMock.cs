using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Tests.Mock
{
    public class ExternalMarketTradesExecutorMock : IExternalMarketTradesExecutor
    {
        public Task<ExecutedVolumes> ExecuteExternalMarketTrades(IEnumerable<ExternalMarketTrade> externalMarketTrades, string fromAsset, string brokerId)
        {
            throw new System.NotImplementedException();
        }
    }
}