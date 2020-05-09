using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Services;
using MediatR;

namespace AspireBudgetTelegramBot.Queries.DashboardQuery
{
    public class DashboardQueryHandler : IRequestHandler<DashboardQuery, TelegramReplyMessage>
    {
        private readonly AspireApiService _apiService;

        public DashboardQueryHandler(AspireApiService apiService)
        {
            _apiService = apiService;
        }
        
        public async Task<TelegramReplyMessage> Handle(DashboardQuery request, CancellationToken cancellationToken)
        {
            var dashboard = await _apiService.GetDashboardAsync();
            return TelegramReplyMessage.DashboardMessage(request.Message, dashboard);
        }
    }
}