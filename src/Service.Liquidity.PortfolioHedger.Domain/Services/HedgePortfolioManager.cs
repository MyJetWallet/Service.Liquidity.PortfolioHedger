using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using MyJetWallet.Domain.Orders;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
{
    public class HedgePortfolioManager : IHedgePortfolioManager
    {
        private readonly IExchangeTradeManager _exchangeTradeManager;
        private readonly IExchangeTradeWriter _exchangeTradeWriter;
        private readonly IExternalExchangeManager _externalExchangeManager;
        private readonly IExternalMarket _externalMarket;
        
        public HedgePortfolioManager(IExchangeTradeManager exchangeTradeManager,
            IExchangeTradeWriter exchangeTradeWriter,
            IExternalExchangeManager externalExchangeManager,
            IExternalMarket externalMarket)
        {
            _exchangeTradeManager = exchangeTradeManager;
            _exchangeTradeWriter = exchangeTradeWriter;
            _externalExchangeManager = externalExchangeManager;
            _externalMarket = externalMarket;
        }
        private async Task<List<ExternalMarket>> GetAvailableExchangesAsync(string fromAsset, string toAsset)
        {
            var exchanges = (await _externalExchangeManager.GetExternalExchangeCollectionAsync()).ExchangeNames;

            var availableExchanges = new List<ExternalMarket>();
            
            foreach (var exchange in exchanges)
            {
                var exchangeMarkets = await _externalMarket.GetMarketInfoListAsync(new GetMarketInfoListRequest()
                {
                    ExchangeName = exchange
                });

                var exchangeMarketInfo = exchangeMarkets.Infos
                    .FirstOrDefault(e => (e.AssociateBaseAsset == fromAsset && e.AssociateQuoteAsset == toAsset) ||
                              (e.AssociateBaseAsset == toAsset && e.AssociateQuoteAsset == fromAsset));
                
                if (exchangeMarketInfo != null)
                    availableExchanges.Add(new ExternalMarket()
                    {
                        ExchangeName = exchange,
                        MarketInfo = exchangeMarketInfo
                    });
            }
            return availableExchanges;
        }

        public async Task<List<ExternalMarketTrade>> GetTradesForExternalMarketAsync(string fromAsset, string toAsset, decimal fromVolume, decimal toVolume)
        {
            var externalMarkets = await GetAvailableExchangesAsync(fromAsset, toAsset);
            return await _exchangeTradeManager.GetTradesByExternalMarkets(externalMarkets, fromAsset, toAsset, fromVolume, toVolume);
        }

        public async Task<ExecutedVolumes> ExecuteExternalMarketTrades(
            IEnumerable<ExternalMarketTrade> externalMarketTrades, string fromAsset, string toAsset, string brokerId)
        {
            var marketTrades = new List<ExecutedTrade>();
            foreach (var trade in externalMarketTrades)
            {
                var marketTradeRequest = new MarketTradeRequest
                {
                    ExchangeName = trade.ExchangeName,
                    Volume = (double) trade.BaseVolume,
                    Side = trade.BaseVolume > 0 ? OrderSide.Buy : OrderSide.Sell,
                    Market = trade.Market
                };
            
                var marketTrade = await _externalMarket.MarketTrade(marketTradeRequest);
            
                var exchangeTradeMessage = new TradeMessage()
                {
                    AssociateBrokerId = brokerId,
                    BaseAsset = trade.BaseAsset,
                    QuoteAsset = trade.QuoteAsset,
                    AssociateClientId = marketTrade.AssociateClientId,
                    AssociateSymbol = marketTrade.Market,
                    AssociateWalletId = trade.ExchangeName,
                    Id = marketTrade.Id,
                    Market = marketTrade.Market,
                    Volume = marketTrade.Volume,
                    Timestamp = marketTrade.Timestamp,
                    OppositeVolume = marketTrade.OppositeVolume,
                    Price = marketTrade.Price,
                    ReferenceId = marketTrade.ReferenceId,
                    Side = marketTrade.Side,
                    Source = marketTrade.Source
                };
                await _exchangeTradeWriter.PublishTrade(exchangeTradeMessage);

                marketTrades.Add(new ExecutedTrade()
                {
                    Trade = trade,
                    ExecutedBaseVolume = (decimal) marketTrade.Volume,
                    ExecutedQuoteVolume = (decimal) marketTrade.OppositeVolume
                });
            }
            return GetExecutedVolumesInRequestAssets(marketTrades, fromAsset, toAsset);
        }
        
        private ExecutedVolumes GetExecutedVolumesInRequestAssets(IReadOnlyCollection<ExecutedTrade> executedTrades, string fromAsset, string toAsset)
        {
            var executedVolumes = new ExecutedVolumes
            {
                FromAsset = fromAsset,
                ToAsset = toAsset,
                ExecutedFromVolume = executedTrades.Where(e => e.Trade.BaseAsset == fromAsset)
                    .Sum(e => e.ExecutedBaseVolume) + executedTrades.Where(e => e.Trade.BaseAsset == toAsset)
                    .Sum(e => e.ExecutedQuoteVolume),
                ExecutedToVolume = executedTrades.Where(e => e.Trade.BaseAsset == fromAsset)
                    .Sum(e => e.ExecutedQuoteVolume) + executedTrades.Where(e => e.Trade.BaseAsset == toAsset)
                    .Sum(e => e.ExecutedBaseVolume)
            };
            return executedVolumes;
        }
    }
}