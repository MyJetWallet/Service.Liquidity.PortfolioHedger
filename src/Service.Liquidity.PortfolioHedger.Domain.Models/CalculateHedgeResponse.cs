using System.Collections.Generic;
using Service.Liquidity.Portfolio.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    public class CalculateHedgeResponse
    {
        public List<ExternalMarketTrade> Trades { get; set; }
        public AssetPortfolio NewPortfolio { get; set; }
    }
}