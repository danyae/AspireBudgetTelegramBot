using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Services;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.ReloadCacheCommand
{
    /// <summary>
    /// Reload internal cache
    /// </summary>
    public class ReloadCacheCommandHandler : IRequestHandler<ReloadCacheCommand>
    {
        private readonly AspireApiService _api;
        
        public ReloadCacheCommandHandler(AspireApiService api)
        {
            _api = api;
        }

        async Task<Unit> IRequestHandler<ReloadCacheCommand, Unit>.Handle(ReloadCacheCommand request, CancellationToken cancellationToken)
        {
            await _api.ReloadCacheAsync();
            return Unit.Value;
        }
    }
}