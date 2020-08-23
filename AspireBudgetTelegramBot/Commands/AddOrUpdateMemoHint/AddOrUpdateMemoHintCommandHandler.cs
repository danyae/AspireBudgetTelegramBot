using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Infrastructure.Database;
using AspireBudgetTelegramBot.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AspireBudgetTelegramBot.Commands.AddOrUpdateMemoHint
{
    public class AddOrUpdateMemoHintCommandHandler : IRequestHandler<AddOrUpdateMemoHintCommand>
    {
        private readonly AspireBudgetDbContext _dbContext;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="dbContext"></param>
        public AddOrUpdateMemoHintCommandHandler(AspireBudgetDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<Unit> Handle(AddOrUpdateMemoHintCommand request, CancellationToken cancellationToken)
        {
            var memoItem = await _dbContext.MemoItems.FirstOrDefaultAsync(x => x.Memo == request.Memo, cancellationToken);

            if (memoItem == null)
            {
                memoItem = new MemoItem(request.Memo, request.Type, request.Category, request.AccountFrom, request.AccountTo);
                await _dbContext.MemoItems.AddAsync(memoItem, cancellationToken);
            }
            else
            {
                memoItem.UpdateMemoItem(request.Memo, request.Type, request.Category, request.AccountFrom, request.AccountTo);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}