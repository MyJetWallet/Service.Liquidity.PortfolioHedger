using System.Collections.Generic;
using System.Linq;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using Service.IndexPrices.Domain.Models;
using Service.Liquidity.Portfolio.Domain.Models;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Models;
using Service.Liquidity.PortfolioHedger.Services;
using Service.Liquidity.PortfolioHedger.Tests.Mock;

namespace Service.Liquidity.PortfolioHedger.Tests
{
    public class TesterBase
    {
        protected IHedgePortfolioManager HedgePortfolioManager;
        protected ExchangeTradeWriterMock ExchangeTradeWriter;
        protected ExternalExchangeManagerMock ExternalExchangeManager;
        protected IExchangeTradeManager ExchangeTradeManager;
        protected IOrderBookManager OrderBookManager;
        protected OrderBookSourceMock OrderBookSource;
        protected ExternalMarketMock ExternalMarket;
        protected PortfolioStorageMock PortfolioStorage;
        protected PortfolioHandler PortfolioHandler;
        protected IndexPricesClientMock IndexPricesClientMock;
        
        public static ExternalMarket ExternalMarket1 = new ExternalMarket()
        {
            ExchangeName = "exchange1",
            MarketInfo = new ExchangeMarketInfo()
            {
                AssociateBaseAsset = "BTC",
                AssociateInstrument = "BTCUSD",
                AssociateQuoteAsset = "USD",
                BaseAsset = "BTC",
                QuoteAsset = "USD",
                Market = "BTCUSD",
                MinVolume = 0.01,
                PriceAccuracy = 2,
                VolumeAccuracy = 8
            }
        };
        public static ExternalMarket ExternalMarket2 = new ExternalMarket()
        {
            ExchangeName = "exchange2",
            MarketInfo = new ExchangeMarketInfo()
            {
                AssociateBaseAsset = "BTC",
                AssociateInstrument = "BTCUSD",
                AssociateQuoteAsset = "USD",
                BaseAsset = "BTC",
                QuoteAsset = "USD",
                Market = "BTCUSD",
                MinVolume = 0.01,
                PriceAccuracy = 2,
                VolumeAccuracy = 8
            }
        };
        public static string FromAsset = "BTC";
        public static string ToAsset = "USD";
        public static decimal FromVolume = 0.25m;
        public static decimal ToVolume = 40000m;
        
        
        protected void SetBalance(string exchange, string asset, int volume)
        {
            var balances = ExternalMarket.Balances;
            
            if (balances.TryGetValue(exchange, out var balancesByExchange))
            {
                var balanceByAsset = balancesByExchange.FirstOrDefault(e => e.Symbol == asset);
                if (balanceByAsset != null)
                {
                    balanceByAsset.Balance = volume;
                }
                else
                {
                    balancesByExchange.Add(new ExchangeBalance()
                    {
                        Symbol = asset,
                        Balance = volume
                    });
                }
            }
            else
            {
                balances.Add(exchange, new List<ExchangeBalance>()
                {
                    new ExchangeBalance()
                    {
                        Symbol = asset,
                        Balance = volume
                    }
                });
            }
        }
        
        protected void SetPrices(string exchange, string baseAsset, string quoteAsset, int priceInQuoteAsset)
        {
            var orderBook = OrderBookSource.OrderBooks;
            var instrumentSymbol = baseAsset + quoteAsset;
            
            if (orderBook.TryGetValue(exchange, out var bookByExchange))
            {
                var bookByInstrument = bookByExchange.FirstOrDefault(e => e.Symbol == instrumentSymbol);
                if (bookByInstrument != null)
                {
                    bookByInstrument = new LeOrderBook()
                    {
                        Symbol = instrumentSymbol,
                        Asks = new List<LeOrderBookLevel>()
                        {
                            new LeOrderBookLevel(priceInQuoteAsset, 100000d)
                        },
                        Bids = new List<LeOrderBookLevel>()
                        {
                            new LeOrderBookLevel(priceInQuoteAsset, 100000d)
                        }
                    };
                }
                else
                {
                    bookByExchange.Add(new LeOrderBook()
                    {
                        Symbol = instrumentSymbol,
                        Asks = new List<LeOrderBookLevel>()
                        {
                            new LeOrderBookLevel(priceInQuoteAsset, 100000d)
                        },
                        Bids = new List<LeOrderBookLevel>()
                        {
                            new LeOrderBookLevel(priceInQuoteAsset, 100000d)
                        }
                    });
                }
            }
            else
            {
                orderBook.Add(exchange, new List<LeOrderBook>()
                {
                    new LeOrderBook()
                    {
                        Symbol = instrumentSymbol,
                        Asks = new List<LeOrderBookLevel>()
                        {
                            new LeOrderBookLevel(priceInQuoteAsset, 100000d)
                        },
                        Bids = new List<LeOrderBookLevel>()
                        {
                            new LeOrderBookLevel(priceInQuoteAsset, 100000d)
                        }
                    }
                });
            }
        }
        
        protected void SetPortfolio(string asset, decimal volume)
        {
            var portfolio = PortfolioStorage.Portfolio;
            
            var balanceByAsset = portfolio.BalanceByAsset.FirstOrDefault(e => e.Asset == asset);

            if (balanceByAsset != null)
            {
                balanceByAsset.NetVolume = volume;
                balanceByAsset.NetUsdVolume = (IndexPricesClientMock.GetIndexPriceByAssetAsync(asset)?.UsdPrice ?? 0) * volume;
            }
            else
            {
                portfolio.BalanceByAsset.Add(new NetBalanceByAsset()
                {
                    Asset = asset,
                    NetVolume = volume,
                    NetUsdVolume = (IndexPricesClientMock.GetIndexPriceByAssetAsync(asset)?.UsdPrice ?? 0) * volume
                });
            }
        }
        
        protected decimal GetBalance(string exchange, string asset)
        {
            var balances = ExternalMarket.Balances;

            if (balances.TryGetValue(exchange, out var balancesByExchange))
            {
                var balanceByAsset = balancesByExchange.FirstOrDefault(e => e.Symbol == asset);

                return balanceByAsset?.Balance ?? 0;
            }

            return 0;
        }
        
        protected void SetMarketInfos(Dictionary<string, List<ExchangeMarketInfo>> dictionary)
        {
            ExternalMarket.MarketInfos = dictionary;
        }

        protected void SetExchanges(List<string> exchanges)
        {
            ExternalExchangeManager.ExchangeNames = exchanges;
        }

        protected void SetIndexPrice(string asset, decimal price)
        {
            var prices = IndexPricesClientMock.IndexPrices;
            
            if (prices.TryGetValue(asset, out var indexPrice))
            {
                indexPrice.UsdPrice = price;
            }
            else
            {
                prices.Add(asset, new IndexPrice()
                {
                    Asset = asset,
                    UsdPrice = price
                });
            }
        }
        
        protected AssetPortfolio GetPortfolioSnapshot()
        {
            return PortfolioStorage.Portfolio;
        }

        protected decimal GetPortfolioBalance(string asset)
        {
            var portfolio = PortfolioStorage.Portfolio;
            
            var balanceByAsset = portfolio.BalanceByAsset.FirstOrDefault(e => e.Asset == asset);

            return balanceByAsset?.NetVolume ?? 0;
        }
    }
}