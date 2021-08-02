using Autofac;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Sdk.NoSql;
using Service.AssetsDictionary.Client;
using Service.IndexPrices.Client;

namespace Service.Liquidity.PortfolioHedger.Modules
{
    public class ClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var myNoSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));
            builder.RegisterAssetsDictionaryClients(myNoSqlClient);
            
            builder.RegisterExternalMarketClient(Program.Settings.ExternalApiGrpcUrl);
            builder.RegisterIndexPricesClient(myNoSqlClient);
        }
    }
}