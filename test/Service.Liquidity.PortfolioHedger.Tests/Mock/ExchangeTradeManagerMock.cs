using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Tests.Mock
{
    public class ExchangeTradeManagerMock : IExchangeTradeManager
    {
        public async Task<List<ExternalMarketTrade>> GetTradesByExternalMarkets(List<ExternalMarket> externalMarkets, string fromAsset, string toAsset, decimal fromVolume,
            decimal toVolume)
        {
            var response = new List<ExternalMarketTrade>();

            if (externalMarkets.Select(e=> e.ExchangeName).Contains(StaticFieldsForTests.ExternalMarket1.ExchangeName))
            {
                response.Add(new ExternalMarketTrade()
                {
                    ExchangeName = StaticFieldsForTests.ExternalMarket1.ExchangeName,
                    Market = StaticFieldsForTests.ExternalMarket1.MarketInfo.Market,
                    BaseAsset = StaticFieldsForTests.ExternalMarket1.MarketInfo.BaseAsset,
                    QuoteAsset = StaticFieldsForTests.ExternalMarket1.MarketInfo.QuoteAsset,
                    BaseVolume = 0.15m
                });
            }
            if (externalMarkets.Select(e=> e.ExchangeName).Contains(StaticFieldsForTests.ExternalMarket2.ExchangeName))
            {
                response.Add(new ExternalMarketTrade()
                {
                    ExchangeName = StaticFieldsForTests.ExternalMarket2.ExchangeName,
                    Market = StaticFieldsForTests.ExternalMarket2.MarketInfo.Market,
                    BaseAsset = StaticFieldsForTests.ExternalMarket2.MarketInfo.BaseAsset,
                    QuoteAsset = StaticFieldsForTests.ExternalMarket2.MarketInfo.QuoteAsset,
                    BaseVolume = 0.1m
                });
            }
            return response;
        }
    }
}