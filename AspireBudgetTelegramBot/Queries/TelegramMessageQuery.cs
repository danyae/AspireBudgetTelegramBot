using AspireBudgetTelegramBot.Models;
using MediatR;

namespace AspireBudgetTelegramBot.Queries
{
    public abstract class TelegramMessageQuery : IRequest<TelegramReplyMessage>
    {
        public TelegramMessage Message { get; set; }

        protected TelegramMessageQuery(TelegramMessage msg)
        {
            Message = msg;
        }
    }
}