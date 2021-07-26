using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using MyJetWallet.Domain.Orders;
using MyJetWallet.Sdk.Service;
using Newtonsoft.Json;
using Service.AssetsDictionary.Client;
using Service.Liquidity.PortfolioHedger.Grpc;
using Service.Liquidity.PortfolioHedger.Grpc.Models;
using Service.Liquidity.PortfolioHedger.ServiceBus;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class ManualTradeService : IManualTradeService
    {
        private readonly IExternalExchangeManager _externalExchangeManager;
        private readonly IExternalMarket _externalMarket;
        private readonly ExchangeTradeWriter _exchangeTradeWriter;
        private readonly ILogger<ManualTradeService> _logger;
        private readonly ISpotInstrumentDictionaryClient _spotInstrumentDictionaryClient;
        private readonly HedgerMetricsInterceptor _hedgerMetricsInterceptor;

        public ManualTradeService(IExternalExchangeManager externalExchangeManager,
            IExternalMarket externalMarket,
            ExchangeTradeWriter exchangeTradeWriter,
            ILogger<ManualTradeService> logger,
            ISpotInstrumentDictionaryClient spotInstrumentDictionaryClient,
            HedgerMetricsInterceptor hedgerMetricsInterceptor)
        {
            _externalExchangeManager = externalExchangeManager;
            _externalMarket = externalMarket;
            _exchangeTradeWriter = exchangeTradeWriter;
            _logger = logger;
            _spotInstrumentDictionaryClient = spotInstrumentDictionaryClient;
            _hedgerMetricsInterceptor = hedgerMetricsInterceptor;
        }

        public async Task<ManualTradeResponse> CreateManualTradeAsync(CreateManualTradeRequest request)
        {
            _logger.LogInformation("CreateManualTradeAsync receive message: {requestJson}", JsonConvert.SerializeObject(request));
            try
            {
                var tradeForExchange = GetTradeForExchange(request);
                _logger.LogInformation("tradeForExchange: {tradeForExchangeJson}", JsonConvert.SerializeObject(tradeForExchange));
                
                var marketTrade = await _externalMarket.MarketTrade(tradeForExchange);
                _logger.LogInformation("Trade from externalMarket: {tradeJson}", JsonConvert.SerializeObject(marketTrade));

                var exchangeTradeMessage = new TradeMessage()
                {
                    AssociateBrokerId = request.AssociateBrokerId,
                    BaseAsset = request.BaseAsset,
                    QuoteAsset = request.QuoteAsset,
                    AssociateClientId = marketTrade.AssociateClientId,
                    AssociateSymbol = marketTrade.Market,
                    AssociateWalletId = request.ExchangeName,
                    Id = marketTrade.Id,
                    Market = marketTrade.Market,
                    Volume = marketTrade.Volume,
                    Timestamp = marketTrade.Timestamp,
                    OppositeVolume = marketTrade.OppositeVolume,
                    Price = marketTrade.Price,
                    ReferenceId = marketTrade.ReferenceId,
                    Side = marketTrade.Side,
                    Source = marketTrade.Source,
                    Comment = request.Comment,
                    User = request.User
                };
                _hedgerMetricsInterceptor.SetExecuteTradeMetrics(request);
                await _exchangeTradeWriter.PublishTrade(exchangeTradeMessage);
            }
            catch (Exception exception)
            {
                _logger.LogError("CreateManualTradeAsync receive exception: {exceptionJson}", JsonConvert.SerializeObject(exception));
                
                return new ManualTradeResponse()
                {
                    Success = false,
                    ErrorMessage = exception.Message
                };
            }
            return new ManualTradeResponse()
            {
                Success = true
            };
        }

        public async Task<ManualTradeResponse> ReportManualTradeAsync(ReportManualTradeRequest request)
        {
            using var activity = MyTelemetry.StartActivity("CreateManualTradeAsync");

            request.AddToActivityAsJsonTag("CreateTradeManualRequest");
            
            _logger.LogInformation($"CreateManualTradeAsync receive request: {JsonConvert.SerializeObject(request)}");
            
            if (string.IsNullOrWhiteSpace(request.BrokerId) ||
                string.IsNullOrWhiteSpace(request.WalletName) ||
                string.IsNullOrWhiteSpace(request.Symbol) ||
                string.IsNullOrWhiteSpace(request.Comment) ||
                string.IsNullOrWhiteSpace(request.User) ||
                request.Price == 0 ||
                request.BaseVolume == 0 ||
                request.QuoteVolume == 0 ||
                (request.BaseVolume > 0 && request.QuoteVolume > 0) ||
                (request.BaseVolume < 0 && request.QuoteVolume < 0))
            {
                _logger.LogError($"Bad request entity: {JsonConvert.SerializeObject(request)}");
                return new ManualTradeResponse() {Success = false, ErrorMessage = "Incorrect entity"};
            }
            var instruments = _spotInstrumentDictionaryClient.GetSpotInstrumentByBroker(new JetBrandIdentity
            {
                BrokerId = request.BrokerId
            });
            var instrument = instruments.FirstOrDefault(e => e.Symbol == request.Symbol);

            var trade = new TradeMessage()
            {
                Id = Guid.NewGuid().ToString("N"),
                ReferenceId = string.Empty,
                Market = request.Symbol,
                Side = request.BaseVolume < 0 ? OrderSide.Sell : OrderSide.Buy,
                Price = request.Price,
                Volume = request.BaseVolume,
                OppositeVolume = request.QuoteVolume,
                Timestamp = DateTime.UtcNow,
                AssociateWalletId = request.WalletName,
                AssociateBrokerId = request.BrokerId,
                AssociateClientId = string.Empty,
                AssociateSymbol = request.Symbol,
                Source = "manual",
                BaseAsset = instrument?.BaseAsset,
                QuoteAsset = instrument?.QuoteAsset,
                Comment = request.Comment,
                User = request.User
            };
            try
            {
                _hedgerMetricsInterceptor.SetReportTradeMetrics(request);
                await _exchangeTradeWriter.PublishTrade(trade);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Creating failed: {JsonConvert.SerializeObject(exception)}");
                return new ManualTradeResponse() {Success = false, ErrorMessage = exception.Message};
            }

            var response = new ManualTradeResponse() {Success = true};
            
            _logger.LogInformation($"CreateManualTradeAsync return reponse: {JsonConvert.SerializeObject(response)}");
            return response;
        }

        private MarketTradeRequest GetTradeForExchange(CreateManualTradeRequest request)
        {
            var marketTrade = new MarketTradeRequest
            {
                ExchangeName = request.ExchangeName,
                Volume = request.BaseVolume,
                Side = request.BaseVolume > 0 ? OrderSide.Buy : OrderSide.Sell,
                Market = request.Market
            };
            return marketTrade;
        }

        public async Task<GetExternalExchangeCollectionResponse> GetExternalExchangeCollectionAsync()
        {
            return await _externalExchangeManager.GetExternalExchangeCollectionAsync();
        }
    }
}