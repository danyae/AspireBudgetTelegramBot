using System;
using AspireBudgetTelegramBot.Models;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.AccountFromCommand
{
    public class AccountFromCommand : IRequest
    {
        public TelegramMessage Message { get; private set; }
        public Transaction Transaction { get; private set; }
        
        public AccountFromCommand(TelegramMessage message, Transaction transaction)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }
    }
}