using KafkaSample.Domain.SeedWork;

namespace KafkaSample.Domain.AggregatesModel.CustomerAggregate
{
    /// <summary>
    /// تراکنش
    /// </summary>
    public class Transaction : Entity<int>, IAggregateRoot
    {
        private Transaction() { }
        public Transaction(string? fromAccount, string? toAccount, long amount, DateTime? date)
        {
            FromAccount = fromAccount;
            ToAccount = toAccount;
            Amount = amount;
            Date = date;
        }

        /// <summary>
        /// حساب مبدا
        /// </summary>
        public string? FromAccount { get; private set; }

        /// <summary>
        /// حساب مقصد
        /// </summary>
        public string? ToAccount { get; private set; }

        /// <summary>
        /// مبلغ
        /// </summary>
        public long Amount{ get; private set; }

        /// <summary>
        /// تاریخ تراکنش 
        /// </summary>
        public DateTime? Date{ get; private set; }
    }
}
