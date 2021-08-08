using System.Collections.Generic;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Dto;

namespace Service.Liquidity.PortfolioHedger.Tests.Mock
{
    public class ExternalExchangeManagerMock : IExternalExchangeManager
    {
        public List<string> ExchangeNames = new List<string>();
        
        public async Task<GetExternalExchangeCollectionResponse> GetExternalExchangeCollectionAsync()
        {
            return new GetExternalExchangeCollectionResponse()
            {
                ExchangeNames = ExchangeNames
            };
        }
    }
}