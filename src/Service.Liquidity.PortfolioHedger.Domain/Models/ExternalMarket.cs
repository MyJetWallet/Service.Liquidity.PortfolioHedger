using MyJetWallet.Domain.ExternalMarketApi.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    public class ExternalMarket
    {
        public string ExchangeName { get; set; }
        public ExchangeMarketInfo MarketInfo { get; set; }
    }
}