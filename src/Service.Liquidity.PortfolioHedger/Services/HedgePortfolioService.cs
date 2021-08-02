using System.Threading.Tasks;
using Service.IndexPrices.Client;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Services;
using Service.Liquidity.PortfolioHedger.Grpc;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class HedgePortfolioService : IHedgePortfolioService
    {
        private readonly IHedgePortfolioManager _hedgePortfolioManager;
        private readonly IIndexPricesClient _indexPricesClient;

        public HedgePortfolioService(IHedgePortfolioManager hedgePortfolioManager,
            IIndexPricesClient indexPricesClient)
        {
            _hedgePortfolioManager = hedgePortfolioManager;
            _indexPricesClient = indexPricesClient;
        }

        public async Task<ExecuteManualConvertResponse> ExecuteManualConvert(ExecuteManualConvertRequest request)
        {
            //по индекс цене расчитываем Amount 2
            var toVolume = GetOppositeVolume(request.FromAsset, request.ToAsset, request.FromAssetVolume);

            var brokerId = Program.Settings.DefaultBrokerId;
            var executedVolumes = await _hedgePortfolioManager.ExecuteHedgeConvert(brokerId, request.FromAsset, request.ToAsset,
                request.FromAssetVolume, toVolume);

            return new ExecuteManualConvertResponse()
            {
                ExecutedFromValue = executedVolumes.ExecutedFromVolume,
                ExecutedToValue = executedVolumes.ExecutedToVolume
            };
        }

        private decimal GetOppositeVolume(string fromAsset, string toAsset, decimal fromVolume)
        {
            var fromAssetPrice = _indexPricesClient.GetIndexPriceByAssetAsync(fromAsset);
            var toAssetPrice = _indexPricesClient.GetIndexPriceByAssetAsync(toAsset);
            
            var toVolume = fromVolume * (fromAssetPrice.UsdPrice / toAssetPrice.UsdPrice);
            return toVolume;
        }
    }
}