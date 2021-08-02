using MyJetWallet.Domain.Orders;

namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    public class ExternalMarketTrade
    {
        public string ExchangeName { get; set; }
        public string Market { get; set; }
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
        public decimal BaseVolume { get; set; }
    }
}