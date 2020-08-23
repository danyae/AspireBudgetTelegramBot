using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using MediatR;

namespace AspireBudgetTelegramBot.Queries.RequestMemoHint
{
    public class RequestMemoHintQueryHandler : IRequestHandler<RequestMemoHintQuery, TelegramReplyMessage>
    {
        public Task<TelegramReplyMessage> Handle(RequestMemoHintQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(TelegramReplyMessage.RequestMemoHintMessage(request.Message, request.MemoItem));
        }
    }
}