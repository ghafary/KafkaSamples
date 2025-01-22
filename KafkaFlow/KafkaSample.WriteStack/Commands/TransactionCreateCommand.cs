using System.Text;
using System.Text.Json;
using System.Text.Unicode;

using KafkaFlow.Producers;
using KafkaSample.Framework.Infrastructure;
using KafkaSample.WriteStack.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace CQRSDeepDive.WriteStack.Commands;

public class TransactionCreateCommand : IRequest<int>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Quantity { get; set; }
}

public class ProductCreateCommandHandler(
    ApplicationWriteDbContext applicationWriteDbContext,
    IMediator mediator, IProducerAccessor producerAccessor) : IRequestHandler<TransactionCreateCommand, int>
{
    public async Task<int> Handle(TransactionCreateCommand request, CancellationToken cancellationToken)
    {
        var producer = producerAccessor.GetProducer("NewTransactions");
        await producer.ProduceAsync(null, new TransactionCreated()
        {
            // Id = request.Id,
            //Category = request.Category,
            //Price = request.Price,
            //Quantity = request.Quantity,
            //CreatedAt = request.CreatedAt,
            //Name = request.Name,
            //Description = request.Description,
        });

        return 0;
    }
}