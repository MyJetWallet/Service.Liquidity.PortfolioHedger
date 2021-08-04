using System.ServiceModel;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using Service.Liquidity.PortfolioHedger.Domain.Models;
using Service.Liquidity.PortfolioHedger.Grpc.Models;

namespace Service.Liquidity.PortfolioHedger.Grpc
{
    [ServiceContract]
    public interface IManualTradeService
    {
        [OperationContract]
        Task<ManualTradeResponse> CreateManualTradeAsync(CreateManualTradeRequest request);
        
        [OperationContract]
        Task<ManualTradeResponse> ReportManualTradeAsync(ReportManualTradeRequest request);

        [OperationContract]
        Task<GetExternalExchangeCollectionResponse> GetExternalExchangeCollectionAsync();
    }
}