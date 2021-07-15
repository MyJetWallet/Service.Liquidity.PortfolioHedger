using System.Runtime.Serialization;

namespace Service.Liquidity.PortfolioHedger.Grpc.Models
{
    [DataContract]
    public class CreateManualTradeRequest
    {
        [DataMember(Order = 1)] public string AssociateBrokerId { get; set; }
        [DataMember(Order = 2)] public string BaseAsset { get; set; }
        [DataMember(Order = 3)] public string QuoteAsset { get; set; }
        [DataMember(Order = 4)] public string ExchangeName { get; set; }
        [DataMember(Order = 5)] public string Market { get; set; }
        [DataMember(Order = 6)] public double BaseVolume { get; set; }
        [DataMember(Order = 7)] public string Comment { get; set; }
        [DataMember(Order = 8)] public string User { get; set; }
    }
}