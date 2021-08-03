using MyJetWallet.Domain.ExternalMarketApi.Models;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Tests
{
    public class StaticFieldsForTests
    {
        public static ExternalMarket ExternalMarket1 = new ExternalMarket()
        {
            ExchangeName = "exchange1",
            MarketInfo = new ExchangeMarketInfo()
            {
                AssociateBaseAsset = "BTC",
                AssociateInstrument = "BTCUSD",
                AssociateQuoteAsset = "USD",
                BaseAsset = "BTC",
                QuoteAsset = "USD",
                Market = "BTCUSD",
                MinVolume = 0.01,
                PriceAccuracy = 2,
                VolumeAccuracy = 8
            }
        };
        public static ExternalMarket ExternalMarket2 = new ExternalMarket()
        {
            ExchangeName = "exchange2",
            MarketInfo = new ExchangeMarketInfo()
            {
                AssociateBaseAsset = "BTC",
                AssociateInstrument = "BTCUSD",
                AssociateQuoteAsset = "USD",
                BaseAsset = "BTC",
                QuoteAsset = "USD",
                Market = "BTCUSD",
                MinVolume = 0.01,
                PriceAccuracy = 2,
                VolumeAccuracy = 8
            }
        };
        public static string FromAsset = "BTC";
        public static string ToAsset = "USD";
        public static decimal FromVolume = 0.25m;
        public static decimal ToVolume = 40000m;
    }
}