using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using Service.Liquidity.PortfolioHedger.Job;

namespace Service.Liquidity.PortfolioHedger
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly ServiceBusLifeTime _myServiceBusTcpClient;
        private readonly MyNoSqlClientLifeTime _myNoSqlClientLifeTime;
        private readonly AssetBalanceStateHandler _assetBalanceStateHandler;

        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime,
            ILogger<ApplicationLifetimeManager> logger,
            ServiceBusLifeTime myServiceBusTcpClient,
            MyNoSqlClientLifeTime myNoSqlClientLifeTime,
            AssetBalanceStateHandler assetBalanceStateHandler)
            : base(appLifetime)
        {
            _logger = logger;
            _myServiceBusTcpClient = myServiceBusTcpClient;
            _myNoSqlClientLifeTime = myNoSqlClientLifeTime;
            _assetBalanceStateHandler = assetBalanceStateHandler;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _myServiceBusTcpClient.Start();
            _myNoSqlClientLifeTime.Start();
            _assetBalanceStateHandler.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            try
            {
                _myServiceBusTcpClient.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception on MyServiceBusTcpClient.Stop: {ex}");
            }
            _myNoSqlClientLifeTime.Stop();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
