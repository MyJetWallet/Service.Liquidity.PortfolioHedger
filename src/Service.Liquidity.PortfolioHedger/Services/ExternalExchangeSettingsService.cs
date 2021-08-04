using System.Threading.Tasks;
using Service.Liquidity.PortfolioHedger.Domain.Models;
using Service.Liquidity.PortfolioHedger.Domain.Services;
using Service.Liquidity.PortfolioHedger.Grpc;
using Service.Liquidity.PortfolioHedger.Grpc.Models;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class ExternalExchangeSettingsService : IExternalExchangeSettingsService
    {
        private readonly IExternalExchangeSettingsStorage _externalExchangeSettingsStorage;

        public ExternalExchangeSettingsService(IExternalExchangeSettingsStorage externalExchangeSettingsStorage)
        {
            _externalExchangeSettingsStorage = externalExchangeSettingsStorage;
        }

        public async Task<GetExternalExchangeSettingsResponse> GetExternalExchangeSettingsAsync()
        {
            var settings = _externalExchangeSettingsStorage.GetAExternalExchangeSettings();

            if (settings == null || settings.Count == 0)
                return new GetExternalExchangeSettingsResponse()
                {
                    Success = false,
                    ErrorMessage = "Asset settings not found"
                };
            
            var response = new GetExternalExchangeSettingsResponse()
            {
                Settings = settings,
                Success = true
            };
            return response;
        }

        public Task UpdateExternalExchangeSettingsAsync(ExternalExchangeSettings settings)
        {
            return _externalExchangeSettingsStorage.UpdateExternalExchangeSettingsAsync(settings);
        }
    }
}