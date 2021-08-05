using System;
using System.Collections.Generic;
using MyNoSqlServer.Abstractions;
using Service.Liquidity.Portfolio.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Tests.Mock
{
    public class PortfolioReaderMock : IMyNoSqlServerDataReader<AssetPortfolioBalanceNoSql>
    {

        private PortfolioStorageMock _portfolioStorageMock;

        public PortfolioReaderMock(PortfolioStorageMock portfolioStorageMock)
        {
            _portfolioStorageMock = portfolioStorageMock;
        }

        public AssetPortfolioBalanceNoSql Get(string partitionKey, string rowKey)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<AssetPortfolioBalanceNoSql> Get(string partitionKey)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<AssetPortfolioBalanceNoSql> Get(string partitionKey, int skip, int take)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<AssetPortfolioBalanceNoSql> Get(string partitionKey, int skip, int take, Func<AssetPortfolioBalanceNoSql, bool> condition)
        {
            return new List<AssetPortfolioBalanceNoSql>()
            {
                new AssetPortfolioBalanceNoSql()
                {
                    Balance = _portfolioStorageMock.Portfolio
                }
            };
        }

        public IReadOnlyList<AssetPortfolioBalanceNoSql> Get(string partitionKey, Func<AssetPortfolioBalanceNoSql, bool> condition)
        {
            return new List<AssetPortfolioBalanceNoSql>()
            {
                new AssetPortfolioBalanceNoSql()
                {
                    Balance = _portfolioStorageMock.Portfolio
                }
            };
        }

        public IReadOnlyList<AssetPortfolioBalanceNoSql> Get(Func<AssetPortfolioBalanceNoSql, bool> condition = null)
        {
            return new List<AssetPortfolioBalanceNoSql>()
            {
                new AssetPortfolioBalanceNoSql()
                {
                    Balance = _portfolioStorageMock.Portfolio
                }
            };
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(string partitionKey)
        {
            throw new NotImplementedException();
        }

        public int Count(string partitionKey, Func<AssetPortfolioBalanceNoSql, bool> condition)
        {
            throw new NotImplementedException();
        }

        public IMyNoSqlServerDataReader<AssetPortfolioBalanceNoSql> SubscribeToUpdateEvents(Action<IReadOnlyList<AssetPortfolioBalanceNoSql>> updateSubscriber, Action<IReadOnlyList<AssetPortfolioBalanceNoSql>> deleteSubscriber)
        {
            throw new NotImplementedException();
        }
    }
}