using MyJetWallet.Domain.Orders;

namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    public class ExternalMarketTrade
    {
        public string Exchange { get; set; }
        public string Market { get; set; }
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
        public decimal BaseVolume { get; set; }
        public decimal QuoteVolume { get; set; }
        public OrderSide Side { get; set; }
    }
}