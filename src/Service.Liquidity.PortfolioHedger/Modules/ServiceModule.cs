using Autofac;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using Service.Liquidity.Portfolio.Domain.Models;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Models;
using Service.Liquidity.PortfolioHedger.Domain.Services;
using Service.Liquidity.PortfolioHedger.Job;
using Service.Liquidity.PortfolioHedger.Services;

namespace Service.Liquidity.PortfolioHedger.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.SpotServiceBusHostPort), Program.LogFactory);
            builder.RegisterMyServiceBusPublisher<TradeMessage>(serviceBusClient, TradeMessage.TopicName, true);
            
            var noSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));
            builder.RegisterMyNoSqlReader<AssetPortfolioBalanceNoSql>(noSqlClient, AssetPortfolioBalanceNoSql.TableName);
            
            builder.RegisterMyNoSqlWriter<ExternalExchangeSettingsNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), ExternalExchangeSettingsNoSql.TableName);

            builder
                .RegisterType<AssetBalanceStateHandler>()
                .AsSelf()
                .SingleInstance();
            
            builder
                .RegisterType<ExchangeTradeWriter>()
                .As<IExchangeTradeWriter>()
                .SingleInstance();
            
            builder
                .RegisterType<HedgerMetrics>()
                .AsSelf()
                .SingleInstance();
            builder
                .RegisterType<PortfolioHandler>()
                .AsSelf()
                .SingleInstance();
            
            builder
                .RegisterType<HedgePortfolioCalculator>()
                .As<IHedgePortfolioCalculator>()
                .SingleInstance();
            builder
                .RegisterType<ExternalMarketTradesExecutor>()
                .As<IExternalMarketTradesExecutor>()
                .SingleInstance();
            builder
                .RegisterType<ExchangeTradeManager>()
                .As<IExchangeTradeManager>()
                .SingleInstance();
            builder
                .RegisterType<OrderBookManager>()
                .As<IOrderBookManager>()
                .SingleInstance();
            
            
            builder
                .RegisterType<ExternalExchangeSettingsStorage>()
                .As<IExternalExchangeSettingsStorage>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}