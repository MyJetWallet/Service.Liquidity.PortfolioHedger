using System;
using System.Linq;
using System.Threading.Tasks;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using Service.Liquidity.PortfolioHedger.Domain.Models;

namespace Service.Liquidity.PortfolioHedger.Domain.Services
{
    public class OrderBookManager : IOrderBookManager
    {
        private readonly IOrderBookSource _orderBookSource;
        private readonly IExternalMarket _externalMarket;

        public OrderBookManager(IOrderBookSource orderBookSource,
            IExternalMarket externalMarket)
        {
            _orderBookSource = orderBookSource;
            _externalMarket = externalMarket;
        }

        // todo: вынести в отдельный класс
        // если а - base, b - quote - то берем BUY ордера 
        // смотрим баланс, если баланса меньше - то уменьшаем объем до баланса
        // берем то что меньше - баланс или требуемый объем
        public async Task<bool> GetAvailableOrdersAsync(ExternalMarket externalMarket, string fromAsset, string toAsset, decimal fromVolume,
            decimal toVolume)
        {
            // берем ордербук
            var orderbook = await GetOrderBookFromExchangeAsync(externalMarket, fromAsset, toAsset);

            //обрезаем ордербук по имеющимся балансам
            var sortedByBalance = await GetSortedBookByBalanceAsync(externalMarket, orderbook);
            
            //обрезаем ордербук по требуемому полному обьему сделки
            var sortedByVolume = GetSortedBookByVolume(sortedByBalance);
            
            return false;
        }
        
        private async Task<LeOrderBook> GetOrderBookFromExchangeAsync(ExternalMarket externalMarket, string fromAsset, string toAsset)
        {
            var response = await _orderBookSource.GetOrderBookAsync(new MarketRequest()
            {
                ExchangeName = externalMarket.Exchange,
                Market = externalMarket.MarketInfo.Market
            });

            return response.OrderBook;
        }
        
        private async Task<LeOrderBook> GetSortedBookByBalanceAsync(ExternalMarket externalMarket, LeOrderBook orderBook)
        {
            var balances = await _externalMarket.GetBalancesAsync(new GetBalancesRequest()
            {
                ExchangeName = externalMarket.Exchange
            });

            var balanceByMarket = balances.Balances.FirstOrDefault(e => e.Symbol == externalMarket.MarketInfo.Market);

            decimal availableBalance = balanceByMarket != null
                ? availableBalance = balanceByMarket.Balance - Convert.ToDecimal(externalMarket.MarketInfo.MinVolume)
                : 0;
            
            return null;
        }
        
        private object GetSortedBookByVolume(object sortedByBalance)
        {
            throw new NotImplementedException();
        }
    }
}