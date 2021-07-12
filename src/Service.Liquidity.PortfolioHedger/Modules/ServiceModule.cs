using Autofac;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using Service.Liquidity.Portfolio.Client;
using Service.Liquidity.PortfolioHedger.Job;
using Service.Liquidity.PortfolioHedger.Services;

namespace Service.Liquidity.PortfolioHedger.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.SpotServiceBusHostPort), ApplicationEnvironment.HostName, Program.LogFactory);
            
            builder.RegisterAssetBalanceServiceBusClient(serviceBusClient, $"LiquidityPortfolioHedger-{Program.Settings.ServiceBusQuerySuffix}",
                TopicQueueType.PermanentWithSingleConnection, true);
            
            builder.RegisterMyServiceBusPublisher<ExchangeTrade>(serviceBusClient, ExchangeTradeWriter.TopicName, true);
            
            builder
                .RegisterType<AssetBalanceStateHandler>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();
            
            builder
                .RegisterType<ExchangeTradeWriter>()
                .AsSelf()
                .SingleInstance();
        }
    }
}