using System.Runtime.Serialization;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}