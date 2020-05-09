namespace AspireBudgetTelegramBot.Services.Authentication
{
    public interface IAuthenticationService
    {
        public bool AuthenticateChat(long chatId, string password);
        public bool IsAuthenticated(long chatId);
        public bool IsBanned(long chatId);
    }
}
