using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Services;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.ReloadCacheCommand
{
    /// <summary>
    /// Reload internal cache
    /// </summary>
    public class ReloadCacheCommandHandler : AsyncRequestHandler<ReloadCacheCommand>
    {
        private readonly AspireApiService _api;
        
        public ReloadCacheCommandHandler(AspireApiService api)
        {
            _api = api;
        }
        
        protected override async Task Handle(ReloadCacheCommand request, CancellationToken cancellationToken)
        {
            await _api.ReloadCacheAsync();
        }
    }
}