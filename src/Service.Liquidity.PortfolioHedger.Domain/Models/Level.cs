using MyJetWallet.Domain.ExternalMarketApi.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    public class Level
    {
        public string Exchange { get; set; }
        public LeOrderBookLevel OriginalLevel { get; set; }
        public LeOrderBookLevel NormalizeLevel { get; set; }
        
        public bool NormalizeIsOriginal { get; set; }
    }
}