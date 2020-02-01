using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspireBudgetTelegramBot.Workers
{
    public class IncomingMessageWorker : BackgroundService
    {
        private readonly ILogger<IncomingMessageWorker> _logger;
        private readonly IBackgroundQueue<TelegramMessage> _incomingQueue;
        private readonly IBackgroundQueue<TelegramReplyMessage> _outgoingQueue;
        private readonly AspireApiService _api;
        private readonly IAuthenticateService _authService;

        public IncomingMessageWorker(IBackgroundQueue<TelegramMessage> incomingQueue,
            IBackgroundQueue<TelegramReplyMessage> outgoingQueue,
            AspireApiService api,
            IAuthenticateService authService,
            ILogger<IncomingMessageWorker> logger)
        {
            _incomingQueue = incomingQueue;
            _outgoingQueue = outgoingQueue;
            _api = api;
            _authService = authService;
            _logger = logger;
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
                if (msg.Text == "dashboard")
                {
                    reply = await _api.GetDashboard(msg);
                }
                else
                {
                    reply = await _api.ProcessTransactionStep(msg);
                }
                await _outgoingQueue.Write(reply);
            }
        }
    }
}
