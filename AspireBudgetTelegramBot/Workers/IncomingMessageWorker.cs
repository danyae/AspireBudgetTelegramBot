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

                if (!_authService.IsAuthenticated(msg.ChatId))
                {
                    if (_authService.IsBanned(msg.ChatId))
                    {
                        continue;
                    }

                    if (_authService.AuthenticateChat(msg.ChatId, msg.Text))
                    {
                        await _outgoingQueue.Write(new TelegramReplyMessage
                        {
                            ChatId = msg.ChatId,
                            Text = "Authenticated"
                        });
                    }
                    else
                    {
                        await _outgoingQueue.Write(new TelegramReplyMessage
                        {
                            ChatId = msg.ChatId,
                            Text = "Please enter your password"
                        });
                    }

                    continue;
                }

                TelegramReplyMessage reply;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var transactionService = scope.ServiceProvider.GetRequiredService<TransactionService>();
                    switch (msg.Text)
                    {
                        case "dashboard":
                            reply = await transactionService.GetDashboardAsync(msg);
                            break;
                        case "reload":
                            await transactionService.ReloadCacheAsync(msg);
                            reply = TelegramReplyMessage.OperationCompletedMessage(msg);
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
}
