using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Infrastructure.Database;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Services;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.SaveTransaction
{
    public class SaveTransactionCommandHandler : IRequestHandler<SaveTransactionCommand>
    {
        private readonly AspireApiService _api;
        
        public SaveTransactionCommandHandler(AspireApiService api)
        {
            _api = api;
        }
        
        public async Task<Unit> Handle(SaveTransactionCommand request, CancellationToken cancellationToken)
        {
            switch (request.Transaction.Type)
            {
                case Transaction.TypeOutcome:
                    await _api.SaveTransactionAsync(new AspireBudgetApi.Models.Transaction
                    {
                        Account = request.Transaction.AccountFrom,
                        Category = request.Transaction.Category,
                        Cleared = "🆗",
                        Date = request.Transaction.Date.Value,
                        Outflow = request.Transaction.Sum.Value,
                        Memo = request.Transaction.Memo
                    });
                    break;
                case Transaction.TypeIncome:
                    await _api.SaveTransactionAsync(new AspireBudgetApi.Models.Transaction
                    {
                        Account = request.Transaction.AccountFrom,
                        Category = request.Transaction.Category,
                        Cleared = "🆗",
                        Date = request.Transaction.Date.Value,
                        Inflow = request.Transaction.Sum.Value,
                        Memo = request.Transaction.Memo
                    });
                    break;
                case Transaction.TypeTransfer:
                    await _api.SaveTransactionAsync(new AspireBudgetApi.Models.Transaction
                    {
                        Account = request.Transaction.AccountFrom,
                        Category = "↕️ Account Transfer",
                        Cleared = "🆗",
                        Date = request.Transaction.Date.Value,
                        Outflow = request.Transaction.Sum.Value,
                        Memo = request.Transaction.Memo
                    });
                    await _api.SaveTransactionAsync(new AspireBudgetApi.Models.Transaction
                    {
                        Account = request.Transaction.AccountTo,
                        Category = "↕️ Account Transfer",
                        Cleared = "🆗",
                        Date = request.Transaction.Date.Value,
                        Inflow = request.Transaction.Sum.Value,
                        Memo = request.Transaction.Memo
                    });
                    break;
                default:
                    throw new TransactionAbortException();
            }

            return Unit.Value;
        }
    }
}