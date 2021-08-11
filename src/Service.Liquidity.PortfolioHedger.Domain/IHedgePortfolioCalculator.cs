using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Liquidity.Portfolio.Domain.Models;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain
{
    public interface IHedgePortfolioCalculator
    {
        public Task<GetTradesForHedgeRequest> GetCalculationForHedge(AssetPortfolio portfolioSnapshot, string fromAsset, decimal fromAssetVolume);
    }
}