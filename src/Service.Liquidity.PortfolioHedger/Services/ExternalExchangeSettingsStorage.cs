using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Newtonsoft.Json;
using Service.Liquidity.PortfolioHedger.Domain.Models;
using Service.Liquidity.PortfolioHedger.Domain.Services;

namespace Service.Liquidity.PortfolioHedger.Services
{
    public class ExternalExchangeSettingsStorage : IExternalExchangeSettingsStorage, IStartable
    {
        private readonly ILogger<ExternalExchangeSettingsStorage> _logger;
        private readonly IMyNoSqlServerDataWriter<ExternalExchangeSettingsNoSql> _settingsDataWriter;
        
        private Dictionary<string, ExternalExchangeSettings> _settings = new Dictionary<string, ExternalExchangeSettings>();

        public ExternalExchangeSettingsStorage(ILogger<ExternalExchangeSettingsStorage> logger,
            IMyNoSqlServerDataWriter<ExternalExchangeSettingsNoSql> settingsDataWriter)
        {
            _logger = logger;
            _settingsDataWriter = settingsDataWriter;
        }

        public ExternalExchangeSettings GetExternalExchangeSettingsByExchange(string exchange)
        {
            return _settings.TryGetValue(exchange, out var result) ? result : null;
        }

        public List<ExternalExchangeSettings> GetAExternalExchangeSettings()
        {
            return _settings.Values.ToList();
        }

        public async Task UpdateExternalExchangeSettingsAsync(ExternalExchangeSettings settings)
        {
            await _settingsDataWriter.InsertOrReplaceAsync(ExternalExchangeSettingsNoSql.Create(settings));

            await ReloadSettings();

            _logger.LogInformation("Updated ExternalExchangeSettings Settings: {jsonText}",
                JsonConvert.SerializeObject(settings));
        }

        public void Start()
        {
            ReloadSettings().GetAwaiter().GetResult();
        }
        
        private async Task ReloadSettings()
        {
            var settings = (await _settingsDataWriter.GetAsync()).ToList();

            var settingsMap = new Dictionary<string, ExternalExchangeSettings>();
            foreach (var settingsLiquidityConverterNoSql in settings)
            {
                settingsMap[settingsLiquidityConverterNoSql.Settings.ExchangeName] =
                    settingsLiquidityConverterNoSql.Settings;
            }

            _settings = settingsMap;
        }
    }
}