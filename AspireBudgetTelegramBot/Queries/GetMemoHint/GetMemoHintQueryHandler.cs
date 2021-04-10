using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Infrastructure.Database;
using AspireBudgetTelegramBot.Infrastructure.Models;
using AspireBudgetTelegramBot.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AspireBudgetTelegramBot.Queries.GetMemoHint
{
    public class GetMemoHintQueryHandler : IRequestHandler<GetMemoHintQuery, MemoItem>
    {
        private readonly AspireBudgetDbContext _dbContext;
        private readonly AspireApiService _api;

        public GetMemoHintQueryHandler(AspireBudgetDbContext dbContext, AspireApiService api)
        {
            _dbContext = dbContext;
            _api = api;
        }

        public async Task<MemoItem> Handle(GetMemoHintQuery request, CancellationToken cancellationToken)
        {
            var memoItem = await _dbContext.MemoItems.FirstOrDefaultAsync(x => x.Memo == request.Memo, cancellationToken);
            if (memoItem == null)
            {
                return MemoItem.EmptyMemoItem;
            }
            
            var accounts = await _api.GetAccountsAsync();
            var categories = await _api.GetCategoriesAsync();

            if (accounts.All(x => x != memoItem.AccountFrom) || 
                categories.All(x => x != memoItem.Category) && accounts.All(x => x != memoItem.AccountTo))
            {
                _dbContext.MemoItems.Remove(memoItem);
                await _dbContext.SaveChangesAsync(cancellationToken);
                
                return MemoItem.EmptyMemoItem; 
            }

            return memoItem;
        }
    }
}