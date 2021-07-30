using Autofac;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using Service.Liquidity.Monitoring.Domain.Models;
using Service.Liquidity.Portfolio.Domain.Models;
using Service.Liquidity.PortfolioHedger.Domain.Models;
using Service.Liquidity.PortfolioHedger.Domain.Services;
using Service.Liquidity.PortfolioHedger.Job;
using Service.Liquidity.PortfolioHedger.ServiceBus;
using Service.Liquidity.PortfolioHedger.Services;

namespace Service.Liquidity.PortfolioHedger.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.SpotServiceBusHostPort), ApplicationEnvironment.HostName, Program.LogFactory);
            builder.RegisterMyServiceBusPublisher<TradeMessage>(serviceBusClient, TradeMessage.TopicName, true);
            
            var noSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));
            builder.RegisterMyNoSqlReader<AssetPortfolioBalanceNoSql>(noSqlClient, AssetPortfolioBalanceNoSql.TableName);
            builder.RegisterMyNoSqlReader<AssetPortfolioStatusNoSql>(noSqlClient, AssetPortfolioStatusNoSql.TableName);

            builder
                .RegisterType<AssetBalanceStateHandler>()
                .AsSelf()
                .SingleInstance();
            
            builder
                .RegisterType<ExchangeTradeWriter>()
                .AsSelf()
                .SingleInstance();
            
            builder
                .RegisterType<HedgerMetrics>()
                .AsSelf()
                .SingleInstance();
            
            builder
                .RegisterType<HedgePortfolioManager>()
                .As<IHedgePortfolioManager>()
                .SingleInstance();
        }
    }
}