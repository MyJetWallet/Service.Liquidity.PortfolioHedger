using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.Liquidity.PortfolioHedger.Grpc;

namespace Service.Liquidity.PortfolioHedger.Client
{
    [UsedImplicitly]
    public class Liquidity.PortfolioHedgerClientFactory: MyGrpcClientFactory
    {
        public Liquidity.PortfolioHedgerClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IHelloService GetHelloService() => CreateGrpcService<IHelloService>();
    }
}
