using System.Collections.Generic;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Models;

namespace Service.Liquidity.PortfolioHedger.Tests.Mock
{
    public class ExternalExchangeManagerMock : IExternalExchangeManager
    {
        public async Task<GetExternalExchangeCollectionResponse> GetExternalExchangeCollectionAsync()
        {
            return new GetExternalExchangeCollectionResponse()
            {
                ExchangeNames = new List<string>()
                {
                    {"exchange1"},
                    {"exchange2"}
                }
            };
        }
    }
}