using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using MediatR;

namespace AspireBudgetTelegramBot.Queries.TypeQuery
{
    public class TypeQueryHandler : IRequestHandler<TypeQuery, TelegramReplyMessage>
    {
        public Task<TelegramReplyMessage> Handle(TypeQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(TelegramReplyMessage.RequestTypeMessage(request.Message));
        }
    }
}