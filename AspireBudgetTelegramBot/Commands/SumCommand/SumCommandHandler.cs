using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.SumCommand
{
    /// <summary>
    /// Fill in sum and memo
    /// </summary>
    public class SumCommandHandler : AsyncRequestHandler<SumCommand>
    {
        protected override Task Handle(SumCommand request, CancellationToken cancellationToken)
        {
            var msgParts = request.Message.Text.Split(' ', 2);
            if (msgParts.Length == 0 || !double.TryParse(msgParts[0], out var sum))
            {
                throw new TransactionAbortException();
            }

            request.Transaction.Sum = sum;
            request.Transaction.Memo = msgParts.Length > 1 ? msgParts[1] : null;
            
            return Task.CompletedTask;
        }
    }
}