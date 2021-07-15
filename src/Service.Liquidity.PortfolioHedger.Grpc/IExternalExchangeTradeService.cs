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
        Task<CreateManualTradeResponse> CreateManualTradeAsync(CreateManualTradeRequest request);

        [OperationContract]
        Task<GetExternalExchangeCollectionResponse> GetExternalExchangeCollectionAsync();
    }
}