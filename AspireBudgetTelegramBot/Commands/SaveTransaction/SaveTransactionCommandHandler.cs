using System.Threading;
using System.Threading.Tasks;
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
                    await _api.SaveTransactionAsync(new AspireBudgetApi.Models.Transaction(
                        request.Transaction.Date!.Value,
                        request.Transaction.Sum!.Value,
                        0,
                        request.Transaction.Category,
                        request.Transaction.AccountFrom,
                        request.Transaction.Memo,
                        AspireBudgetApi.Options.ClearedSymbolSettled
                    ));
                    break;
                case Transaction.TypeIncome:
                    await _api.SaveTransactionAsync(new AspireBudgetApi.Models.Transaction
                    (
                        request.Transaction.Date!.Value,
                        0,
                        request.Transaction.Sum!.Value,
                        request.Transaction.Category,
                        request.Transaction.AccountFrom,
                        request.Transaction.Memo,
                        AspireBudgetApi.Options.ClearedSymbolSettled
                    ));
                    break;
                case Transaction.TypeTransfer:
                    await _api.SaveTransactionAsync(new AspireBudgetApi.Models.Transaction
                    (
                        request.Transaction.Date!.Value,
                        request.Transaction.Sum!.Value,
                        0,
                        AspireBudgetApi.Options.AccountTransferCategory, 
                        request.Transaction.AccountFrom,
                        request.Transaction.Memo,
                        AspireBudgetApi.Options.ClearedSymbolSettled
                    ));
                    await _api.SaveTransactionAsync(new AspireBudgetApi.Models.Transaction
                    (
                        request.Transaction.Date!.Value,
                        0,
                        request.Transaction.Sum!.Value,
                        AspireBudgetApi.Options.AccountTransferCategory,
                        request.Transaction.AccountTo,
                        request.Transaction.Memo,
                        AspireBudgetApi.Options.ClearedSymbolSettled
                    ));
                    break;
                default:
                    throw new TransactionAbortException();
            }

            return Unit.Value;
        }
    }
}