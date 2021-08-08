namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    public class AvailableOppositeAsset
    {
        public int Order { get; set; }
        public string Asset { get; set; }
        public decimal Volume { get; set; }
    }
}