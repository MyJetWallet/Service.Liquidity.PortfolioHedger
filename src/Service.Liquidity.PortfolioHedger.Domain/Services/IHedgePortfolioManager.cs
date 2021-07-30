using System.Collections.Generic;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
{
    public interface IHedgePortfolioManager
    {
        public decimal GetOppositeVolume(string fromAsset, string toAsset, decimal fromVolume);

        public List<string> GetAvailableExternalMarkets(string fromAsset, string toAsset);

        public List<ExternalMarketTrade> GetTradesForExternalMarket(List<string> externalMarkets, string fromAsset, 
            string toAsset, decimal fromVolume, decimal toVolume);

        public ExecuteTradesResult ExecuteExternalMarketTrades(List<ExternalMarketTrade> externalMarketTrades);
    }

    public class ExecuteTradesResult
    {
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
        public decimal ExecutedBaseVolume { get; set; }
        public decimal ExecutedQuoteVolume { get; set; }
    }

    public class ExternalMarketTrade
    {
        public string Exchange { get; set; }
        public string Market { get; set; }
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
        public decimal BaseVolume { get; set; }
        public decimal QuoteVolume { get; set; }
    }
}