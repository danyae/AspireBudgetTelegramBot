using System;

namespace AspireBudgetTelegramBot.Models
{
    public class Transaction
    {
        public const string TypeIncome = "💰 Income";
        public const string TypeOutcome = "💸 Outcome";
        public const string TypeTransfer = "🔄 Transfer";

        public double? Sum { get; set; }
        public string Type { get; set; }
        public DateTime? Date { get; set; }
        public string AccountFrom { get;set; }
        public string AccountTo { get;set; }
        public string Category { get; set; }
        public string Memo { get; set; }

        public TransactionStep GetCurrentStep()
        {
            if (Sum == null)
            {
                return TransactionStep.Sum;
            }

            if (Type == null)
            {
                return TransactionStep.Type;
            }

            if (Date == null)
            {
                return TransactionStep.Date;
            }

            if (AccountFrom == null)
            {
                return TransactionStep.AccountFrom;
            }

            if (AccountTo == null && Category == null)
            {
                return TransactionStep.AccountToOrCategory;
            }

            return TransactionStep.FinalStep;
        }
    }
}
