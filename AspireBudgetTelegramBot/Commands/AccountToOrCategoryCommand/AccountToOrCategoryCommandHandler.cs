using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Services;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.AccountToOrCategoryCommand
{
    /// <summary>
    /// Fill in account to or category
    /// </summary>
    public class AccountToOrCategoryCommandHandler : IRequestHandler<AccountToOrCategoryCommand, TelegramReplyMessage>
    {
        private readonly AspireApiService _api;
        
        public AccountToOrCategoryCommandHandler(AspireApiService api)
        {
            _api = api;
        }
        
        public async Task<TelegramReplyMessage> Handle(AccountToOrCategoryCommand request, CancellationToken cancellationToken)
        {
            if (request.Transaction.Type == Transaction.TypeTransfer)
            {
                var accounts = await _api.GetAccountsAsync();
                if (!accounts.Contains(request.Message.Text))
                {
                    throw new TransactionAbortException();
                }
                request.Transaction.AccountTo = request.Message.Text;
            }
            else
            {
                var categories = await _api.GetCategoriesAsync();
                if (!categories.Contains(request.Message.Text))
                {
                    throw new TransactionAbortException();
                }
                request.Transaction.Category = request.Message.Text;
            }

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
            }
            
            return TelegramReplyMessage.OperationCompletedMessage(request.Message);
        }
    }
}