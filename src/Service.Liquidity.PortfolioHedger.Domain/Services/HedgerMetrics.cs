using Prometheus;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
{
    public class HedgerMetrics
    {
        private static readonly Counter ManualTradeReportCounter = Metrics
            .CreateCounter("jet_portfolio_report_trade",
                "Report manual trade count.",
                new CounterConfiguration{ LabelNames = new []{"broker", "wallet", "market"}});
        
        private static readonly Counter ManualTradeExecuteCounter = Metrics
            .CreateCounter("jet_portfolio_make_trade",
                "Execute manual trade count.",
                new CounterConfiguration{ LabelNames = new []{"broker", "wallet", "market"}});


        public void SetReportTradeMetrics(ReportManualTradeRequest tradeRequest)
        {
            ManualTradeReportCounter
                .WithLabels(tradeRequest.BrokerId, tradeRequest.WalletName, tradeRequest.AssociateSymbol)
                .Inc();
        }
        
        public void SetExecuteTradeMetrics(CreateManualTradeRequest tradeRequest)
        {
            ManualTradeExecuteCounter
                .WithLabels(tradeRequest.AssociateBrokerId, tradeRequest.ExchangeName, tradeRequest.Market)
                .Inc();
        }
    }
}
