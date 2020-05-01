using System;
using AspireBudgetTelegramBot.Models;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.SumCommand
{
    public class SumCommand : IRequest<TelegramReplyMessage>
    {
        public TelegramMessage Message { get; private set; }
        public Transaction Transaction { get; private set; }
        
        public SumCommand(TelegramMessage message, Transaction transaction)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }
    }
}