using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using AspireBudgetTelegramBot.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspireBudgetTelegramBot.Services
{
    public class InMemoryAuthenticateService : IAuthenticateService
    {
        private static readonly Dictionary<long, bool> AuthenticationResults = new Dictionary<long, bool>();
        private static readonly Dictionary<long, int> PendingAuthentications = new Dictionary<long, int>();
        private static readonly int MaxRetries = 4;

        private readonly ILogger<InMemoryAuthenticateService> _logger;
        private readonly TelegramOptions _options;

        public InMemoryAuthenticateService(IOptions<TelegramOptions> options, ILogger<InMemoryAuthenticateService> logger)
        {
            _logger = logger;
            _options = options.Value;
        }

        public bool AuthenticateChat(long chatId, string password)
        {
            if (IsBanned(chatId))
            {
                return false;
            }

            if (password.Trim() != _options.Password)
            {
                if (PendingAuthentications.ContainsKey(chatId))
                {
                    PendingAuthentications[chatId]++;
                    IsBanned(chatId);
                }
                else
                {
                    PendingAuthentications.Add(chatId, 1);
                }

                return false;
            }

            AuthenticationResults.Add(chatId, true);
            _logger.LogInformation($"IsAuthenticated {chatId}");
            return true;
        }

        public bool IsBanned(long chatId)
        {
            if (AuthenticationResults.ContainsKey(chatId))
            {
                return AuthenticationResults[chatId] == false;
            }

            if (PendingAuthentications.ContainsKey(chatId) && PendingAuthentications[chatId] == MaxRetries)
            {
                PendingAuthentications.Remove(chatId);
                AuthenticationResults.Add(chatId, false);

                _logger.LogInformation($"Banned {chatId}");

                return true;
            }

            return false;
        }

        public bool IsAuthenticated(long chatId)
        {
            return AuthenticationResults.ContainsKey(chatId) && AuthenticationResults[chatId];
        }
    }
}
