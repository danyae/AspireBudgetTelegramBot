using System;
using AspireBudgetTelegramBot.Models;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.ReloadCacheCommand
{
    public class ReloadCacheCommand : IRequest<TelegramReplyMessage>
    {
        public TelegramMessage Message { get; private set; }
        
        public ReloadCacheCommand(TelegramMessage message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}