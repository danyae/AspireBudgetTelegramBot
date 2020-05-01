using System;
using AspireBudgetTelegramBot.Models;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.TypeCommand
{
    public class TypeCommand : IRequest<TelegramReplyMessage>
    {
        public TelegramMessage Message { get; private set; }
        public Transaction Transaction { get; private set; }
        
        public TypeCommand(TelegramMessage message, Transaction transaction)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }
    }
}