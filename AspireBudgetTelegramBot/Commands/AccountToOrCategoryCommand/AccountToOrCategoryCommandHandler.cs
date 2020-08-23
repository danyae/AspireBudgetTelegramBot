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
    public class AccountToOrCategoryCommandHandler : AsyncRequestHandler<AccountToOrCategoryCommand>
    {
        private readonly AspireApiService _api;
        
        public AccountToOrCategoryCommandHandler(AspireApiService api)
        {
            _api = api;
        }
        
        protected override async Task Handle(AccountToOrCategoryCommand request, CancellationToken cancellationToken)
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
        }
    }
}