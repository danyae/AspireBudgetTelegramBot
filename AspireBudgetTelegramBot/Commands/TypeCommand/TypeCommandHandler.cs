using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.TypeCommand
{
    /// <summary>
    /// Fill in type
    /// </summary>
    public class TypeCommandHandler : IRequestHandler<TypeCommand, TelegramReplyMessage>
    {
        public Task<TelegramReplyMessage> Handle(TypeCommand request, CancellationToken cancellationToken)
        {
            if (request.Message.Text == Transaction.TypeTransfer || request.Message.Text == Transaction.TypeOutcome 
                                                     || request.Message.Text == Transaction.TypeIncome)
            {
                request.Transaction.Type = request.Message.Text;
                
                var reply = TelegramReplyMessage.RequestDateMessage(request.Message);
                return Task.FromResult(reply);
            }

            throw new TransactionAbortException();
        }
    }
}