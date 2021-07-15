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
    public class PortfolioHedgerServiceBusSubscriber : Subscriber<ExchangeTradeMessage>
    {
        public PortfolioHedgerServiceBusSubscriber(MyServiceBusTcpClient client, string queueName, TopicQueueType queryType, bool batchSubscriber) :
            base(client, ExchangeTradeMessage.TopicName, queueName, queryType,
                bytes => bytes.ByteArrayToServiceBusContract<ExchangeTradeMessage>(), batchSubscriber)
        {
        }
    }
}