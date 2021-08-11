namespace Service.Liquidity.PortfolioHedger.Services
{
    public class AnalysisResult
    {
        public bool HedgeIsNeeded { get; set; }
        public string FromAsset { get; set; }
        public decimal FromAssetVolume { get; set; }
    }
}