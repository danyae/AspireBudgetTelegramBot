using System;
using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Services;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.DateCommand
{
    /// <summary>
    /// Fill in date
    /// </summary>
    public class DateCommandHandler : IRequestHandler<DateCommand, TelegramReplyMessage>
    {
        private readonly AspireApiService _api;
        
        public DateCommandHandler(AspireApiService api)
        {
            _api = api;
        }
        
        public async Task<TelegramReplyMessage> Handle(DateCommand request, CancellationToken cancellationToken)
        {
            if (byte.TryParse(request.Message.Text, out var day) && day > 0 && day < 32)
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

                request.Transaction.Date = transactionDate;

                var accounts = await _api.GetAccountsAsync();
                var replay = TelegramReplyMessage.RequestAccountFromMessage(request.Message, accounts);
                return replay;
            }

            throw new TransactionAbortException();
        }
    }
}