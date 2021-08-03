using System;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Newtonsoft.Json;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Tests.Mock
{
    public class ExchangeTradeWriterMock : IExchangeTradeWriter
    {
        public async Task PublishTrade(TradeMessage trade)
        {
            Console.WriteLine($"ExchangeTradeWriterMock PublishTrade : {JsonConvert.SerializeObject(trade)}\n");
        }
    }
}