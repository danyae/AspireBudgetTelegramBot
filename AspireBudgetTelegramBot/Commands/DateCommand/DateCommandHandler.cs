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
                throw new TransactionAbortException();
            }
            
            var date = request.Message.Text[..2];
            if (byte.TryParse(date, out var day) && day > 0 && day < 32)
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
                
                return Task.CompletedTask;
            }

            throw new TransactionAbortException();
        }
    }
}