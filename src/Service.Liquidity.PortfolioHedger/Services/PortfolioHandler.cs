using System;
using System.Collections.Generic;
using System.Linq;
using MyNoSqlServer.Abstractions;
using Service.IndexPrices.Client;
using Service.Liquidity.Portfolio.Domain.Models;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class PortfolioHandler
    {
        private readonly IIndexPricesClient _indexPricesClient;
        private const string UsdAssetName = "USD";
        private readonly IMyNoSqlServerDataReader<AssetPortfolioBalanceNoSql> _assetPortfolioBalanceDataReader;

        public PortfolioHandler(IIndexPricesClient indexPricesClient,
            IMyNoSqlServerDataReader<AssetPortfolioBalanceNoSql> assetPortfolioBalanceDataReader)
        {
            _indexPricesClient = indexPricesClient;
            _assetPortfolioBalanceDataReader = assetPortfolioBalanceDataReader;
        }

        public List<AvailableOppositeAsset> GetAvailableOppositeAssets(AssetPortfolio portfolioSnapshot, string fromAsset, decimal fromVolume)
        {
            var response = new List<AvailableOppositeAsset>();

            if (fromAsset != UsdAssetName)
            {
                response.Add(new AvailableOppositeAsset()
                {
                    Asset = UsdAssetName,
                    Order = 999,
                    Volume = int.MaxValue // todo: вынести в конфиг
                });
            }
            
            if (portfolioSnapshot == null)
            {
                return response;
            }
            if (fromVolume < 0)
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
                        Volume = GetOppositeVolume(portfolioSnapshot, fromAsset, balanceByAsset.Asset, fromVolume)
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
                        Volume = GetOppositeVolume(portfolioSnapshot ,fromAsset, balanceByAsset.Asset, fromVolume)
                    });
                }
            }
            return response;
        }

        public AssetPortfolio GetPortfolioSnapshot()
        {
            var noSqlEntity = _assetPortfolioBalanceDataReader.Get().FirstOrDefault();
            if (noSqlEntity == null)
                throw new Exception("Asset portfolio not found in NoSql");
            
            return noSqlEntity.Balance;
        }

        // todo: это значение или максимум из того что есть в портфолио (до нуля)
        private decimal GetOppositeVolume(AssetPortfolio portfolioSnapshot, string fromAsset, string toAsset,
            decimal fromVolume)
        {
            var fromAssetPrice = _indexPricesClient.GetIndexPriceByAssetAsync(fromAsset);
            var toAssetPrice = _indexPricesClient.GetIndexPriceByAssetAsync(toAsset);

            var volumeInPortfolio = portfolioSnapshot.BalanceByAsset.FirstOrDefault(e => e.Asset == toAsset)?.NetVolume ?? 0;
            var toVolumeAbs = Math.Abs(fromVolume) * (fromAssetPrice.UsdPrice / toAssetPrice.UsdPrice);
            
            var volumeInPortfolioAbs = Math.Abs(volumeInPortfolio);

            if (fromVolume > 0)
            {

                var response = Math.Max(volumeInPortfolio, toVolumeAbs);

                return Math.Abs(response);
            }
            else
            {
                var response = Math.Min(volumeInPortfolioAbs, toVolumeAbs);

                return response;
            }
        }

        public decimal GetRemainder(AssetPortfolio portfolioSnapshot, AssetPortfolio newPortfolio, string fromAsset, decimal fromAssetVolume)
        {
            var fromAssetBalanceBefore = portfolioSnapshot.BalanceByAsset.FirstOrDefault(e => e.Asset == fromAsset);
            var fromAssetBalanceAfter = newPortfolio.BalanceByAsset.FirstOrDefault(e => e.Asset == fromAsset);
            
            var targetBalance = (fromAssetBalanceBefore?.NetVolume ?? 0) + fromAssetVolume;
            
            var remainder = targetBalance - fromAssetBalanceAfter?.NetVolume;

            return remainder ?? 0;
        }
        
        public AnalysisResult GetAnalysisResult(AssetPortfolio requestPortfolioSnapshot)
        {
            var problemVolume = 10000;
            var mostProblemAsset = requestPortfolioSnapshot.BalanceByAsset
                .Where(e => Math.Abs(e.NetUsdVolume) > problemVolume)
                .OrderByDescending(e => Math.Abs(e.NetUsdVolume))
                .FirstOrDefault();

            return new AnalysisResult()
            {
                HedgeIsNeeded = mostProblemAsset != null,
                FromAsset = mostProblemAsset?.Asset ?? string.Empty,
                FromAssetVolume = -(mostProblemAsset?.NetVolume ?? 0)
            };
        }
    }
}