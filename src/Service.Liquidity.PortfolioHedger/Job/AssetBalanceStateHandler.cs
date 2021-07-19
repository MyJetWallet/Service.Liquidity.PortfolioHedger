using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Service.Liquidity.Monitoring.Domain.Models;
using Service.Liquidity.Portfolio.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Job
{
    public class AssetBalanceStateHandler
    {
        private readonly ILogger<AssetBalanceStateHandler> _logger;
        private readonly IMyNoSqlServerDataReader<AssetPortfolioBalanceNoSql> _assetPortfolioBalanceDataReader;
        private readonly IMyNoSqlServerDataReader<AssetPortfolioStatusNoSql> _assetPortfolioStatusDataReader;
        
        public AssetBalanceStateHandler(ILogger<AssetBalanceStateHandler> logger,
            IMyNoSqlServerDataReader<AssetPortfolioBalanceNoSql> assetPortfolioBalanceDataReader,
            IMyNoSqlServerDataReader<AssetPortfolioStatusNoSql> assetPortfolioStatusDataReader)
        {
            _logger = logger;
            _assetPortfolioBalanceDataReader = assetPortfolioBalanceDataReader;
            _assetPortfolioStatusDataReader = assetPortfolioStatusDataReader;
        }

        public void Start()
        {
            _assetPortfolioBalanceDataReader.SubscribeToUpdateEvents(HandleUpdateBalance, HandleDeleteBalance);
            _assetPortfolioStatusDataReader.SubscribeToUpdateEvents(HandleUpdateStatus, HandleDeleteStatus);

            var t = _assetPortfolioBalanceDataReader.Get();
            var n = _assetPortfolioStatusDataReader.Get();
        }

        private void HandleDeleteStatus(IReadOnlyList<AssetPortfolioStatusNoSql> statuses)
        {
        }

        private void HandleUpdateStatus(IReadOnlyList<AssetPortfolioStatusNoSql> statuses)
        {
        }

        private void HandleDeleteBalance(IReadOnlyList<AssetPortfolioBalanceNoSql> balances)
        {
        }

        private void HandleUpdateBalance(IReadOnlyList<AssetPortfolioBalanceNoSql> balances)
        {
            _logger.LogInformation($"AssetBalanceStateHandler receive message {balances}");
        }
    }
}