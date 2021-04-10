using System;
using JetBrains.Annotations;

namespace AspireBudgetTelegramBot.Models
{
    public class TransactionAbortException : Exception
    {
        public TransactionAbortException()
        {
        }
        
        public TransactionAbortException([CanBeNull] string message) : base(message)
        {
        }
    }
}
