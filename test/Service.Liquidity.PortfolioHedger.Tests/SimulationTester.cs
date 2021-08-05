using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using NUnit.Framework;
using Service.IndexPrices.Domain.Models;
using Service.Liquidity.Portfolio.Domain.Models;
using Service.Liquidity.PortfolioHedger.Domain;
using Service.Liquidity.PortfolioHedger.Domain.Services;
using Service.Liquidity.PortfolioHedger.Grpc.Models;
using Service.Liquidity.PortfolioHedger.Services;
using Service.Liquidity.PortfolioHedger.Tests.Mock;

namespace Service.Liquidity.PortfolioHedger.Tests
{
    public class SimulationTester
    {
        private HedgePortfolioService _hedgePortfolioService;
        private IHedgePortfolioManager _hedgePortfolioManager;
        private ExchangeTradeWriterMock _exchangeTradeWriter;
        private ExternalExchangeManagerMock _externalExchangeManager;
        private IExchangeTradeManager _exchangeTradeManager;
        private IOrderBookManager _orderBookManager;
        private OrderBookSourceMock _orderBookSource;
        private ExternalMarketMock _externalMarket;
        private PortfolioStorageMock _portfolioStorage;
        private HedgePortfolioHelper _hedgePortfolioHelper;
        private PortfolioReaderMock _portfolioReaderMock;
        private IndexPricesClientMock _indexPricesClientMock;

        [SetUp]
        public void Setup()
        {
            _externalMarket = GetExternalMarket();
            _orderBookManager = GetOrderBookManager(_externalMarket);
            _exchangeTradeManager = GetExchangeTradeManager(_orderBookManager);
            _hedgePortfolioManager = GetHedgePortfolioManager(_exchangeTradeManager, _externalMarket);
            _portfolioStorage = new PortfolioStorageMock();
            _portfolioReaderMock = new PortfolioReaderMock(_portfolioStorage);
            _indexPricesClientMock = new IndexPricesClientMock();
            _hedgePortfolioHelper = new HedgePortfolioHelper(_indexPricesClientMock, _portfolioReaderMock);
            _hedgePortfolioService = new HedgePortfolioService(_hedgePortfolioManager, _hedgePortfolioHelper);
        }

        private ExternalMarketMock GetExternalMarket()
        {
            return new ExternalMarketMock()
            {
                Balances = new Dictionary<string, List<ExchangeBalance>>()
            };
        }

        private IHedgePortfolioManager GetHedgePortfolioManager(IExchangeTradeManager exchangeTradeManager,
            IExternalMarket externalMarket)
        {
            _exchangeTradeWriter = new ExchangeTradeWriterMock();
            _externalExchangeManager = new ExternalExchangeManagerMock();

            return new HedgePortfolioManager(exchangeTradeManager, _exchangeTradeWriter,
                _externalExchangeManager, externalMarket);
        }

        private IExchangeTradeManager GetExchangeTradeManager(IOrderBookManager orderBookManager)
        {
            return new ExchangeTradeManager(orderBookManager);
        }

        private IOrderBookManager GetOrderBookManager(IExternalMarket externalMarket)
        {
            _orderBookSource = new OrderBookSourceMock()
            {
                OrderBooks = new Dictionary<string, List<LeOrderBook>>()
            };
            return new OrderBookManager(_orderBookSource, externalMarket);
        }

        [Test]
        public async Task Test1()
        {
            SetBalance("Binance", "BTC", 5);
            SetBalance("Binance", "USD", 1000000);
            SetBalance("FTX", "BTC", 5);
            SetBalance("FTX", "USD", 1000000);
            
            SetPrices("Binance", "BTC", "USD", 30000);
            SetPrices("Binance", "ETH", "USD", 3000);
            SetPrices("FTX", "BTC", "USD", 30000);
            SetPrices("FTX", "ETH", "USD", 3000);

            SetIndexPrice("BTC", 30000);
            
            SetPortfolio("BTC", -1);
            SetPortfolio("USD", 30100);
            
            // Hedge +1 BTC ExecuteAutoConvert: FROM BTC, FROMVALUE -1
            await _hedgePortfolioService.ExecuteAutoConvert(new ExecuteAutoConvertRequest()
            {
                FromAsset = "BTC",
                FromAssetVolume = -1
            });
            
            
            //Assert.AreEqual(0, GetPortfolioBalance("BTC"));
            //Assert.AreEqual(100, GetPortfolioBalance("USD"));
            //
            //Assert.AreEqual(6, GetBalance("Binance", "BTC"));
        }

        private void SetIndexPrice(string asset, decimal price)
        {
            var prices = _indexPricesClientMock.IndexPrices;
            
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

        private decimal GetPortfolioBalance(string asset)
        {
            var portfolio = _portfolioStorage.Portfolio;
            
            var balanceByAsset = portfolio.BalanceByAsset.FirstOrDefault(e => e.Asset == asset);

            return balanceByAsset?.NetVolume ?? 0;
        }

        private decimal GetBalance(string exchange, string asset)
        {
            var balances = _externalMarket.Balances;

            if (balances.TryGetValue(exchange, out var balancesByExchange))
            {
                var balanceByAsset = balancesByExchange.FirstOrDefault(e => e.Symbol == asset);

                return balanceByAsset?.Balance ?? 0;
            }

            return 0;
        }

        private void SetPortfolio(string asset, decimal volume)
        {
            var portfolio = _portfolioStorage.Portfolio;
            
            var balanceByAsset = portfolio.BalanceByAsset.FirstOrDefault(e => e.Asset == asset);

            if (balanceByAsset != null)
            {
                balanceByAsset.NetVolume = volume;
                balanceByAsset.NetUsdVolume = _indexPricesClientMock.GetIndexPriceByAssetAsync(asset)?.UsdPrice ?? 0;
            }
            else
            {
                portfolio.BalanceByAsset.Add(new NetBalanceByAsset()
                {
                    Asset = asset,
                    NetVolume = volume,
                    NetUsdVolume = _indexPricesClientMock.GetIndexPriceByAssetAsync(asset)?.UsdPrice ?? 0
                });
            }
        }

        private void SetPrices(string exchange, string baseAsset, string quoteAsset, int priceInQuoteAsset)
        {
            var orderBook = _orderBookSource.OrderBooks;
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

        private void SetBalance(string exchange, string asset, int volume)
        {
            var balances = _externalMarket.Balances;
            
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
    }
}