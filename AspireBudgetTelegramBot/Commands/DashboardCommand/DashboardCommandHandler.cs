using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Services;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.DashboardCommand
{
    public class DashboardCommandHandler: IRequestHandler<DashboardCommand, TelegramReplyMessage>
    {
        private readonly AspireApiService _api;
        
        public DashboardCommandHandler(AspireApiService api)
        {
            _api = api;
        }
        
        public async Task<TelegramReplyMessage> Handle(DashboardCommand request, CancellationToken cancellationToken)
        {
            var dashboard = await _api.GetDashboardAsync();
            return TelegramReplyMessage.DashboardMessage(request.Message, dashboard);
        }
    }
}