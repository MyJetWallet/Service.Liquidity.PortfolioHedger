using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Models;
using Service.Liquidity.PortfolioHedger.Grpc;
using Service.Liquidity.PortfolioHedger.Grpc.Models;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class HedgePortfolioService : IHedgePortfolioService
    {
        private readonly IHedgePortfolioManager _hedgePortfolioManager;
        private readonly PortfolioHandler _portfolioHandler;

        public HedgePortfolioService(IHedgePortfolioManager hedgePortfolioManager,
            PortfolioHandler portfolioHandler)
        {
            _hedgePortfolioManager = hedgePortfolioManager;
            _portfolioHandler = portfolioHandler;
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
            var availableOppositeAssets = _portfolioHandler.GetAvailableOppositeAssets(request.PortfolioSnapshot, request.FromAsset, request.FromAssetVolume);

            foreach (var oppositeAsset in availableOppositeAssets.OrderBy(e=> e.Order))
            {
                var trades = await _hedgePortfolioManager.GetTradesForExternalMarketAsync(request.FromAsset, 
                    oppositeAsset.Asset, request.FromAssetVolume, oppositeAsset.Volume);

                if (!trades.Any()) 
                    continue;
                
                var executedVolumes = await _hedgePortfolioManager.ExecuteExternalMarketTrades(trades, request.FromAsset,
                    oppositeAsset.Asset, "jetwallet");

                _portfolioHandler.ChangePortfolioBalance(request.PortfolioSnapshot, executedVolumes);
                
                return new ExecuteAutoConvertResponse()
                {
                    Success = true,
                    ExecutedFromValue = executedVolumes.ExecutedFromVolume,
                    ExecutedToValue = executedVolumes.ExecutedToVolume, 
                    ToAsset = oppositeAsset.Asset
                };
            }
            return new ExecuteAutoConvertResponse()
            {
                Success = false,
                ErrorMessage = "Cant find any available trades."
            };
        }
    }
}