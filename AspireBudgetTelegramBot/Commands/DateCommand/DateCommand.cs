using System;
using AspireBudgetTelegramBot.Models;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.DateCommand
{
    public class DateCommand : IRequest
    {
        public TelegramMessage Message { get; private set; }
        public Transaction Transaction { get; private set; }
        
        public DateCommand(TelegramMessage message, Transaction transaction)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }
    }
}