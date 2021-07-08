using Autofac;
using Service.Liquidity.PortfolioHedger.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.Liquidity.PortfolioHedger.Client
{
    public static class AutofacHelper
    {
        public static void RegisterLiquidity.PortfolioHedgerClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new Liquidity.PortfolioHedgerClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetHelloService()).As<IHelloService>().SingleInstance();
        }
    }
}
