using System.Collections.Generic;
using Service.Liquidity.Portfolio.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    public class GetTradesForHedgeRequest
    {
        public List<ExternalMarketTrade> Trades { get; set; }
        public AssetPortfolio PortfolioAfterTrades { get; set; }
    }
}