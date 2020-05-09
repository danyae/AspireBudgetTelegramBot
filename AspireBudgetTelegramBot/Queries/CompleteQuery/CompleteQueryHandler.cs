using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Extensions;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Services;
using MediatR;
using Telegram.Bot.Types.ReplyMarkups;

namespace AspireBudgetTelegramBot.Queries.CompleteQuery
{
    public class CompleteQueryHandler : IRequestHandler<CompleteQuery, TelegramReplyMessage>
    {
        public Task<TelegramReplyMessage> Handle(CompleteQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(TelegramReplyMessage.OperationCompletedMessage(request.Message));
        }
    }
}