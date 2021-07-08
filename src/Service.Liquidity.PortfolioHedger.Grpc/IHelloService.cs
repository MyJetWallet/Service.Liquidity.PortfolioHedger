using System.ServiceModel;
using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Grpc.Models;

namespace Service.Liquidity.PortfolioHedger.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}