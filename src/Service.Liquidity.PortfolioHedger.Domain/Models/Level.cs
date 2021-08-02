using MyJetWallet.Domain.ExternalMarketApi.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    public class Level
    {
        public LeOrderBookLevel OriginalLevel { get; set; }
        public LeOrderBookLevel NormalizeLevel { get; set; }
    }
}