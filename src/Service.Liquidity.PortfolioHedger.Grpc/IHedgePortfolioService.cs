using System.ServiceModel;
using System.Threading.Tasks;
using Service.Liquidity.Portfolio.Domain.Models;
using Service.Liquidity.PortfolioHedger.Domain.Models;
using Service.Liquidity.PortfolioHedger.Grpc.Models;

namespace Service.Liquidity.PortfolioHedger.Grpc
{
    [ServiceContract]
    public interface IHedgePortfolioService
    {
        [OperationContract]
        Task<GetTradesForHedgeRequest> ExecuteAutoConvert(ExecuteAutoConvertRequest request);

        Task<ExecuteManualConvertResponse> ExecuteManualConvert(ExecuteManualConvertRequest request);
    }
}