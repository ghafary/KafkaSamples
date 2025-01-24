using KafkaFlow.Producers;
using KafkaSample.Framework.Infrastructure;
using KafkaSample.WriteStack.Events;
using MediatR;

namespace KafkaSample.WriteStack.Commands;

public class TransactionCreateCommand : IRequest<int>
{
    public string? FromAccount { get; set; }
    public string? ToAccount { get; set; }
    public long Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TransactionCreateCommandHandler(
    ApplicationWriteDbContext applicationWriteDbContext,
    IMediator mediator, IProducerAccessor producerAccessor) : IRequestHandler<TransactionCreateCommand, int>
{
    public async Task<int> Handle(TransactionCreateCommand request, CancellationToken cancellationToken)
    {
        var producer = producerAccessor.GetProducer("NewTransactions");
        await producer.ProduceAsync(null, new TransactionCreated()
        {
            // Id = request.Id,
             Date = request.CreatedAt,
             Amount= request.Amount,
             FromAccount= request.FromAccount,
             ToAccount= request.ToAccount
        });

        return 0;
    }
}