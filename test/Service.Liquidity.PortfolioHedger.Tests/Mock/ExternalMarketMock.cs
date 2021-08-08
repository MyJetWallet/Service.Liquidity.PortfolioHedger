using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using MyJetWallet.Domain.ExternalMarketApi.Models;

namespace Service.Liquidity.PortfolioHedger.Tests.Mock
{
    public class ExternalMarketMock : IExternalMarket
    {
        private readonly IndexPricesClientMock _indexPricesClientMock;

        public ExternalMarketMock(IndexPricesClientMock indexPricesClientMock)
        {
            _indexPricesClientMock = indexPricesClientMock;
        }

        public Dictionary<string, List<ExchangeBalance>> Balances { get; set; }
        public Dictionary<string, List<ExchangeMarketInfo>> MarketInfos { get; set; }
        
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
            if (request.ExchangeName == TesterBase.ExternalMarket1.ExchangeName)
            {
                return new GetMarketInfoListResponse()
                {
                    Infos = new List<ExchangeMarketInfo>()
                    {
                        {TesterBase.ExternalMarket1.MarketInfo}
                    }
                };
            }

            if (request.ExchangeName == TesterBase.ExternalMarket2.ExchangeName)
            {
                return new GetMarketInfoListResponse()
                {
                    Infos = new List<ExchangeMarketInfo>()
                    {
                        {TesterBase.ExternalMarket2.MarketInfo}
                    }
                };
            }
            if (MarketInfos.TryGetValue(request.ExchangeName, out var value))
            {
                return new GetMarketInfoListResponse()
                {
                    Infos = value
                };
            }
            return null;
        }

        public async Task<ExchangeTrade> MarketTrade(MarketTradeRequest request)
        {
            var oppositeVolumeAbs = Math.Abs((double) GetOppositeVolume(request.Market.Substring(0, 3),
                request.Market.Substring(3, 3), (decimal) request.Volume));
            return new ExchangeTrade()
            {
                Volume = request.Volume,
                OppositeVolume = request.Volume > 0 
                    ? -oppositeVolumeAbs
                    : request.Volume
            };
        }
        
        private decimal GetOppositeVolume(string fromAsset, string toAsset, decimal fromVolume)
        {
            var fromAssetPrice = _indexPricesClientMock.GetIndexPriceByAssetAsync(fromAsset);
            var toAssetPrice = _indexPricesClientMock.GetIndexPriceByAssetAsync(toAsset);
            
            var toVolume = fromVolume * (fromAssetPrice.UsdPrice / toAssetPrice.UsdPrice);
            return toVolume;
        }
    }
}