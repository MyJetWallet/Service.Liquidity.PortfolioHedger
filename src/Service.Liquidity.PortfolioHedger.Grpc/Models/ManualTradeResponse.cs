using System.Runtime.Serialization;

namespace Service.Liquidity.PortfolioHedger.Grpc.Models
{
    public class ManualTradeResponse
    {
        [DataMember(Order = 1)] public bool Success { get; set; }
        [DataMember(Order = 2)] public string ErrorMessage { get; set; }
    }
}