using System.Runtime.Serialization;

namespace Service.Liquidity.PortfolioHedger.Grpc.Models
{
    [DataContract]
    public class ExecuteAutoConvertResponse
    {
        [DataMember(Order = 1)] public bool Success { get; set; }
        [DataMember(Order = 2)] public string ErrorMessage { get; set; }
        [DataMember(Order = 3)] public string FromAsset { get; set; }
        [DataMember(Order = 4)] public decimal ExecutedFromValue { get; set; }
        [DataMember(Order = 5)] public string ToAsset { get; set; }
        [DataMember(Order = 6)] public decimal ExecutedToValue { get; set; }
    }
}