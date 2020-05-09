using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using MediatR;

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