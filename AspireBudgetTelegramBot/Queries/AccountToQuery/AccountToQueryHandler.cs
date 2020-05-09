using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Services;
using MediatR;

namespace AspireBudgetTelegramBot.Queries.AccountToQuery
{
    public class AccountToQueryHandler : IRequestHandler<AccountToQuery, TelegramReplyMessage>
    {
        private readonly AspireApiService _apiService;

        public AccountToQueryHandler(AspireApiService apiService)
        {
            _apiService = apiService;
        }
        
        public async Task<TelegramReplyMessage> Handle(AccountToQuery request, CancellationToken cancellationToken)
        {
            var accounts = await _apiService.GetAccountsAsync();
            var reply = TelegramReplyMessage.RequestAccountToMessage(request.Message, accounts);
            return reply;
        }
    }
}