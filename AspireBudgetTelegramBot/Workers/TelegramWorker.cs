using System;
using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Services;
using AspireBudgetTelegramBot.Services.BackgroundQueue;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace AspireBudgetTelegramBot.Workers
{
    public class TelegramWorker : BackgroundService
    {
        private readonly ILogger<TelegramWorker> _logger;
        private readonly TelegramBotService _bot;
        private readonly IBackgroundQueue<TelegramMessage> _incomingQueue;
        private readonly IBackgroundQueue<TelegramReplyMessage> _outgoingQueue;

        public TelegramWorker(TelegramBotService bot,
            IBackgroundQueue<TelegramMessage> incomingQueue,
            IBackgroundQueue<TelegramReplyMessage> outgoingQueue,
            ILogger<TelegramWorker> logger)
        {
            _bot = bot;
            _bot.OnMessage += BotOnMessage;
            _bot.OnMessageEdited += BotOnMessage;

            _incomingQueue = incomingQueue;
            _outgoingQueue = outgoingQueue;
            _logger = logger;
        }

        private async void BotOnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Type != MessageType.Text) { return; }

            var model = new TelegramMessage
            {
                ChatId = e.Message.Chat.Id,
                Text = e.Message.Text
            };

            await _incomingQueue.Write(model);

            _logger.LogInformation(model.Text);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _bot.StartBot(stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await _outgoingQueue.ReadAsync(stoppingToken);

                try
                {
                    await _bot.SendMessage(message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error sending telegram msg");
                }
            }
            _bot.StopBot();
        }
    }
}
