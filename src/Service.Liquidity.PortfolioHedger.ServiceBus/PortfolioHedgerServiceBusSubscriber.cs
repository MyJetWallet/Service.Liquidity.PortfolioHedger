using System;
using JetBrains.Annotations;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using MyJetWallet.Domain.ServiceBus;
using MyJetWallet.Domain.ServiceBus.Serializers;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;

namespace Service.Liquidity.PortfolioHedger.ServiceBus
{
    [UsedImplicitly]
    public class PortfolioHedgerServiceBusSubscriber : Subscriber<ExchangeTrade>
    {
        public const string TopicName = "trade-hedger";
        
        public PortfolioHedgerServiceBusSubscriber(MyServiceBusTcpClient client, string queueName, TopicQueueType queryType, bool batchSubscriber) :
            base(client, TopicName, queueName, queryType,
                bytes => bytes.ByteArrayToServiceBusContract<ExchangeTrade>(), batchSubscriber)
        {
        }
    }
}