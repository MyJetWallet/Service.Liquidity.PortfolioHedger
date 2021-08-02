using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using Service.IndexPrices.Client;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
{
    public class HedgePortfolioManager : IHedgePortfolioManager
    {
        private readonly IExchangeTradeManager _exchangeTradeManager;
        private readonly ExchangeTradeWriter _exchangeTradeWriter;
        private readonly IIndexPricesClient _indexPricesClient;
        private readonly IExternalExchangeManager _externalExchangeManager;
        private readonly IExternalMarket _externalMarket;
        
        public HedgePortfolioManager(IExchangeTradeManager exchangeTradeManager,
            ExchangeTradeWriter exchangeTradeWriter,
            IIndexPricesClient indexPricesClient,
            IExternalExchangeManager externalExchangeManager,
            IExternalMarket externalMarket)
        {
            _exchangeTradeManager = exchangeTradeManager;
            _exchangeTradeWriter = exchangeTradeWriter;
            _indexPricesClient = indexPricesClient;
            _externalExchangeManager = externalExchangeManager;
            _externalMarket = externalMarket;
        }

        public decimal GetOppositeVolume(string fromAsset, string toAsset, decimal fromVolume)
        {
            var fromAssetPrice = _indexPricesClient.GetIndexPriceByAssetAsync(fromAsset);
            var toAssetPrice = _indexPricesClient.GetIndexPriceByAssetAsync(toAsset);
            
            var toVolume = fromVolume * (fromAssetPrice.UsdPrice / toAssetPrice.UsdPrice);
            return toVolume;
        }

        public async Task<List<string>> GetAvailableExternalMarketsAsync(string fromAsset, string toAsset)
        {
            var exchanges = (await _externalExchangeManager.GetExternalExchangeCollectionAsync()).ExchangeNames;

            var availableMarkets = new List<string>();
            
            foreach (var exchange in exchanges)
            {
                var exchangeMarkets = await _externalMarket.GetMarketInfoListAsync(new GetMarketInfoListRequest()
                {
                    ExchangeName = exchange
                });

                var marketIsAvailable = exchangeMarkets.Infos
                    .Any(e => (e.AssociateBaseAsset == fromAsset && e.AssociateQuoteAsset == toAsset) ||
                              (e.AssociateBaseAsset == toAsset && e.AssociateQuoteAsset == fromAsset));
                
                if (marketIsAvailable)
                    availableMarkets.Add(exchange);
            }

            return availableMarkets;
        }

        public List<ExternalMarketTrade> GetTradesForExternalMarket(List<string> externalMarkets, string fromAsset, string toAsset, decimal fromVolume,
            decimal toVolume)
        {
            var tradesByExchanges = new List<ExternalMarketTrade>();
            foreach (var externalMarket in externalMarkets)
            {
                // получить с каждого рынка доступные ордера с учтом баланса и верхней граници сделки
                var availableOrders = _exchangeTradeManager.GetAvailableOrders(externalMarket, fromAsset, toAsset, fromVolume, toVolume);

                // Сортируем агрегированный ордербук по цене
                var sortedOrderBook = _exchangeTradeManager.GetSortedOrderBook(availableOrders);

                // Наберем нужный обьем по агрегированному ордербуку
                var tradeByExchange = _exchangeTradeManager.GetTradeByExchange(sortedOrderBook);
                
                tradesByExchanges.Add(tradeByExchange);
            }
            return tradesByExchanges;
        }

        public List<ExecutedTrade> ExecuteExternalMarketTrades(List<ExternalMarketTrade> externalMarketTrades)
        {
            
            //await _exchangeTradeWriter.PublishTrade(exchangeTradeMessage);
            
            throw new System.NotImplementedException();
        }

        public ExecutedVolumes GetExecutedVolumesInRequestAssets(List<ExecutedTrade> executedTrades, string fromAsset, string toAsset)
        {
            throw new System.NotImplementedException();
        }
    }
}