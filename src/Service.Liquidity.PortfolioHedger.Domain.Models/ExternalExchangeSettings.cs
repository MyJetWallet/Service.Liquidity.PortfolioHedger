namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    public class ExternalExchangeSettings
    {
        public string ExchangeName { get; set; }
        public string Asset { get; set; }
        public decimal MinBalancePercent { get; set; }
        public bool IsActive { get; set; }
    }
}