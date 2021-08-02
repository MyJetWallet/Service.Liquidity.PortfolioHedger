using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain
{
    public interface IHedgePortfolioManager
    {
        public Task<ExecutedVolumes> ExecuteHedgeConvert(string brokerId, string fromAsset, string toAsset, decimal fromAssetVolume, decimal toAssetVolume);
    }
}