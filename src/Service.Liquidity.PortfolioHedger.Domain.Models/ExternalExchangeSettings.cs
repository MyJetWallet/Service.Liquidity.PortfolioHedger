using System.Runtime.Serialization;

namespace Service.Liquidity.PortfolioHedger.Domain.Models
{
    [DataContract]
    public class ExternalExchangeSettings
    {
        [DataMember(Order = 1)] public string ExchangeName { get; set; }
        [DataMember(Order = 2)] public decimal MinBalancePercent { get; set; }
        [DataMember(Order = 3)] public bool IsActive { get; set; }
    }
}