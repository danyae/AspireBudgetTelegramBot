using System;
using AspireBudgetTelegramBot.Models;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.DashboardCommand
{
    public class DashboardCommand : IRequest<TelegramReplyMessage>
    {
        public TelegramMessage Message { get; private set; }

        public DashboardCommand(TelegramMessage message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}