using System.Collections.Generic;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using MyJetWallet.Domain.ExternalMarketApi.Models;

namespace Service.Liquidity.PortfolioHedger.Tests.Mock
{
    public class ExternalMarketMock : IExternalMarket
    {
        public Dictionary<string, List<ExchangeBalance>> Balances { get; set; }
        
        public Task<GetNameResult> GetNameAsync(GetNameRequest request)
        {
            throw new System.NotImplementedException();
        }

        public async Task<GetBalancesResponse> GetBalancesAsync(GetBalancesRequest request)
        {
            if (Balances.TryGetValue(request.ExchangeName, out var value))
            {
                return new GetBalancesResponse()
                {
                    Balances = value
                };
            }
            return null;
        }

        public Task<GetMarketInfoResponse> GetMarketInfoAsync(MarketRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<GetMarketInfoListResponse> GetMarketInfoListAsync(GetMarketInfoListRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExchangeTrade> MarketTrade(MarketTradeRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}