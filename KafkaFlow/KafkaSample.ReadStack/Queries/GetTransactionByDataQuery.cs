using KafkaSample.Domain.AggregatesModel.CustomerAggregate;
using KafkaSample.Framework.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KafkaSample.ReadStack.Queries;

public class GetTransactionByDataQuery  : IRequest<Transaction?>
{
    public required DateTime Date { get; set; }

    public string? FromAccount { get;  set; }

    /// <summary>
    /// حساب مقصد
    /// </summary>
    public string? ToAccount { get;  set; }
}

public class GetTransactionByDataQueryHandler(ApplicationReadDbContext applicationReadDbContext) : IRequestHandler<GetTransactionByDataQuery, Transaction?>
{
    public Task<Transaction?> Handle(GetTransactionByDataQuery request, CancellationToken cancellationToken)
    {
        return applicationReadDbContext.Transactions
            .SingleOrDefaultAsync(transaction => transaction.Date==request.Date, cancellationToken: cancellationToken);
    }
}

