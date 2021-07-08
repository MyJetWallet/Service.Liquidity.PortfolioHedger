using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.Liquidity.PortfolioHedger.Settings
{
    public class SettingsModel
    {
        [YamlProperty("LiquidityPortfolioHedger.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("LiquidityPortfolioHedger.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("LiquidityPortfolioHedger.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("LiquidityPortfolioHedger.SpotServiceBusHostPort")]
        public string SpotServiceBusHostPort { get; set; }

        [YamlProperty("LiquidityPortfolioHedger.ServiceBusQuerySuffix")]
        public string ServiceBusQuerySuffix { get; set; }
    }
}
