using System.Text.Json;
using KafkaFlow;
using KafkaSample.Domain.AggregatesModel.CustomerAggregate;
using StackExchange.Redis;

namespace KafkaSample.Transactions;

public class TransactionConsumer(IConnectionMultiplexer connectionMultiplexer) : IMessageHandler<Transaction>
{
    public async Task Handle(IMessageContext context, Transaction message)
    {
        await connectionMultiplexer.GetDatabase()
            .StringSetAsync("transactions:" + message.Id, JsonSerializer.Serialize(message));
    }
}