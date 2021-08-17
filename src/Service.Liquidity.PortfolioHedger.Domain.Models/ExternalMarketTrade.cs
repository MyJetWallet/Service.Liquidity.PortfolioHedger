using System.Runtime.Serialization;

namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    [DataContract]
    public class ExternalMarketTrade
    {
        [DataMember(Order = 1)] public string ExchangeName { get; set; }
        [DataMember(Order = 2)] public string Market { get; set; }
        [DataMember(Order = 3)] public string BaseAsset { get; set; }
        [DataMember(Order = 4)] public string QuoteAsset { get; set; }
        [DataMember(Order = 5)] public decimal BaseVolume { get; set; }
        [DataMember(Order = 6)] public decimal QuoteVolume { get; set; }
    }
}