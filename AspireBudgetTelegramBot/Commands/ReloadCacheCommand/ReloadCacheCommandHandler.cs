using System;
using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Services;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.ReloadCacheCommand
{
    /// <summary>
    /// Reload internal cache
    /// </summary>
    public class ReloadCacheCommandHandler : IRequestHandler<ReloadCacheCommand, TelegramReplyMessage>
    {
        private readonly AspireApiService _api;
        
        public ReloadCacheCommandHandler(AspireApiService api)
        {
            _api = api;
        }
        
        public async Task<TelegramReplyMessage> Handle(ReloadCacheCommand request, CancellationToken cancellationToken)
        {
            await _api.ReloadCacheAsync();
            return TelegramReplyMessage.OperationCompletedMessage(request.Message);
        }
    }
}