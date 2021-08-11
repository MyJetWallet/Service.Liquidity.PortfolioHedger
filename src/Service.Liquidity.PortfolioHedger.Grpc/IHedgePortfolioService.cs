using System.ServiceModel;
using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Grpc.Models;

namespace Service.Liquidity.PortfolioHedger.Grpc
{
    [ServiceContract]
    public interface IHedgePortfolioService
    {
        [OperationContract]
        Task ExecuteAutoConvert(ExecuteAutoConvertRequest request);

        Task<ExecuteManualConvertResponse> ExecuteManualConvert(ExecuteManualConvertRequest request);
    }
}