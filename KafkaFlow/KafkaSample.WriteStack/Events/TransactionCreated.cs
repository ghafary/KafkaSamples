using MediatR;
using StackExchange.Redis;
using System.Text.Json;

namespace KafkaSample.WriteStack.Events;

public class TransactionCreated : INotification
{
    public int Id { get; set; }
    /// <summary>
    /// حساب مبدا
    /// </summary>
    public string? FromAccount { get;  set; }

    /// <summary>
    /// حساب مقصد
    /// </summary>
    public string? ToAccount { get; set; }

    /// <summary>
    /// مبلغ
    /// </summary>
    public long Amount { get; set; }

    /// <summary>
    /// تاریخ تراکنش 
    /// </summary>
    public DateTime Date { get; set; }
}

public class TransactionCreatedHandler(IConnectionMultiplexer connectionMultiplexer) : INotificationHandler<TransactionCreated>
{
    public async Task Handle(TransactionCreated notification, CancellationToken cancellationToken)
    {
        await Task.Delay(5000, cancellationToken);
        await connectionMultiplexer.GetDatabase()
            .StringSetAsync("transactions:" + notification.Id, JsonSerializer.Serialize(notification));
    }
}