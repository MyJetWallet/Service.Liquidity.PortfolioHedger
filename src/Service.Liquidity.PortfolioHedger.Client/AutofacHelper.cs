using Autofac;
using Service.Liquidity.PortfolioHedger.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.Liquidity.PortfolioHedger.Client
{
    public static class AutofacHelper
    {
        public static void RegisterPortfolioHedgerClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new PortfolioHedgerClientFactory(grpcServiceUrl);
            builder.RegisterInstance(factory.GetExternalExchangeTradeService()).As<IExternalExchangeTradeService>().SingleInstance();
        }
    }
}
