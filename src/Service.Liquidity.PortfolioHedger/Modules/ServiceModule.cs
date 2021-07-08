using Autofac;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using Service.Liquidity.Portfolio.Client;
using Service.Liquidity.PortfolioHedger.Job;

namespace Service.Liquidity.PortfolioHedger.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.SpotServiceBusHostPort), ApplicationEnvironment.HostName, Program.LogFactory);
            builder.RegisterAssetBalanceServiceBusClient(serviceBusClient, $"LiquidityPortfolioHedger-{Program.Settings.ServiceBusQuerySuffix}",
                TopicQueueType.PermanentWithSingleConnection, true);
            
            builder
                .RegisterType<AssetBalanceStateHandler>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}