using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.Liquidity.PortfolioHedger.Grpc;

namespace Service.Liquidity.PortfolioHedger.Client
{
    [UsedImplicitly]
    public class PortfolioHedgerClientFactory: MyGrpcClientFactory
    {
        public PortfolioHedgerClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }
        public IManualTradeService GetExternalExchangeTradeService() => CreateGrpcService<IManualTradeService>();
    }
}
