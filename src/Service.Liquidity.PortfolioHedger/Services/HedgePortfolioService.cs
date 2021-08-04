using System.Collections.Generic;
using System.Threading.Tasks;
using Service.IndexPrices.Client;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Grpc;
using Service.Liquidity.PortfolioHedger.Grpc.Models;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class HedgePortfolioService : IHedgePortfolioService
    {
        private readonly IHedgePortfolioManager _hedgePortfolioManager;
        private HedgePortfolioHelper _hedgePortfolioHelper;

        public HedgePortfolioService(IHedgePortfolioManager hedgePortfolioManager,
            HedgePortfolioHelper hedgePortfolioHelper)
        {
            _hedgePortfolioManager = hedgePortfolioManager;
            _hedgePortfolioHelper = hedgePortfolioHelper;
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
            
            var toVolume = _hedgePortfolioHelper.GetOppositeVolume(request.FromAsset, request.ToAsset, request.FromAssetVolume);

            if (toVolume == 0)
            {
                return new ExecuteManualConvertResponse()
                {
                    Success = false,
                    ErrorMessage = "Cannot calculate opposite asset volume."
                };
            }
            
            var brokerId = Program.Settings.DefaultBrokerId;
            var executedVolumes = await _hedgePortfolioManager.ExecuteHedgeConvert(brokerId, request.FromAsset, request.ToAsset,
                request.FromAssetVolume, toVolume);

            return new ExecuteManualConvertResponse()
            {
                Success = true,
                ExecutedFromValue = executedVolumes.ExecutedFromVolume,
                ExecutedToValue = executedVolumes.ExecutedToVolume
            };
        }

        public async Task<ExecuteAutoConvertResponse> ExecuteAutoConvert(ExecuteAutoConvertRequest request)
        {
            if (request.FromAsset == string.Empty || request.FromAssetVolume == 0)
            {
                return new ExecuteAutoConvertResponse()
                {
                    Success = false,
                    ErrorMessage = "Bad request: asset and volume cannot has empty values."
                };
            }
            
            var assetsToSkip = new List<string>() {request.FromAsset};
            var toAsset = _hedgePortfolioHelper.GetOppositeAsset(assetsToSkip, request.FromAssetVolume);

            if (string.IsNullOrWhiteSpace(toAsset))
            {
                return new ExecuteAutoConvertResponse()
                {
                    Success = false,
                    ErrorMessage = "Cannot find opposite asset."
                };
            }
            
            var toVolume = _hedgePortfolioHelper.GetOppositeVolume(request.FromAsset, toAsset, request.FromAssetVolume);

            if (toVolume == 0)
            {
                return new ExecuteAutoConvertResponse()
                {
                    Success = false,
                    ErrorMessage = "Cannot calculate opposite asset volume."
                };
            }
            
            var brokerId = Program.Settings.DefaultBrokerId;
            var executedVolumes = await _hedgePortfolioManager.ExecuteHedgeConvert(brokerId, request.FromAsset, toAsset,
                request.FromAssetVolume, toVolume);

            return new ExecuteAutoConvertResponse()
            {
                Success = true,
                ExecutedFromValue = executedVolumes.ExecutedFromVolume,
                ExecutedToValue = executedVolumes.ExecutedToVolume, 
                ToAsset = toAsset
            };
        }
    }
}