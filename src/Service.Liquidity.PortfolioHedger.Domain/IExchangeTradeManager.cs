using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain
{
    public interface IExchangeTradeManager
    {
        public Task<List<ExternalMarketTrade>> GetTradesByExternalMarkets(string fromAsset, string toAsset, decimal fromVolume,
            decimal toVolume);
    }
}