using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain
{
    public interface IOrderBookManager
    {
        public Task<List<Level>> GetAvailableOrdersAsync(ExternalMarket externalMarket, string fromAsset, string toAsset,
            decimal fromVolume, decimal toVolume);
    }
}