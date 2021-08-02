using System;
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
        private readonly ExchangeTradeWriter _exchangeTradeWriter;
        private readonly IExternalExchangeManager _externalExchangeManager;
        private readonly IExternalMarket _externalMarket;
        
        public HedgePortfolioManager(IExchangeTradeManager exchangeTradeManager,
            ExchangeTradeWriter exchangeTradeWriter,
            IExternalExchangeManager externalExchangeManager,
            IExternalMarket externalMarket)
        {
            _exchangeTradeManager = exchangeTradeManager;
            _exchangeTradeWriter = exchangeTradeWriter;
            _externalExchangeManager = externalExchangeManager;
            _externalMarket = externalMarket;
        }
        
        public async Task<ExecutedVolumes> ExecuteHedgeConvert(string brokerId, string fromAsset, string toAsset, decimal fromAssetVolume, decimal toAssetVolume)
        {
            //найти внешнии рынки на которых мы можем обменять Asset1 и Asset2
            var externalMarkets = await GetAvailableExchangesAsync(fromAsset, toAsset);

            // резделить обьем между биржами
            var tradesForExternalMarkets = await GetTradesForExternalMarketAsync(externalMarkets,
                fromAsset, toAsset, fromAssetVolume, toAssetVolume);

            // выполнить трейды согластно плану
            var executedTrades = await ExecuteExternalMarketTrades(tradesForExternalMarkets, brokerId);

            // посчитать ExecutedVolume по факту
            return GetExecutedVolumesInRequestAssets(executedTrades, fromAsset, toAsset);
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
                        Exchange = exchange,
                        MarketInfo = exchangeMarketInfo
                    });
            }

            return availableExchanges;
        }

        private async Task<List<ExternalMarketTrade>> GetTradesForExternalMarketAsync(List<ExternalMarket> externalMarkets, string fromAsset, string toAsset, decimal fromVolume,
            decimal toVolume)
        {
            return await _exchangeTradeManager.GetTradesByExternalMarkets(externalMarkets, fromAsset, toAsset, fromVolume, toVolume);

        }

        private async Task<List<ExecutedTrade>> ExecuteExternalMarketTrades(
            List<ExternalMarketTrade> externalMarketTrades, string brokerId)
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

            return marketTrades;
        }

        private ExecutedVolumes GetExecutedVolumesInRequestAssets(List<ExecutedTrade> executedTrades, string fromAsset, string toAsset)
        {
            throw new System.NotImplementedException();
        }
    }
}