using System.Text.Json;
using MediatR;
using StackExchange.Redis;

namespace KafkaSample.WriteStack.Events;

public class TransactionCreated : INotification
{
    public int Id { get; set; }
    /// <summary>
    /// حساب مبدا
    /// </summary>
    public string? FromAccount { get; private set; }

    /// <summary>
    /// حساب مقصد
    /// </summary>
    public string? ToAccount { get; private set; }

    /// <summary>
    /// مبلغ
    /// </summary>
    public long Amount { get; private set; }

    /// <summary>
    /// تاریخ تراکنش 
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

public class ProductCreatedHandler(IConnectionMultiplexer connectionMultiplexer) : INotificationHandler<TransactionCreated>
{
    public async Task Handle(TransactionCreated notification, CancellationToken cancellationToken)
    {
        await Task.Delay(5000, cancellationToken);
        await connectionMultiplexer.GetDatabase()
            .StringSetAsync("products:" + notification.Id, JsonSerializer.Serialize(notification));
    }
}