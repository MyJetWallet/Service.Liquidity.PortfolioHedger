using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.Liquidity.PortfolioHedger.Settings
{
    public class SettingsModel
    {
        [YamlProperty("Liquidity.PortfolioHedger.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("Liquidity.PortfolioHedger.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("Liquidity.PortfolioHedger.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
    }
}
