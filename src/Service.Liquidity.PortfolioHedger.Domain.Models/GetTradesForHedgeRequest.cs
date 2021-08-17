using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.Liquidity.Portfolio.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    [DataContract]
    public class GetTradesForHedgeRequest
    {
        [DataMember(Order = 1)] public List<ExternalMarketTrade> Trades { get; set; }
        [DataMember(Order = 2)] public AssetPortfolio PortfolioAfterTrades { get; set; }
    }
}