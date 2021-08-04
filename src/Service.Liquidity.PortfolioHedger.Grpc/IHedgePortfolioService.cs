using System.ServiceModel;
using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Grpc.Models;

namespace Service.Liquidity.PortfolioHedger.Grpc
{
    [ServiceContract]
    public interface IHedgePortfolioService
    {
        [OperationContract]
        Task<ExecuteManualConvertResponse> ExecuteManualConvert(ExecuteManualConvertRequest request);
        
        [OperationContract]
        Task<ExecuteAutoConvertResponse> ExecuteAutoConvert(ExecuteAutoConvertRequest request);
    }
}