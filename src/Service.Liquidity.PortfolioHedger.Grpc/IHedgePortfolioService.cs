using System.ServiceModel;
using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Grpc.Models;

namespace Service.Liquidity.PortfolioHedger.Grpc
{
    [ServiceContract]
    public interface IHedgePortfolioService
    {
        [OperationContract]
        Task<ExecuteAutoConvertResponse> ExecuteAutoConvert(ExecuteAutoConvertRequest request);
    }
}