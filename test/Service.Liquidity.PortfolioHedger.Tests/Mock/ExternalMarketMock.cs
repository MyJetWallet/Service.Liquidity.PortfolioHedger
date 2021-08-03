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

        public async Task<GetMarketInfoListResponse> GetMarketInfoListAsync(GetMarketInfoListRequest request)
        {
            if (request.ExchangeName == StaticFieldsForTests.ExternalMarket1.ExchangeName)
            {
                return new GetMarketInfoListResponse()
                {
                    Infos = new List<ExchangeMarketInfo>()
                    {
                        {StaticFieldsForTests.ExternalMarket1.MarketInfo}
                    }
                };
            }

            if (request.ExchangeName == StaticFieldsForTests.ExternalMarket2.ExchangeName)
            {
                return new GetMarketInfoListResponse()
                {
                    Infos = new List<ExchangeMarketInfo>()
                    {
                        {StaticFieldsForTests.ExternalMarket2.MarketInfo}
                    }
                };
            }
            return null;
        }

        public async Task<ExchangeTrade> MarketTrade(MarketTradeRequest request)
        {
            if (request.ExchangeName == StaticFieldsForTests.ExternalMarket1.ExchangeName)
            {
                return new ExchangeTrade()
                {
                    Volume = 0.1,
                    OppositeVolume = 0.1 * (double) StaticFieldsForTests.ToVolume
                };
            }

            if (request.ExchangeName == StaticFieldsForTests.ExternalMarket2.ExchangeName)
            {
                return new ExchangeTrade()
                {
                    Volume = 0.15,
                    OppositeVolume = 0.15 * (double) StaticFieldsForTests.ToVolume
                };
            }
            return null;
        }
    }
}