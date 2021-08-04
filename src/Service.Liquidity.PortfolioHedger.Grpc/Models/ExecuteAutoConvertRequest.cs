using System.Runtime.Serialization;

namespace Service.Liquidity.PortfolioHedger.Grpc.Models
{
    [DataContract]
    public class ExecuteAutoConvertRequest
    {
        [DataMember(Order = 1)] public string FromAsset { get; set; }
        [DataMember(Order = 2)] public decimal FromAssetVolume { get; set; }
    }
}