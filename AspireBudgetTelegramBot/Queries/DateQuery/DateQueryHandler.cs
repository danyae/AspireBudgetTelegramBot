using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using MediatR;

namespace AspireBudgetTelegramBot.Queries.DateQuery
{
    public class DateQueryHandler : IRequestHandler<DateQuery, TelegramReplyMessage>
    {
        public Task<TelegramReplyMessage> Handle(DateQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(TelegramReplyMessage.RequestDateMessage(request.Message));
        }
    }
}