using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain
{
    public interface IExchangeTradeWriter
    {
        public Task PublishTrade(TradeMessage trade);
    }
}