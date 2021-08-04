using System.Collections.Generic;
using Autofac;
using DotNetCoreDecorators;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;
using Service.Liquidity.PortfolioHedger.Domain.Models;
using Service.Liquidity.PortfolioHedger.Grpc;
using Service.Liquidity.PortfolioHedger.ServiceBus;

// ReSharper disable UnusedMember.Global

namespace Service.Liquidity.PortfolioHedger.Client
{
    public static class AutofacHelper
    {
        public static void RegisterPortfolioHedgerClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new PortfolioHedgerClientFactory(grpcServiceUrl);
            builder.RegisterInstance(factory.GetExternalExchangeTradeService()).As<IManualTradeService>().SingleInstance();
            builder.RegisterInstance(factory.GetHedgePortfolioService()).As<IHedgePortfolioService>().SingleInstance();
        }
        
        public static void RegisterExternalExchangeSettingsServiceClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new PortfolioHedgerClientFactory(grpcServiceUrl);
            builder.RegisterInstance(factory.GetExternalExchangeSettingsService()).As<IExternalExchangeSettingsService>().SingleInstance();
        }
        
        public static void RegisterPortfolioHedgerServiceBusClient(this ContainerBuilder builder, MyServiceBusTcpClient client,
            string queueName,
            TopicQueueType queryType,
            bool batchSubscriber)
        {
            if (batchSubscriber)
            {
                builder
                    .RegisterInstance(new PortfolioHedgerServiceBusSubscriber(client, queueName, queryType, true))
                    .As<ISubscriber<IReadOnlyList<TradeMessage>>>()
                    .SingleInstance();
            }
            else
            {
                builder
                    .RegisterInstance(new PortfolioHedgerServiceBusSubscriber(client, queueName, queryType, false))
                    .As<ISubscriber<TradeMessage>>()
                    .SingleInstance();
            }
        }
    }
}
