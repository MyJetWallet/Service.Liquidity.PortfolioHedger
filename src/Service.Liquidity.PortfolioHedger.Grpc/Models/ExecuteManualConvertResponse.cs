using System.Runtime.Serialization;

namespace Service.Liquidity.PortfolioHedger.Grpc.Models
{
    [DataContract]
    public class ExecuteManualConvertResponse
    {
        [DataMember(Order = 1)] public decimal ExecutedFromValue { get; set; }
        [DataMember(Order = 2)] public decimal ExecutedToValue { get; set; }
    }
}