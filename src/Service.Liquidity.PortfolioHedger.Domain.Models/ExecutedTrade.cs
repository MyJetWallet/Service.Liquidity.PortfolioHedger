namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    public class ExecutedTrade
    {
        public ExternalMarketTrade Trade { get; set; }
        public decimal ExecutedBaseVolume { get; set; }
        public decimal ExecutedQuoteVolume { get; set; }
    }
}