using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.Liquidity.Portfolio.Grpc.Models.GetBalances;

namespace Service.Liquidity.PortfolioHedger.Job
{
    public class AssetBalanceStateHandler : IStartable
    {
        private readonly ILogger<AssetBalanceStateHandler> _logger;
        
        public AssetBalanceStateHandler(ISubscriber<IReadOnlyList<NetBalanceByAsset>> subscriber,
            ILogger<AssetBalanceStateHandler> logger)
        {
            _logger = logger;
            subscriber.Subscribe(HandleBalances);
        }

        private ValueTask HandleBalances(IReadOnlyList<NetBalanceByAsset> balances)
        {
            _logger.LogInformation($"AssetBalanceStateHandler receive message {balances}");
            return default;
        }

        public void Start()
        {
        }
    }
}