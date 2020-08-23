using System;
using Ardalis.GuardClauses;
using AspireBudgetTelegramBot.Infrastructure.Models;
using AspireBudgetTelegramBot.Models;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.UpdateTransactionFromMemoHint
{
    public class UpdateTransactionFromMemoHintCommand : IRequest
    {
        public Transaction Transaction { get; private set; }
        
        public UpdateTransactionFromMemoHintCommand(Transaction transaction)
        {
            Transaction = Guard.Against.Null(transaction, nameof(transaction));
        }
    }
}