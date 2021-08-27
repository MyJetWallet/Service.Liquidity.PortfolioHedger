using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.IndexPrices.Client;
using Service.Liquidity.Portfolio.Domain.Models;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class HedgePortfolioCalculator : IHedgePortfolioCalculator
    {
        private readonly IExchangeTradeManager _exchangeTradeManager;
        private readonly PortfolioHandler _portfolioHandler;
        private readonly IIndexPricesClient _indexPricesClient;
        
        public HedgePortfolioCalculator(IExchangeTradeManager exchangeTradeManager,
            PortfolioHandler portfolioHandler,
            IIndexPricesClient indexPricesClient)
        {
            _exchangeTradeManager = exchangeTradeManager;
            _portfolioHandler = portfolioHandler;
            _indexPricesClient = indexPricesClient;
        }
        
        public async Task<GetTradesForHedgeRequest> GetCalculationForHedge(AssetPortfolio portfolioSnapshot, string fromAsset, decimal fromAssetVolume)
        {
            var tradesForHedge = new List<ExternalMarketTrade>();
            var remainder = fromAssetVolume;
            var actualPortfolio = portfolioSnapshot;
            
            var availableOppositeAssets = _portfolioHandler.GetAvailableOppositeAssets(actualPortfolio, 
                fromAsset, fromAssetVolume);
            
            foreach (var oppositeAsset in availableOppositeAssets.OrderBy(e=> e.Order))
            {
                var calculationResponseByOppositeAsset = await CalculateHedge(new CalculateHedgeRequest()
                {
                    PortfolioSnapshot = actualPortfolio,
                    FromAsset = fromAsset,
                    ToAsset = oppositeAsset.Asset,
                    FromVolume = remainder,
                    ToVolume = oppositeAsset.Volume
                });

                if (calculationResponseByOppositeAsset.Trades != null && calculationResponseByOppositeAsset.Trades.Any())
                    tradesForHedge.AddRange(calculationResponseByOppositeAsset.Trades);
                
                remainder = _portfolioHandler.GetRemainder(portfolioSnapshot, calculationResponseByOppositeAsset.NewPortfolio, fromAsset, fromAssetVolume);
                actualPortfolio = calculationResponseByOppositeAsset.NewPortfolio;
                
                if (remainder == 0)
                {
                    break;
                }
            }
            return new GetTradesForHedgeRequest()
            {
                Trades = GetAggregatedTradesForHedge(tradesForHedge).ToList(),
                PortfolioAfterTrades = actualPortfolio
            };
        }
        
        private IEnumerable<ExternalMarketTrade> GetAggregatedTradesForHedge(List<ExternalMarketTrade> tradesForHedge)
        {
            var aggregatedTrades = new List<ExternalMarketTrade>();
            
            tradesForHedge.ForEach(e =>
            {
                var agrTrade =
                    aggregatedTrades.FirstOrDefault(x => x.ExchangeName == e.ExchangeName && x.Market == e.Market);

                if (agrTrade != null)
                {
                    agrTrade.BaseVolume += e.BaseVolume;
                    agrTrade.QuoteVolume += e.QuoteVolume;
                }
                else
                {
                    aggregatedTrades.Add(new ExternalMarketTrade()
                    {
                        ExchangeName = e.ExchangeName,
                        Market = e.Market,
                        BaseAsset = e.BaseAsset,
                        QuoteAsset = e.QuoteAsset,
                        BaseVolume = e.BaseVolume,
                        QuoteVolume = e.QuoteVolume
                    });
                }
            });
            return aggregatedTrades;
        }

        private async Task<CalculateHedgeResponse> CalculateHedge(CalculateHedgeRequest request)
        {
            var response = new CalculateHedgeResponse();

            var trades =
                await _exchangeTradeManager.GetTradesByExternalMarkets(request.FromAsset, request.ToAsset, request.FromVolume,
                    request.ToVolume);
            
            response.Trades = trades;
            response.NewPortfolio = GetNewPortfolio(request.PortfolioSnapshot, trades);

            return response;
        }

        private AssetPortfolio GetNewPortfolio(AssetPortfolio portfolioSnapshot, List<ExternalMarketTrade> trades)
        {
            var newPortfolio = new AssetPortfolio()
            {
                BalanceByAsset = portfolioSnapshot.BalanceByAsset.Select(e => e.GetCopy()).ToList(),
                BalanceByWallet = portfolioSnapshot.BalanceByWallet.Select(e => e.GetCopy()).ToList()
            };
            trades.ForEach(trade =>
            {
                var fromAssetBalance =
                    newPortfolio.BalanceByAsset.FirstOrDefault(e => e.Asset == trade.BaseAsset);

                if (fromAssetBalance != null)
                {
                    fromAssetBalance.Volume += trade.BaseVolume;
                    fromAssetBalance.UsdVolume = fromAssetBalance.Volume *
                                                    _indexPricesClient.GetIndexPriceByAssetAsync(fromAssetBalance.Asset)
                                                        .UsdPrice;
                }

                var toAssetBalance =
                    newPortfolio.BalanceByAsset.FirstOrDefault(e => e.Asset == trade.QuoteAsset);

                if (toAssetBalance != null)
                {
                    toAssetBalance.Volume += trade.QuoteVolume;
                    toAssetBalance.UsdVolume = toAssetBalance.Volume *
                                                  _indexPricesClient.GetIndexPriceByAssetAsync(toAssetBalance.Asset)
                                                      .UsdPrice;
                }
            });
            return newPortfolio;
        }
    }
}