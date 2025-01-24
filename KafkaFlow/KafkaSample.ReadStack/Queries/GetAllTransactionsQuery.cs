using KafkaSample.Domain.AggregatesModel.CustomerAggregate;
using KafkaSample.Framework.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KafkaSample.ReadStack.Queries;

public class GetAllTransactionsQuery : IRequest<List<Transaction>>
{
}

public class GetAllTransactionsQueryHandler(ApplicationReadDbContext applicationReadDbContext) : IRequestHandler<GetAllTransactionsQuery, List<Transaction>> 
{
    public Task<List<Transaction>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
    {
        return applicationReadDbContext.Transactions.ToListAsync(cancellationToken);
    }
}