using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using Service.Liquidity.PortfolioHedger.Grpc.Models;

namespace Service.Liquidity.PortfolioHedger.Grpc
{
    [ServiceContract]
    public interface IExternalExchangeTradeService
    {
        [OperationContract]
        Task<CreateManualTradeResponse> CreateManualTradeAsync(MarketTradeRequest request);

        [OperationContract]
        Task<GetExternalExchangeCollectionResponse> GetExternalExchangeCollectionAsync();
    }
}