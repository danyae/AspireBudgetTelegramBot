using Ardalis.GuardClauses;
using AspireBudgetTelegramBot.Infrastructure.Models;
using MediatR;

namespace AspireBudgetTelegramBot.Queries.GetMemoHint
{
    public class GetMemoHintQuery : IRequest<MemoItem>
    {
        public string Memo { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="memo"></param>
        public GetMemoHintQuery(string memo)
        {
            Memo = Guard.Against.NullOrWhiteSpace(memo, nameof(memo));
        }
    }
}