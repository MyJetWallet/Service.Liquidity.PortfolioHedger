using System.Collections.Generic;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using MyJetWallet.Domain.ExternalMarketApi.Models;

namespace Service.Liquidity.PortfolioHedger.Tests.Mock
{
    public class OrderBookSourceMock : IOrderBookSource
    {
        public Dictionary<string, LeOrderBook> OrderBooks { get; set; }


        public Task<GetNameResult> GetNameAsync(GetOrderBookNameRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<GetSymbolResponse> GetSymbolsAsync(GetSymbolsRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<HasSymbolResponse> HasSymbolAsync(MarketRequest request)
        {
            throw new System.NotImplementedException();
        }

        public async Task<GetOrderBookResponse> GetOrderBookAsync(MarketRequest request)
        {
            if (OrderBooks.TryGetValue(request.ExchangeName, out var value))
            {
                return new GetOrderBookResponse()
                {
                    OrderBook = value
                };
            }
            return null;
        }
    }
}