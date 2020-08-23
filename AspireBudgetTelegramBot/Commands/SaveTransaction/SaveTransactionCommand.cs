using System;
using AspireBudgetTelegramBot.Models;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.SaveTransaction
{
    public class SaveTransactionCommand : IRequest
    {
        public Transaction Transaction { get; private set; }
        
        public SaveTransactionCommand(Transaction transaction)
        {
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }
    }
}