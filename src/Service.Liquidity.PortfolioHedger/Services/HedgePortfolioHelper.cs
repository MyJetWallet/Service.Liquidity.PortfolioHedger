using System.Collections.Generic;
using System.Linq;
using MyNoSqlServer.Abstractions;
using Service.IndexPrices.Client;
using Service.Liquidity.Portfolio.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class HedgePortfolioHelper
    {
        private readonly IIndexPricesClient _indexPricesClient;
        private readonly IMyNoSqlServerDataReader<AssetPortfolioBalanceNoSql> _assetPortfolioBalanceDataReader;

        public HedgePortfolioHelper(IIndexPricesClient indexPricesClient,
            IMyNoSqlServerDataReader<AssetPortfolioBalanceNoSql> assetPortfolioBalanceDataReader)
        {
            _indexPricesClient = indexPricesClient;
            _assetPortfolioBalanceDataReader = assetPortfolioBalanceDataReader;
        }

        public string GetOppositeAsset(List<string> assetsToSkip, decimal fromVolume)
        {
            var portfolioEntity = _assetPortfolioBalanceDataReader.Get().FirstOrDefault();
            var portfolio = portfolioEntity?.Balance;

            if (portfolio == null)
            {
                return string.Empty;
            }

            if (fromVolume > 0)
            {
                var orderedPortfolio = portfolio.BalanceByAsset
                    .Where(e => !assetsToSkip.Contains(e.Asset))
                    .OrderBy(e => e.NetUsdVolume)
                    .ToList();

                var firstElem = orderedPortfolio.FirstOrDefault();

                if (firstElem == null || firstElem.NetUsdVolume >= 0)
                {
                    return string.Empty;
                }
                return firstElem.Asset;
            }
            else
            {
                var orderedPortfolio = portfolio.BalanceByAsset
                    .Where(e => !assetsToSkip.Contains(e.Asset))
                    .OrderByDescending(e => e.NetUsdVolume)
                    .ToList();

                var firstElem = orderedPortfolio.FirstOrDefault();

                if (firstElem == null || firstElem.NetUsdVolume <= 0)
                {
                    return string.Empty;
                }
                return firstElem.Asset;
            }
        }

        public decimal GetOppositeVolume(string fromAsset, string toAsset, decimal fromVolume)
        {
            var fromAssetPrice = _indexPricesClient.GetIndexPriceByAssetAsync(fromAsset);
            var toAssetPrice = _indexPricesClient.GetIndexPriceByAssetAsync(toAsset);
            
            var toVolume = fromVolume * (fromAssetPrice.UsdPrice / toAssetPrice.UsdPrice);
            return toVolume;
        }
        
    }
}