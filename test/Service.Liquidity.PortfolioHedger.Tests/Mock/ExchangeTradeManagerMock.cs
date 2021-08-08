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

            if (externalMarkets.Select(e=> e.ExchangeName).Contains(TesterBase.ExternalMarket1.ExchangeName))
            {
                response.Add(new ExternalMarketTrade()
                {
                    ExchangeName = TesterBase.ExternalMarket1.ExchangeName,
                    Market = TesterBase.ExternalMarket1.MarketInfo.Market,
                    BaseAsset = TesterBase.ExternalMarket1.MarketInfo.BaseAsset,
                    QuoteAsset = TesterBase.ExternalMarket1.MarketInfo.QuoteAsset,
                    BaseVolume = 0.15m
                });
            }
            if (externalMarkets.Select(e=> e.ExchangeName).Contains(TesterBase.ExternalMarket2.ExchangeName))
            {
                response.Add(new ExternalMarketTrade()
                {
                    ExchangeName = TesterBase.ExternalMarket2.ExchangeName,
                    Market = TesterBase.ExternalMarket2.MarketInfo.Market,
                    BaseAsset = TesterBase.ExternalMarket2.MarketInfo.BaseAsset,
                    QuoteAsset = TesterBase.ExternalMarket2.MarketInfo.QuoteAsset,
                    BaseVolume = 0.1m
                });
            }
            return response;
        }
    }
}