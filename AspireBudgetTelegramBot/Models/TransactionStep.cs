using System;
using System.Collections.Generic;
using System.Text;

namespace AspireBudgetTelegramBot.Models
{
    public enum TransactionStep
    {
        Sum,
        Type,
        Date,
        AccountFrom,
        AccountToOrCategory,
        FinalStep
    }
}
