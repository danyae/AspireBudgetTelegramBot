using System;
using System.Collections.Generic;
using System.Linq;
using AspireBudgetApi.Models;
using AspireBudgetTelegramBot.Extensions;
using Telegram.Bot.Types.ReplyMarkups;

namespace AspireBudgetTelegramBot.Models
{
    public class TelegramReplyMessage : TelegramMessage
    {
        public IReplyMarkup ReplyMarkup { get; set; }

        public static TelegramReplyMessage DashboardMessage(TelegramMessage msg, List<DashboardRow> dashboardRows)
        {
            return new TelegramReplyMessage
            {
                ChatId = msg.ChatId,
                ReplyMarkup = new ReplyKeyboardRemove(),
                Text = dashboardRows.ToHtmlSummary()
            };
        }

        public static TelegramReplyMessage UnknownOperationMessage(TelegramMessage msg)
        {
            return new TelegramReplyMessage
            {
                ChatId = msg.ChatId,
                Text = "🤷",
                ReplyMarkup = new ReplyKeyboardRemove()
            };
        }

        public static TelegramReplyMessage OperationCompletedMessage(TelegramMessage msg)
        {
            return new TelegramReplyMessage
            {
                ChatId = msg.ChatId,
                Text = "👌",
                ReplyMarkup = new ReplyKeyboardRemove()
            };
        }

        public static TelegramReplyMessage RequestTypeMessage(TelegramMessage msg)
        {
            return new TelegramReplyMessage
            {
                ChatId = msg.ChatId,
                ReplyMarkup = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton(Transaction.TypeOutcome),
                    new KeyboardButton(Transaction.TypeIncome),
                    new KeyboardButton(Transaction.TypeTransfer),
                }),
                Text = "Type of transaction?"
            };
        }

        public static TelegramReplyMessage RequestDateMessage(TelegramMessage msg)
        {
            var keyboard = new List<List<KeyboardButton>>();

            var now = DateTime.Today;
            var days = Enumerable.Range(now.AddDays(-7).Day, 9).ToArray(); // including tomorrow
            Array.Reverse(days);

            for (var i = 0; i < (float) days.Length / 3; i++)
            {
                keyboard.Add(new List<KeyboardButton>(days.Skip(i * 3).Take(3)
                    .Select(x => x == now.Day ? 
                        new KeyboardButton($"{x:00} ✅") : 
                        new KeyboardButton(x.ToString("00")))));
            }

            return new TelegramReplyMessage
            {
                ChatId = msg.ChatId,
                Text = "Day?",
                ReplyMarkup = new ReplyKeyboardMarkup(keyboard)
            };
        }

        public static TelegramReplyMessage RequestAccountFromMessage(TelegramMessage msg, List<string> accounts)
        {
            return FromItemList(msg, accounts, "Account from?");
        }

        public static TelegramReplyMessage RequestAccountToMessage(TelegramMessage msg, List<string> accounts)
        {
            return FromItemList(msg, accounts, "Account to?");
        }

        public static TelegramReplyMessage RequestCategoryMessage(TelegramMessage msg, List<string> categories)
        {
            return FromItemList(msg, categories, "Category?");
        }

        private static TelegramReplyMessage FromItemList(TelegramMessage msg, List<string> buttonsList, string text)
        {
            var keyboard = new List<List<KeyboardButton>>();

            for (var i = 0; i < (float) buttonsList.Count / 2; i++)
            {
                keyboard.Add(
                    new List<KeyboardButton>(buttonsList.Skip(i * 2).Take(2).Select(x => new KeyboardButton(x))));
            }

            return new TelegramReplyMessage
            {
                ChatId = msg.ChatId,
                Text = text,
                ReplyMarkup = new ReplyKeyboardMarkup(keyboard)
            };
        }
    }
}