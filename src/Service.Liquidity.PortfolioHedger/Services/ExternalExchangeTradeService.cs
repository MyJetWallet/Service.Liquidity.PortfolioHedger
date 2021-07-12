using System;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using MyJetWallet.Domain.Orders;
using Service.Liquidity.PortfolioHedger.Grpc;
using Service.Liquidity.PortfolioHedger.Grpc.Models;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class ExternalExchangeTradeService : IExternalExchangeTradeService
    {
        private readonly IExternalExchangeManager _externalExchangeManager;
        private readonly IExternalMarket _externalMarket;
        private readonly ExchangeTradeWriter _exchangeTradeWriter;

        public ExternalExchangeTradeService(IExternalExchangeManager externalExchangeManager,
            IExternalMarket externalMarket, ExchangeTradeWriter exchangeTradeWriter)
        {
            _externalExchangeManager = externalExchangeManager;
            _externalMarket = externalMarket;
            _exchangeTradeWriter = exchangeTradeWriter;
        }

        public async Task<CreateManualTradeResponse> CreateManualTradeAsync(CreateManualTradeRequest request)
        {
            try
            {
                var tradeForExchange = GetTradeForExchange(request);
                
                var trade = await _externalMarket.MarketTrade(tradeForExchange);
                await _exchangeTradeWriter.PublishTrade(trade);
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

        private MarketTradeRequest GetTradeForExchange(CreateManualTradeRequest request)
        {
            var marketTrade = new MarketTradeRequest();
            marketTrade.ExchangeName = request.ExchangeName;
            marketTrade.Volume = request.BaseVolume;
            marketTrade.Side = request.BaseVolume > 0 ? OrderSide.Buy : OrderSide.Sell;
            marketTrade.Market = request.InstrumentSymbol;

            return marketTrade;
        }

        public async Task<GetExternalExchangeCollectionResponse> GetExternalExchangeCollectionAsync()
        {
            return await _externalExchangeManager.GetExternalExchangeCollectionAsync();
        }
    }
}