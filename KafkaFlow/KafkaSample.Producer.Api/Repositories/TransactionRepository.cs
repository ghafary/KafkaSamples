
using FluentResults;
using KafkaSample.Domain.AggregatesModel.CustomerAggregate;
using KafkaSample.Framework.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace KafkaSample.Repositories;

public class TransactionRepository(ApplicationWriteDbContext writeDbContext)
{
    public async Task<IResult> Create(Transaction request)
    {
        if (request.Amount <= 0)
        {
            return Results.BadRequest("Quantity Cannot be less or equal to 0");
        }

        writeDbContext.Transactions.Add(request);
        await writeDbContext.SaveChangesAsync();

        return Results.Ok();
    }
}