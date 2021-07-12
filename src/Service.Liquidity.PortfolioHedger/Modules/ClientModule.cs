using Autofac;
using MyJetWallet.Domain.ExternalMarketApi;

namespace Service.Liquidity.PortfolioHedger.Modules
{
    public class ClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterExternalMarketClient(Program.Settings.ExternalApiGrpcUrl);
        }
    }
}