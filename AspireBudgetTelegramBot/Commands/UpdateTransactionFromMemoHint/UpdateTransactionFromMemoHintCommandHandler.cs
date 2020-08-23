using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.UpdateTransactionFromMemoHint
{
    public class UpdateTransactionFromMemoHintCommandHandler : IRequestHandler<UpdateTransactionFromMemoHintCommand>
    {
        public Task<Unit> Handle(UpdateTransactionFromMemoHintCommand request, CancellationToken cancellationToken)
        {
            request.Transaction.Type = request.Transaction.MemoHint.Type;
            request.Transaction.AccountFrom = request.Transaction.MemoHint.AccountFrom;
            request.Transaction.AccountTo = request.Transaction.MemoHint.AccountTo;
            request.Transaction.Category = request.Transaction.MemoHint.Category;

            return Task.FromResult(Unit.Value);
        }
    }
}