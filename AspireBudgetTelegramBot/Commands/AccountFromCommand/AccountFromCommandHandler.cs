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
    public class AccountFromCommandHandler : IRequestHandler<AccountFromCommand, TelegramReplyMessage>
    {
        private readonly AspireApiService _api;
        
        public AccountFromCommandHandler(AspireApiService api)
        {
            _api = api;
        }
        
        public async Task<TelegramReplyMessage> Handle(AccountFromCommand request, CancellationToken cancellationToken)
        {
            var accounts = await _api.GetAccountsAsync();
            
            if (!accounts.Contains(request.Message.Text))
            {
                throw new TransactionAbortException();
            }

            request.Transaction.AccountFrom = request.Message.Text;
            if (request.Transaction.Type == Transaction.TypeTransfer)
            {
                return TelegramReplyMessage.RequestAccountToMessage(request.Message, accounts);
            }

            var categories = await _api.GetCategoriesAsync();
            return TelegramReplyMessage.RequestCategoryMessage(request.Message, categories);
        }
    }
}