using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Extensions;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types.ReplyMarkups;
using Transaction = AspireBudgetTelegramBot.Models.Transaction;

namespace AspireBudgetTelegramBot.Services
{
    public class AspireApiService : IDisposable
    {
        public static Transaction CurrentTransaction { get; set; }

        private static List<string> Accounts { get; set; }
        private static List<string> Categories { get; set; }

        private readonly ILogger<AspireApiService> _logger;
        private readonly AspireBudgetApi.AspireBudgetApi _api;

        public AspireApiService(ILogger<AspireApiService> logger,
            IOptions<AspireOptions> options)
        {
            string json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, options.Value.ApiCredentialsFile), Encoding.UTF8);
            _api = new AspireBudgetApi.AspireBudgetApi(json, options.Value.SheetId, logger);
            _logger = logger;
        }

        public async Task<TelegramReplyMessage> GetDashboard(TelegramMessage msg)
        {
            var dashboard = await _api.GetDashboardAsync();
            return new TelegramReplyMessage
            {
                ChatId = msg.ChatId,
                ReplyMarkup = new ReplyKeyboardRemove(),
                Text = dashboard.ToSummary()
            };
        }
        
        public async Task<TelegramReplyMessage> ProcessTransactionStep(TelegramMessage msg)
        {
            await TryInitNewTransaction();

            try
            {
                switch (CurrentTransaction.GetCurrentStep())
                {
                    case TransactionStep.Sum:
                        ProcessSum(msg);
                        return TelegramReplyMessage.RequestTypeMessage(msg);
                    case TransactionStep.Type:
                        ProcessType(msg);
                        return TelegramReplyMessage.RequestDateMessage(msg);
                    case TransactionStep.Date:
                        ProcessDate(msg);
                        var accounts = await _api.GetAccountsAsync();
                        return TelegramReplyMessage.RequestAccountFromMessage(msg, accounts); 
                    case TransactionStep.AccountFrom:
                        ProcessAccountFrom(msg);
                        if (CurrentTransaction.Type == Transaction.TypeTransfer)
                        {
                            return TelegramReplyMessage.RequestAccountToMessage(msg, Accounts);
                        }
                        else
                        {
                            return TelegramReplyMessage.RequestCategoryMessage(msg, Categories);
                        } 
                    case TransactionStep.AccountToOrCategory:
                        await ProcessAccountToOrCategory(msg);
                        RemoveTransaction();
                        return TelegramReplyMessage.OperationCompletedMessage(msg);
                    default:
                        throw new TransactionAbortException();
                }
            }
            catch (Exception ex)
            {
                if (!(ex is TransactionAbortException))
                {
                    _logger.LogError(ex, ex.Message);
                }
                RemoveTransaction();
                return TelegramReplyMessage.UnknownOperationMessage(msg);
            }
        }

        private async Task TryInitNewTransaction()
        {
            if (CurrentTransaction == null)
            {
                CurrentTransaction = new Transaction();
            }

            if (Categories == null)
            {
                Categories = await _api.GetCategoriesAsync();
            }

            if (Accounts == null)
            {
                Accounts = await _api.GetAccountsAsync();
            }
        }

        private void RemoveTransaction()
        {
            CurrentTransaction = null;
            Categories = null;
            Accounts = null;
        }

        public void ProcessSum(TelegramMessage msg)
        {
            if (double.TryParse(msg.Text, out var sum))
            {
                CurrentTransaction.Sum = sum;
                return;
            }

            throw new TransactionAbortException();
        }

        public void ProcessType(TelegramMessage msg)
        {
            if (msg.Text == Transaction.TypeTransfer || msg.Text == Transaction.TypeOutcome 
                                                     || msg.Text == Transaction.TypeIncome)
            {
                CurrentTransaction.Type = msg.Text;
                return;
            }

            throw new TransactionAbortException();
        }

        public void ProcessDate(TelegramMessage msg)
        {
            if (byte.TryParse(msg.Text, out var day) && day > 0 && day < 32)
            {
                var transactionDate = DateTime.Now;
                if (transactionDate.Day > day)
                {
                    transactionDate = new DateTime(transactionDate.Year, transactionDate.Month, day);
                }

                if (transactionDate.Day < day)
                {
                    transactionDate = new DateTime(transactionDate.Year, transactionDate.Month-1, day);
                }

                CurrentTransaction.Date = transactionDate;
                return;
            }

            throw new TransactionAbortException();
        }

        public void ProcessAccountFrom(TelegramMessage msg)
        {
            if (!Accounts.Contains(msg.Text))
            {
                throw new TransactionAbortException();
            }

            CurrentTransaction.AccountFrom = msg.Text;
        }

        public async Task ProcessAccountToOrCategory(TelegramMessage msg)
        {
            if (CurrentTransaction.Type == Transaction.TypeTransfer)
            {
                var accounts = await _api.GetAccountsAsync();
                if (!accounts.Contains(msg.Text))
                {
                    throw new TransactionAbortException();
                }
                CurrentTransaction.AccountTo = msg.Text;
            }
            else
            {
                var categories = await _api.GetCategoriesAsync();
                if (!categories.Contains(msg.Text))
                {
                    throw new TransactionAbortException();
                }
                CurrentTransaction.Category = msg.Text;
            }


            switch (CurrentTransaction.Type)
            {
                case Transaction.TypeOutcome:
                    await _api.SaveTransactionAsync(new AspireBudgetApi.Models.Transaction
                    {
                        Account = CurrentTransaction.AccountFrom,
                        Category = CurrentTransaction.Category,
                        Cleared = "🆗",
                        Date = CurrentTransaction.Date.Value,
                        Outflow = CurrentTransaction.Sum.Value
                    });
                    break;
                case Transaction.TypeIncome:
                    await _api.SaveTransactionAsync(new AspireBudgetApi.Models.Transaction
                    {
                        Account = CurrentTransaction.AccountFrom,
                        Category = CurrentTransaction.Category,
                        Cleared = "🆗",
                        Date = CurrentTransaction.Date.Value,
                        Inflow = CurrentTransaction.Sum.Value
                    });
                    break;
                case Transaction.TypeTransfer:
                    await _api.SaveTransactionAsync(new AspireBudgetApi.Models.Transaction
                    {
                        Account = CurrentTransaction.AccountFrom,
                        Category = "↕️ Account Transfer",
                        Cleared = "🆗",
                        Date = CurrentTransaction.Date.Value,
                        Outflow = CurrentTransaction.Sum.Value
                    });
                    await _api.SaveTransactionAsync(new AspireBudgetApi.Models.Transaction
                    {
                        Account = CurrentTransaction.AccountTo,
                        Category = "↕️ Account Transfer",
                        Cleared = "🆗",
                        Date = CurrentTransaction.Date.Value,
                        Inflow = CurrentTransaction.Sum.Value
                    });
                    break;
            }
        }

        public void Dispose()
        {
            _api?.Dispose();
        }
    }
}
