using KafkaSample.Domain.AggregatesModel.CustomerAggregate;
using MediatR;

namespace KafkaSample.Domain.Events.Customers
{
    public class TransactionCreatedDomainEvent : INotification
    {
        public Transaction Customer { get; }


        public TransactionCreatedDomainEvent(Transaction customer)
        {
            Customer = customer;
        }
    }
}
