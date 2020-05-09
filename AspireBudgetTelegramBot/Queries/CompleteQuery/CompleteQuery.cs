using System.Security.Cryptography.X509Certificates;
using AspireBudgetTelegramBot.Models;

namespace AspireBudgetTelegramBot.Queries.CompleteQuery
{
    public class CompleteQuery : TelegramMessageQuery
    {
        public CompleteQuery(TelegramMessage msg) : base(msg)
        {
        }
    }
}