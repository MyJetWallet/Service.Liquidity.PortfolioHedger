using System.ServiceModel;
using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Domain.Models;
using Service.Liquidity.PortfolioHedger.Grpc.Models;

namespace Service.Liquidity.PortfolioHedger.Grpc
{
    [ServiceContract]
    public interface IExternalExchangeSettingsService
    {
        [OperationContract]
        Task<GetExternalExchangeSettingsResponse> GetExternalExchangeSettingsAsync();
        
        [OperationContract]
        Task UpdateExternalExchangeSettingsAsync(ExternalExchangeSettings settings);
    }
}