using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Grpc.Models
{
    [DataContract]
    public class GetExternalExchangeSettingsResponse
    {
        [DataMember(Order = 1)] public bool Success { get; set; }
        [DataMember(Order = 2)] public string ErrorMessage { get; set; }
        [DataMember(Order = 3)] public List<ExternalExchangeSettings> Settings { get; set; }
    }
}