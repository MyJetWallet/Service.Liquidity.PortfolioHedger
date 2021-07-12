using System.Runtime.Serialization;

namespace Service.Liquidity.PortfolioHedger.Grpc.Models
{
    [DataContract]
    public class CreateManualTradeResponse
    {
        [DataMember(Order = 1)] public bool Success { get; set; }
        [DataMember(Order = 2)] public string ErrorMessage { get; set; }
    }
}