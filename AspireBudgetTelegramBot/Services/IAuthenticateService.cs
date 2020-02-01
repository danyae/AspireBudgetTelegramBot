using System;
using System.Collections.Generic;
using System.Text;

namespace AspireBudgetTelegramBot.Services
{
    public interface IAuthenticateService
    {
        public bool AuthenticateChat(long chatId, string password);
        public bool IsAuthenticated(long chatId);
        public bool IsBanned(long chatId);
    }
}
