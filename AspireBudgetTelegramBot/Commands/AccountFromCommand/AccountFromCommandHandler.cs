using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Services;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.AccountFromCommand
{
    /// <summary>
    /// Fill in account from
    /// </summary>
    public class AccountFromCommandHandler : AsyncRequestHandler<AccountFromCommand>
    {
        private readonly AspireApiService _api;
        
        public AccountFromCommandHandler(AspireApiService api)
        {
            _api = api;
        }
        
        protected override async Task Handle(AccountFromCommand request, CancellationToken cancellationToken)
        {
            var accounts = await _api.GetAccountsAsync();
            
            if (!accounts.Contains(request.Message.Text))
            {
                throw new TransactionAbortException();
            }

            request.Transaction.AccountFrom = request.Message.Text;
        }
    }
}