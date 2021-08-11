using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Models;
using Service.Liquidity.PortfolioHedger.Grpc;
using Service.Liquidity.PortfolioHedger.Grpc.Models;

namespace Service.Liquidity.PortfolioHedger.Services.Grpc
{
    public class HedgePortfolioService : IHedgePortfolioService
    {
        private readonly IHedgePortfolioCalculator _hedgePortfolioCalculator;
        private readonly PortfolioHandler _portfolioHandler;
        private readonly IExternalMarketTradesExecutor _externalMarketTradesExecutor;

        public HedgePortfolioService(IHedgePortfolioCalculator hedgePortfolioCalculator,
            PortfolioHandler portfolioHandler,
            IExternalMarketTradesExecutor externalMarketTradesExecutor)
        {
            _hedgePortfolioCalculator = hedgePortfolioCalculator;
            _portfolioHandler = portfolioHandler;
            _externalMarketTradesExecutor = externalMarketTradesExecutor;
        }

        public async Task ExecuteAutoConvert(ExecuteAutoConvertRequest request)
        {
            var actualPortfolio = request.PortfolioSnapshot;
            var analysisResult = _portfolioHandler.GetAnalysisResult(actualPortfolio);
            
            var tradesForHedge = new List<ExternalMarketTrade>();
            
            while (analysisResult.HedgeIsNeeded)
            {
                var getTradesForHedgeRequest = await _hedgePortfolioCalculator.GetCalculationForHedge(actualPortfolio, analysisResult.FromAsset, analysisResult.FromAssetVolume);

                if (getTradesForHedgeRequest.Trades == null)
                {
                    // todo: error log with portfolio json
                    break;
                }

                tradesForHedge.AddRange(getTradesForHedgeRequest.Trades);
                actualPortfolio = getTradesForHedgeRequest.PortfolioAfterTrades;
                analysisResult = _portfolioHandler.GetAnalysisResult(actualPortfolio);
            }

            var result = await _externalMarketTradesExecutor.ExecuteExternalMarketTrades(tradesForHedge, string.Empty,"jetwallet");
        }

        public async Task<ExecuteManualConvertResponse> ExecuteManualConvert(ExecuteManualConvertRequest request)
        {
            if (request.FromAsset == string.Empty || request.FromAssetVolume == 0)
            {
                return new ExecuteManualConvertResponse()
                {
                    Success = false,
                    ErrorMessage = "Bad request: asset and volume cannot has empty values."
                };
            }
            
            var portfolioSnapshot = _portfolioHandler.GetPortfolioSnapshot();
            
            var calculationResult = await _hedgePortfolioCalculator.GetCalculationForHedge(portfolioSnapshot, request.FromAsset, request.FromAssetVolume);
            
            var result = await _externalMarketTradesExecutor.ExecuteExternalMarketTrades(calculationResult.Trades, request.FromAsset,"jetwallet");
            
            return new ExecuteManualConvertResponse()
            {
                Success = true,
                FromAsset = result.FromAsset,
                ExecutedFromValue = result.ExecutedFromVolume
            };
        }

    }
}