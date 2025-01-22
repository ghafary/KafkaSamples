using KafkaSample.Domain.AggregatesModel.CustomerAggregate;
using KafkaSample.Framework.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRSDeepDive.ReadStack.Queries;

public class GetTransactionByNameQuery : IRequest<Transaction?>
{
    public required DateTime Date { get; set; }
}

public class GetProductByNameQueryHandler(ApplicationReadDbContext applicationReadDbContext) : IRequestHandler<GetTransactionByNameQuery, Transaction?>
{
    public Task<Transaction?> Handle(GetTransactionByNameQuery request, CancellationToken cancellationToken)
    {
        return applicationReadDbContext.Transactions
            .SingleOrDefaultAsync(product => product.Date==request.Date, cancellationToken: cancellationToken);
    }
}

