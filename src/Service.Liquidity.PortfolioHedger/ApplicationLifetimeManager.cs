using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyNoSqlServer.DataReader;
using MyServiceBus.TcpClient;
using Service.Liquidity.PortfolioHedger.Job;

namespace Service.Liquidity.PortfolioHedger
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly MyServiceBusTcpClient _myServiceBusTcpClient;
        private readonly MyNoSqlTcpClient[] _myNoSqlTcpClientManagers;
        private readonly AssetBalanceStateHandler _assetBalanceStateHandler;

        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime,
            ILogger<ApplicationLifetimeManager> logger,
            MyServiceBusTcpClient myServiceBusTcpClient,
            MyNoSqlTcpClient[] myNoSqlTcpClientManagers,
            AssetBalanceStateHandler assetBalanceStateHandler)
            : base(appLifetime)
        {
            _logger = logger;
            _myServiceBusTcpClient = myServiceBusTcpClient;
            _myNoSqlTcpClientManagers = myNoSqlTcpClientManagers;
            _assetBalanceStateHandler = assetBalanceStateHandler;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _myServiceBusTcpClient.Start();
            foreach(var client in _myNoSqlTcpClientManagers)
            {
                client.Start();
            }
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
            
            foreach(var client in _myNoSqlTcpClientManagers)
            {
                try
                {
                    client.Stop();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
