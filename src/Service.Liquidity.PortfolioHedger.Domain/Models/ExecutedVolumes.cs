namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    public class ExecutedVolumes
    {
        public string FromAsset { get; set; }
        public string ToAsset { get; set; }
        public decimal ExecutedFromVolume { get; set; }
        public decimal ExecutedToVolume { get; set; }
    }
}