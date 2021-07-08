using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;

namespace Service.Liquidity.PortfolioHedger.Client
{
    [UsedImplicitly]
    public class PortfolioHedgerClientFactory: MyGrpcClientFactory
    {
        public PortfolioHedgerClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }
    }
}
