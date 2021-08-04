using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
{
    public interface IExternalExchangeSettingsStorage
    {
        ExternalExchangeSettings GetExternalExchangeSettingsByExchange(string exchange);
        List<ExternalExchangeSettings> GetAExternalExchangeSettings();
        Task UpdateExternalExchangeSettingsAsync(ExternalExchangeSettings settings);
    }
}