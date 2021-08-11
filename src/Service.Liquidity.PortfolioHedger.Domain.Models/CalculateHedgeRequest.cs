using Service.Liquidity.Portfolio.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    public class CalculateHedgeRequest
    {
        public AssetPortfolio PortfolioSnapshot { get; set; }
        public string FromAsset { get; set; }
        public string ToAsset { get; set; }
        public decimal FromVolume { get; set; }
        public decimal ToVolume { get; set; }
    }
}