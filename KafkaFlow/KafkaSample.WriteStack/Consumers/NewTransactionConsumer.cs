using KafkaFlow;
using KafkaFlow.Producers;
using KafkaSample.Domain.AggregatesModel.CustomerAggregate;
using KafkaSample.Framework.Infrastructure;
using KafkaSample.WriteStack.Events;
using MediatR;
using System.Text;
using KafkaSample.ReadStack.Queries;

namespace KafkaSample.WriteStack.Consumers;

public class NewTransactionConsumer(ApplicationWriteDbContext writeDbContext, IMediator mediator, IProducerAccessor producerAccessor) : IMessageHandler<Transaction>
{
    public async Task Handle(IMessageContext context, Transaction message)
    {
        var transaction = await mediator.Send(new GetTransactionByDataQuery()
        {
            Date = message.Date.Value,
            FromAccount = message.FromAccount,
            ToAccount = message.ToAccount
        });

        if (transaction is not null)
        {
            throw new Exception($"Transaction with name: {message.Date} already exists.");
        }

        //if (message.Quantity <= 0)
        //{
        //    throw new Exception($"Quantity is zero or negative.");
        //}

        var transactionEntity = new Transaction(message.FromAccount, message.ToAccount, message.Amount, message.Date);

        writeDbContext.Transactions.Add(transactionEntity);
        await writeDbContext.SaveChangesAsync(CancellationToken.None);

        var producer = producerAccessor.GetProducer("Transactions");
        await producer.ProduceAsync(Encoding.UTF8.GetBytes(transactionEntity.Id.ToString()), new TransactionCreated()
        {
            Id = transactionEntity.Id,
            FromAccount=transactionEntity.FromAccount,
            ToAccount = transactionEntity.ToAccount,
            Amount = transactionEntity.Amount,
            Date = transactionEntity.Date.Value
        });
    }
}