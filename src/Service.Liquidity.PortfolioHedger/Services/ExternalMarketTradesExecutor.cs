using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using MyJetWallet.Domain.Orders;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class ExternalMarketTradesExecutor : IExternalMarketTradesExecutor
    {
        private readonly IExternalMarket _externalMarket;

        public ExternalMarketTradesExecutor(IExternalMarket externalMarket)
        {
            _externalMarket = externalMarket;
        }

        public async Task<ExecutedVolumes> ExecuteExternalMarketTrades(IEnumerable<ExternalMarketTrade> externalMarketTrades, string fromAsset, string brokerId)
        {
            var marketTrades = new List<ExecutedTrade>();

            if (externalMarketTrades == null || !externalMarketTrades.Any())
            {
                return new ExecutedVolumes();
            }
            
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
                //await _exchangeTradeWriter.PublishTrade(exchangeTradeMessage);

                marketTrades.Add(new ExecutedTrade()
                {
                    Trade = trade,
                    ExecutedBaseVolume = (decimal) marketTrade.Volume,
                    ExecutedQuoteVolume = (decimal) marketTrade.OppositeVolume
                });
            }
            var executedVolumes = fromAsset == string.Empty 
                ? new ExecutedVolumes()
                : new ExecutedVolumes
                {
                    FromAsset = fromAsset,
                    ExecutedFromVolume = marketTrades.Where(e => e.Trade.BaseAsset == fromAsset)
                        .Sum(e => e.ExecutedBaseVolume) + marketTrades.Where(e => e.Trade.QuoteAsset == fromAsset)
                        .Sum(e => e.ExecutedQuoteVolume)
                };
            return executedVolumes;
        }
    }
}