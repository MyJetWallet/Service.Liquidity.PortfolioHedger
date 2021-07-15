using System.ServiceModel;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using Service.Liquidity.PortfolioHedger.Grpc.Models;

namespace Service.Liquidity.PortfolioHedger.Grpc
{
    [ServiceContract]
    public interface IExternalExchangeTradeService
    {
        [OperationContract]
        Task<ManualTradeResponse> CreateManualTradeAsync(CreateManualTradeRequest request);
        
        [OperationContract]
        Task<ManualTradeResponse> ReportManualTradeAsync(ReportManualTradeRequest request);

        [OperationContract]
        Task<GetExternalExchangeCollectionResponse> GetExternalExchangeCollectionAsync();
    }
}