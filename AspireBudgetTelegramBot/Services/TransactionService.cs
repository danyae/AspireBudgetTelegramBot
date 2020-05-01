using System;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Commands.AccountFromCommand;
using AspireBudgetTelegramBot.Commands.AccountToOrCategoryCommand;
using AspireBudgetTelegramBot.Commands.DashboardCommand;
using AspireBudgetTelegramBot.Commands.DateCommand;
using AspireBudgetTelegramBot.Commands.ReloadCacheCommand;
using AspireBudgetTelegramBot.Commands.SumCommand;
using AspireBudgetTelegramBot.Commands.TypeCommand;
using AspireBudgetTelegramBot.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AspireBudgetTelegramBot.Services
{
    public class TransactionService
    {
        private static Transaction CurrentTransaction { get; set; }
        
        private readonly IMediator _mediator;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(IMediator mediator,
            ILogger<TransactionService> logger
            )
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        public async Task<TelegramReplyMessage> GetDashboardAsync(TelegramMessage msg)
        {
            return await _mediator.Send(new DashboardCommand(msg));
        }
        
        public async Task<TelegramReplyMessage> ReloadCacheAsync(TelegramMessage msg)
        {
            return await _mediator.Send(new ReloadCacheCommand(msg));
        } 
        
        public async Task<TelegramReplyMessage> ProcessTransactionStepAsync(TelegramMessage msg)
        {
            TryInitNewTransaction();

            try
            {
                switch (CurrentTransaction.GetCurrentStep())
                {
                    case TransactionStep.Sum:
                        return await _mediator.Send(new SumCommand(msg, CurrentTransaction));
                    case TransactionStep.Type:
                        return await _mediator.Send(new TypeCommand(msg, CurrentTransaction));
                    case TransactionStep.Date:
                        return await _mediator.Send(new DateCommand(msg, CurrentTransaction));
                    case TransactionStep.AccountFrom:
                        return await _mediator.Send(new AccountFromCommand(msg, CurrentTransaction));
                    case TransactionStep.AccountToOrCategory:
                        var reply = await _mediator.Send(new AccountToOrCategoryCommand(msg, CurrentTransaction));
                        RemoveTransaction();
                        return reply;
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

        private void TryInitNewTransaction()
        {
            CurrentTransaction ??= new Transaction();
        }

        private void RemoveTransaction()
        {
            CurrentTransaction = null;
        }
    }
}