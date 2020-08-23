using Ardalis.GuardClauses;
using AspireBudgetTelegramBot.Infrastructure.Models;
using AspireBudgetTelegramBot.Models;

namespace AspireBudgetTelegramBot.Queries.RequestMemoHint
{
    public class RequestMemoHintQuery : TelegramMessageQuery
    {
        public MemoItem MemoItem { get; private set; }
        
        public RequestMemoHintQuery(TelegramMessage msg, MemoItem memoItem) : base(msg)
        {
            MemoItem = Guard.Against.Null(memoItem, nameof(memoItem));
        }
    }
}