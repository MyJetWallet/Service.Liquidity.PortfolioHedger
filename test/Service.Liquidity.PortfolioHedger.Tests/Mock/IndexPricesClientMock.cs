using System.Collections.Generic;
using Service.IndexPrices.Client;
using Service.IndexPrices.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Tests.Mock
{
    public class IndexPricesClientMock : IIndexPricesClient
    {
        public Dictionary<string, IndexPrice> IndexPrices = new Dictionary<string, IndexPrice>();
        public IndexPrice GetIndexPriceByAssetAsync(string asset)
        {
            if (IndexPrices.TryGetValue(asset, out var value))
            {
                return value;
            }

            return null;
        }

        public (IndexPrice, decimal) GetIndexPriceByAssetVolumeAsync(string asset, decimal volume)
        {
            throw new System.NotImplementedException();
        }

        public List<IndexPrice> GetIndexPricesAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}