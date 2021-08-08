using System.Runtime.Serialization;
using Service.Liquidity.Portfolio.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Grpc.Models
{
    [DataContract]
    public class ExecuteAutoConvertRequest
    {
        [DataMember(Order = 1)] public AssetPortfolio PortfolioSnapshot { get; set; }
        [DataMember(Order = 2)] public string FromAsset { get; set; }
        [DataMember(Order = 3)] public decimal FromAssetVolume { get; set; }
    }
}