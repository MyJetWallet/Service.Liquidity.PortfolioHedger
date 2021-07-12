using System.Runtime.Serialization;

namespace Service.Liquidity.PortfolioHedger.Grpc.Models
{
    [DataContract]
    public class CreateManualTradeRequest
    {
        [DataMember(Order = 1)] public string ExchangeName { get; set; }
        [DataMember(Order = 2)] public string InstrumentSymbol { get; set; }
        [DataMember(Order = 3)] public double BaseVolume { get; set; }
    }
}