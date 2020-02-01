using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.Enums;

namespace AspireBudgetTelegramBot.Models
{
    public class TelegramMessage
    {
        public long ChatId { get; set; }
        public string Text { get; set; }
    }
}
