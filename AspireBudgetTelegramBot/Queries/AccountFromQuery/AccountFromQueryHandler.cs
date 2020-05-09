using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Services;
using MediatR;

namespace AspireBudgetTelegramBot.Queries.AccountFromQuery
{
    public class AccountFromQueryHandler : IRequestHandler<AccountFromQuery, TelegramReplyMessage>
    {
        private readonly AspireApiService _apiService;

        public AccountFromQueryHandler(AspireApiService apiService)
        {
            _apiService = apiService;
        }
        
        public async Task<TelegramReplyMessage> Handle(AccountFromQuery request, CancellationToken cancellationToken)
        {
            var accounts = await _apiService.GetAccountsAsync();
            var reply = TelegramReplyMessage.RequestAccountFromMessage(request.Message, accounts);
            return reply;
        }
    }
}