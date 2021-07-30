using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Domain.Services;
using Service.Liquidity.PortfolioHedger.Grpc;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class HedgePortfolioService : IHedgePortfolioService
    {
        private readonly IHedgePortfolioManager _hedgePortfolioManager;

        public HedgePortfolioService(IHedgePortfolioManager hedgePortfolioManager)
        {
            _hedgePortfolioManager = hedgePortfolioManager;
        }

        public async Task<ExecuteManualConvertResponse> ExecuteManualConvert(ExecuteManualConvertRequest request)
        {
            var toVolume =
                _hedgePortfolioManager.GetOppositeVolume(request.FromAsset, request.ToAsset, request.FromAssetVolume);
            var externalMarkets = _hedgePortfolioManager.GetAvailableExternalMarkets(request.FromAsset, request.ToAsset);

            var tradesForExternalMarkets = _hedgePortfolioManager.GetTradesForExternalMarket(externalMarkets,
                request.FromAsset, request.ToAsset, request.FromAssetVolume, toVolume);

            var executedTrades = _hedgePortfolioManager.ExecuteExternalMarketTrades(tradesForExternalMarkets);

            var executedVolumes =
                _hedgePortfolioManager.GetExecutedVolumesInRequestAssets(executedTrades, request.FromAsset,
                    request.ToAsset);

            return new ExecuteManualConvertResponse()
            {
                ExecutedFromValue = executedVolumes.ExecutedFromVolume,
                ExecutedToValue = executedVolumes.ExecutedToVolume
            };
        }
    }
}