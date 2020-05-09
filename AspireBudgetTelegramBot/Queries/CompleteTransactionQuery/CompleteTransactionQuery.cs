using System.Security.Cryptography.X509Certificates;
using AspireBudgetTelegramBot.Models;

namespace AspireBudgetTelegramBot.Queries.CompleteTransactionQuery
{
    public class CompleteTransactionQuery : TelegramMessageQuery
    {
        public Transaction Transaction { get; private set; }
        
        public CompleteTransactionQuery(TelegramMessage msg, Transaction transaction) : base(msg)
        {
            Transaction = transaction;
        }
    }
}