using System.Runtime.Serialization;

namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    [DataContract]
    public class ReportManualTradeRequest
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string WalletName { get; set; }
        [DataMember(Order = 3)] public string Symbol { get; set; }
        [DataMember(Order = 4)] public double Price { get; set; }
        [DataMember(Order = 5)] public double BaseVolume { get; set; }
        [DataMember(Order = 6)] public double QuoteVolume { get; set; }
        [DataMember(Order = 7)] public string Comment { get; set; }
        [DataMember(Order = 8)] public string User { get; set; }
    }
}