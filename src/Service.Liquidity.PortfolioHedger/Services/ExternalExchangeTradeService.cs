using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using MyJetWallet.Domain.Orders;
using Newtonsoft.Json;
using Service.Liquidity.PortfolioHedger.Grpc;
using Service.Liquidity.PortfolioHedger.Grpc.Models;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class ExternalExchangeTradeService : IExternalExchangeTradeService
    {
        private readonly IExternalExchangeManager _externalExchangeManager;
        private readonly IExternalMarket _externalMarket;
        private readonly ExchangeTradeWriter _exchangeTradeWriter;
        private readonly ILogger<ExternalExchangeTradeService> _logger;

        public ExternalExchangeTradeService(IExternalExchangeManager externalExchangeManager,
            IExternalMarket externalMarket, ExchangeTradeWriter exchangeTradeWriter,
            ILogger<ExternalExchangeTradeService> logger)
        {
            _externalExchangeManager = externalExchangeManager;
            _externalMarket = externalMarket;
            _exchangeTradeWriter = exchangeTradeWriter;
            _logger = logger;
        }

        public async Task<CreateManualTradeResponse> CreateManualTradeAsync(CreateManualTradeRequest request)
        {
            _logger.LogInformation("CreateManualTradeAsync receive message: {requestJson}", JsonConvert.SerializeObject(request));
            try
            {
                var tradeForExchange = GetTradeForExchange(request);
                _logger.LogInformation("tradeForExchange: {tradeForExchangeJson}", JsonConvert.SerializeObject(tradeForExchange));
                
                var trade = await _externalMarket.MarketTrade(tradeForExchange);
                
                _logger.LogInformation("Trade from externalMarket: {tradeJson}", JsonConvert.SerializeObject(trade));
                
                await _exchangeTradeWriter.PublishTrade(trade);
            }
            catch (Exception exception)
            {
                _logger.LogError("CreateManualTradeAsync receive exception: {exceptionJson}", JsonConvert.SerializeObject(exception));
                
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