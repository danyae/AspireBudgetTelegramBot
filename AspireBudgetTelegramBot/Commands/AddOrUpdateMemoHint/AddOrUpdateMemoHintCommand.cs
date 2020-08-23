using System;
using Ardalis.GuardClauses;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.AddOrUpdateMemoHint
{
    public class AddOrUpdateMemoHintCommand : IRequest
    {
        public string Memo { get; private set; }
        
        public string Type { get; private set; }
        
        public string Category { get; private set; }
        
        public string AccountFrom { get; private set; }
        
        public string AccountTo { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="memo"></param>
        /// <param name="type"></param>
        /// <param name="category"></param>
        /// <param name="accountFrom"></param>
        /// <param name="accountTo"></param>
        public AddOrUpdateMemoHintCommand(string memo, string type, string category, string accountFrom, string accountTo)
        {
            Memo = Guard.Against.NullOrWhiteSpace(memo, nameof(memo));
            Type = Guard.Against.NullOrWhiteSpace(type, nameof(type));
            AccountFrom = Guard.Against.NullOrWhiteSpace(accountFrom, nameof(accountFrom));
            if (string.IsNullOrWhiteSpace(category) && string.IsNullOrWhiteSpace(accountTo))
            {
                throw new ArgumentNullException(nameof(category));
            }
            
            Category = category;
            AccountTo = accountTo;
        }
    }
}