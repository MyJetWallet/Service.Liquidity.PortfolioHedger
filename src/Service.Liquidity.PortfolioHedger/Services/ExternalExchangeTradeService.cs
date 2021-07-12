using System;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using Service.Liquidity.PortfolioHedger.Grpc;
using Service.Liquidity.PortfolioHedger.Grpc.Models;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class ExternalExchangeTradeService : IExternalExchangeTradeService
    {
        private readonly IExternalExchangeManager _externalExchangeManager;
        private readonly IExternalMarket _externalMarket;

        public ExternalExchangeTradeService(IExternalExchangeManager externalExchangeManager,
            IExternalMarket externalMarket)
        {
            _externalExchangeManager = externalExchangeManager;
            _externalMarket = externalMarket;
        }

        public async Task<CreateManualTradeResponse> CreateManualTradeAsync(MarketTradeRequest request)
        {
            try
            {
                await _externalMarket.MarketTrade(request);
            }
            catch (Exception exception)
            {
                return new CreateManualTradeResponse()
                {
                    Success = false,
                    ErrorMessage = exception.Message
                };
            }
            return new CreateManualTradeResponse()
            {
                Success = true
            };
        }

        public async Task<GetExternalExchangeCollectionResponse> GetExternalExchangeCollectionAsync()
        {
            return await _externalExchangeManager.GetExternalExchangeCollectionAsync();
        }
    }
}