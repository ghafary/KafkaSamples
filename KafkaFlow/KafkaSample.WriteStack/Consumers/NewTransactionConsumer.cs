using System.Text;
using CQRSDeepDive.ReadStack.Queries;
using KafkaFlow;
using KafkaFlow.Producers;
using KafkaSample.Domain.AggregatesModel.CustomerAggregate;
using KafkaSample.Framework.Infrastructure;
using KafkaSample.WriteStack.Events;
using MediatR;

namespace KafkaSample.WriteStack.Consumers;

public class NewTransactionConsumer(ApplicationWriteDbContext writeDbContext, IMediator mediator, IProducerAccessor producerAccessor) : IMessageHandler<Transaction>
{
    public async Task Handle(IMessageContext context,Transaction message)
    {
        var product = await mediator.Send(new GetTransactionByNameQuery()
        {
            Date = message.Date.Value
        });

        if (product is not null)
        {
            throw new Exception($"Product with name: {message.Date} already exists.");
        }

        //if (message.Quantity <= 0)
        //{
        //    throw new Exception($"Quantity is zero or negative.");
        //}

        var transactionEntity = new Transaction(message.FromAccount, message.ToAccount,message.Amount,message.Date);
        
        writeDbContext.Transactions.Add(transactionEntity);
        await writeDbContext.SaveChangesAsync(CancellationToken.None);
        
        var producer = producerAccessor.GetProducer("Products");
        await producer.ProduceAsync(Encoding.UTF8.GetBytes(transactionEntity.Id.ToString()), new TransactionCreated()
        {
            
        });
    }
}