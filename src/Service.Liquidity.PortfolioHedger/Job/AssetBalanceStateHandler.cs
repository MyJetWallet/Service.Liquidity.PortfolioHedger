using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Service.Liquidity.Portfolio.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Job
{
    public class AssetBalanceStateHandler
    {
        private readonly ILogger<AssetBalanceStateHandler> _logger;
        private readonly IMyNoSqlServerDataReader<AssetPortfolioBalanceNoSql> _assetPortfolioBalanceDataReader;

        public AssetBalanceStateHandler(ILogger<AssetBalanceStateHandler> logger,
            IMyNoSqlServerDataReader<AssetPortfolioBalanceNoSql> assetPortfolioBalanceDataReader)
        {
            _logger = logger;
            _assetPortfolioBalanceDataReader = assetPortfolioBalanceDataReader;
        }

        public void Start()
        {
            _assetPortfolioBalanceDataReader.SubscribeToUpdateEvents(HandleUpdateBalance, HandleDeleteBalance);
            var t = _assetPortfolioBalanceDataReader.Get();
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