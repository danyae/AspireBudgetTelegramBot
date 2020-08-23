using System;
using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Services;
using AspireBudgetTelegramBot.Services.Authentication;
using AspireBudgetTelegramBot.Services.BackgroundQueue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspireBudgetTelegramBot.Workers
{
    public class IncomingMessageWorker : BackgroundService
    {
        private readonly IBackgroundQueue<TelegramMessage> _incomingQueue;
        private readonly IBackgroundQueue<TelegramReplyMessage> _outgoingQueue;
        private readonly IAuthenticationService _authService;
        private readonly IServiceProvider _serviceProvider;

        public IncomingMessageWorker(IBackgroundQueue<TelegramMessage> incomingQueue,
            IBackgroundQueue<TelegramReplyMessage> outgoingQueue,
            IAuthenticationService authService,
            IServiceProvider serviceProvider
            )
        {
            _incomingQueue = incomingQueue;
            _outgoingQueue = outgoingQueue;
            _authService = authService;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var msg = await _incomingQueue.ReadAsync(stoppingToken);

                // if (!await AuthenticateChat(msg.ChatId, msg.Text))
                // {
                //     continue;
                // }

                await ProcessMessage(msg);
            }
        }

        private async Task<bool> AuthenticateChat(long chatId, string text)
        {
            if (_authService.IsBanned(chatId))
            {
                return false;
            }
            
            if (_authService.IsAuthenticated(chatId))
            {
                return true;
            }

            if (_authService.AuthenticateChat(chatId, text))
            {
                await _outgoingQueue.Write(new TelegramReplyMessage
                {
                    ChatId = chatId,
                    Text = "Authenticated"
                });
                
                return false; // do not process
            }
            
            await _outgoingQueue.Write(new TelegramReplyMessage
            {
                ChatId = chatId,
                Text = "Please enter your password"
            });

            return false;
        }

        private async Task ProcessMessage(TelegramMessage msg)
        {
            TelegramReplyMessage reply;
            using (var scope = _serviceProvider.CreateScope())
            {
                var transactionService = scope.ServiceProvider.GetRequiredService<TransactionService>();
                switch (msg.Text.ToLower())
                {
                    case "dashboard":
                        reply = await transactionService.GetDashboardAsync(msg);
                        break;
                    case "reload":
                        reply = await transactionService.ReloadCacheAsync(msg);
                        break;
                    default:
                        reply = await transactionService.ProcessTransactionStepAsync(msg);
                        break;
                }
            }
            await _outgoingQueue.Write(reply);
        }
    }
}
