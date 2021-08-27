using System.Collections.Generic;
using Service.Liquidity.Portfolio.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Tests.Mock
{
    public class PortfolioStorageMock
    {
        public AssetPortfolio Portfolio = new AssetPortfolio()
        {
            BalanceByAsset = new List<BalanceByAsset>()
        };
    }
}