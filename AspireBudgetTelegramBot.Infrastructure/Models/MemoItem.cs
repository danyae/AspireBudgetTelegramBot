using System;
using Ardalis.GuardClauses;

namespace AspireBudgetTelegramBot.Infrastructure.Models
{
    /// <summary>
    /// Transaction log based on memo.
    /// Basically if you usually enter *your grocery shop name* in memo
    /// and your transactions are payed with cash and go to the grocery category
    /// it can ask you if you want to fill your new transaction with same data.
    /// </summary>
    public class MemoItem
    {
        public static MemoItem EmptyMemoItem = new MemoItem("empty", "empty", "empty", "empty", "empty");
        
        public Guid Id { get; private set; }
        
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
        public MemoItem(string memo, string type, string category, string accountFrom, string accountTo)
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

        /// <summary>
        /// Update existing item
        /// </summary>
        /// <param name="memo"></param>
        /// <param name="type"></param>
        /// <param name="category"></param>
        /// <param name="accountFrom"></param>
        /// <param name="accountTo"></param>
        public void UpdateMemoItem(string memo, string type, string category, string accountFrom, string accountTo)
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

        /// <summary>
        /// was not found
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return Id == default;
        }
    }
}