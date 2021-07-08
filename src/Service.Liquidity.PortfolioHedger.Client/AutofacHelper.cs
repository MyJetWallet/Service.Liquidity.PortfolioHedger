using Autofac;

// ReSharper disable UnusedMember.Global

namespace Service.Liquidity.PortfolioHedger.Client
{
    public static class AutofacHelper
    {
        public static void PortfolioHedgerClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new PortfolioHedgerClientFactory(grpcServiceUrl);
        }
    }
}
