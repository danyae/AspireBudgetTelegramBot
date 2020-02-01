using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MihaZupan;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace AspireBudgetTelegramBot.Services
{
    public class TelegramBotService
    {
        private ITelegramBotClient BotClient { get; set; }

        private readonly TelegramOptions _options;

        public event EventHandler<MessageEventArgs> OnMessage
        {
            add => BotClient.OnMessage += value;
            remove => BotClient.OnMessage -= value;
        }
        public event EventHandler<MessageEventArgs> OnMessageEdited
        {
            add => BotClient.OnMessageEdited += value;
            remove => BotClient.OnMessageEdited -= value;
        }

        public event EventHandler<CallbackQueryEventArgs> BotOnCallbackQuery
        {
            add => BotClient.OnCallbackQuery += value;
            remove => BotClient.OnCallbackQuery -= value;
        }

        public TelegramBotService(IOptions<TelegramOptions> options,
            ILogger<TelegramBotService> logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
            InitBot();
        }

        private void InitBot()
        {
            if(string.IsNullOrWhiteSpace(_options.ApiToken))
            {
                throw new NullReferenceException(nameof(_options.ApiToken));
            }

            if (_options.UseTorProxy)
            {
                HttpToSocks5Proxy proxy = new HttpToSocks5Proxy("127.0.0.1", 9050);
                BotClient = new TelegramBotClient(_options.ApiToken, proxy);
            }
            else
            {
                BotClient = new TelegramBotClient(_options.ApiToken);
            }
        }

        public void StartBot(CancellationToken token = default)
        {
            BotClient.StartReceiving(cancellationToken: token);
        }

        public void StopBot()
        {
            BotClient.StopReceiving();
        }

        public async Task SendMessage(TelegramReplyMessage msg)
        {
            await BotClient.SendTextMessageAsync(msg.ChatId, msg.Text, ParseMode.Html, replyMarkup: msg.ReplyMarkup);
        }
    }
}
