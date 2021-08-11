using System.Runtime.Serialization;
using Service.Liquidity.Portfolio.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Grpc.Models
{
    [DataContract]
    public class ExecuteAutoConvertRequest
    {
        [DataMember(Order = 1)] public AssetPortfolio PortfolioSnapshot { get; set; }
    }
}