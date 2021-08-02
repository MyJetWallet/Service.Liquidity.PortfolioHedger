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
            //по индекс цене расчитываем Amount 2
            var toVolume =
                _hedgePortfolioManager.GetOppositeVolume(request.FromAsset, request.ToAsset, request.FromAssetVolume);
            
            //найти внешнии рынки на которых мы можем обменять Asset1 и Asset2
            var externalMarkets = await _hedgePortfolioManager.GetAvailableExternalMarketsAsync(request.FromAsset, request.ToAsset);

            // резделить обьем между биржами
            var tradesForExternalMarkets = _hedgePortfolioManager.GetTradesForExternalMarket(externalMarkets,
                request.FromAsset, request.ToAsset, request.FromAssetVolume, toVolume);

            // выполнить трейды согластно плану
            var executedTrades = _hedgePortfolioManager.ExecuteExternalMarketTrades(tradesForExternalMarkets);

            // посчитать ExecutedVolume по факту
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