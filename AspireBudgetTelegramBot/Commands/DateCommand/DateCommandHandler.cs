using System;
using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using MediatR;

namespace AspireBudgetTelegramBot.Commands.DateCommand
{
    /// <summary>
    /// Fill in date
    /// </summary>
    public class DateCommandHandler : AsyncRequestHandler<DateCommand>
    {
        protected override Task Handle(DateCommand request, CancellationToken cancellationToken)
        {
            if (request.Message.Text.Length < 2)
            {
                throw new TransactionAbortException("Date length < 2 symbols");
            }
            
            var date = request.Message.Text[..2];
            if (byte.TryParse(date, out var day) && day > 0 && day < 32)
            {
                var now = DateTime.Now;
                var tomorrow = now.AddDays(1);
                var transactionDate = now;
                
                if (now.Day > day)
                {
                    transactionDate = day == tomorrow.Day ? tomorrow : new DateTime(now.Year, now.Month, day);
                }
                else if (now.Day < day)
                {
                    transactionDate = new DateTime(now.Year, now.Month-1, day);
                }

                request.Transaction.Date = transactionDate;
                
                return Task.CompletedTask;
            }

            throw new TransactionAbortException("Can't parse date");
        }
    }
}