using System.Collections.Generic;
using System.Linq;
using Service.IndexPrices.Client;
using Service.Liquidity.Portfolio.Domain.Models;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class PortfolioHandler
    {
        private readonly IIndexPricesClient _indexPricesClient;
        private const string UsdAssetName = "USD";

        public PortfolioHandler(IIndexPricesClient indexPricesClient)
        {
            _indexPricesClient = indexPricesClient;
        }

        public List<AvailableOppositeAsset> GetAvailableOppositeAssets(AssetPortfolio portfolioSnapshot, string fromAsset,
            decimal fromVolume)
        {
            var response = new List<AvailableOppositeAsset>()
            {
                new AvailableOppositeAsset()
                {
                    Asset = UsdAssetName,
                    Order = 999,
                    Volume = GetOppositeVolume(fromAsset, UsdAssetName, fromVolume)
                }
            };
            if (portfolioSnapshot == null)
            {
                return response;
            }
            
            if (fromVolume > 0)
            {
                var orderedPortfolio = portfolioSnapshot.BalanceByAsset
                    .Where(e => e.Asset != fromAsset && e.NetUsdVolume < 0)
                    .OrderBy(e => e.NetUsdVolume)
                    .ToList();
                
                for (var index = 0; index < orderedPortfolio.Count; index++)
                {
                    var balanceByAsset = orderedPortfolio[index];
                    response.Add(new AvailableOppositeAsset()
                    {
                        Order = index,
                        Asset = balanceByAsset.Asset,
                        Volume = GetOppositeVolume(fromAsset, balanceByAsset.Asset, fromVolume)
                    });
                }
            }
            else
            {
                var orderedPortfolio = portfolioSnapshot.BalanceByAsset
                    .Where(e => e.Asset != fromAsset && e.NetUsdVolume > 0)
                    .OrderBy(e => e.NetUsdVolume)
                    .ToList();

                for (var index = 0; index < orderedPortfolio.Count; index++)
                {
                    var balanceByAsset = orderedPortfolio[index];
                    response.Add(new AvailableOppositeAsset()
                    {
                        Order = index,
                        Asset = balanceByAsset.Asset,
                        Volume = GetOppositeVolume(fromAsset, balanceByAsset.Asset, fromVolume)
                    });
                }
            }
            return response;
        }

        private decimal GetOppositeVolume(string fromAsset, string toAsset, decimal fromVolume)
        {
            var fromAssetPrice = _indexPricesClient.GetIndexPriceByAssetAsync(fromAsset);
            var toAssetPrice = _indexPricesClient.GetIndexPriceByAssetAsync(toAsset);
            
            var toVolume = fromVolume * (fromAssetPrice.UsdPrice / toAssetPrice.UsdPrice);
            return toVolume;
        }

        public void ChangePortfolioBalance(AssetPortfolio portfolioSnapshot, ExecutedVolumes executedVolumes)
        {
            var fromAssetBalance =
                portfolioSnapshot.BalanceByAsset.FirstOrDefault(e => e.Asset == executedVolumes.FromAsset);
            
            if (fromAssetBalance != null)
                fromAssetBalance.NetVolume += executedVolumes.ExecutedFromVolume;

            var toAssetBalance =
                portfolioSnapshot.BalanceByAsset.FirstOrDefault(e => e.Asset == executedVolumes.ToAsset);
            
            if (toAssetBalance != null)
                toAssetBalance.NetVolume += executedVolumes.ExecutedToVolume;
        }
    }
}