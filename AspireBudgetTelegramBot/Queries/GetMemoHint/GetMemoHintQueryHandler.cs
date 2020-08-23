using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Infrastructure.Database;
using AspireBudgetTelegramBot.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AspireBudgetTelegramBot.Queries.GetMemoHint
{
    public class GetMemoHintQueryHandler : IRequestHandler<GetMemoHintQuery, MemoItem>
    {
        private readonly AspireBudgetDbContext _dbContext;

        public GetMemoHintQueryHandler(AspireBudgetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MemoItem> Handle(GetMemoHintQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.MemoItems.FirstOrDefaultAsync(x => x.Memo == request.Memo, cancellationToken) ??
                   MemoItem.EmptyMemoItem;
        }
    }
}