using System;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Commands.AccountFromCommand;
using AspireBudgetTelegramBot.Commands.AccountToOrCategoryCommand;
using AspireBudgetTelegramBot.Commands.DateCommand;
using AspireBudgetTelegramBot.Commands.ReloadCacheCommand;
using AspireBudgetTelegramBot.Commands.SumCommand;
using AspireBudgetTelegramBot.Commands.TypeCommand;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Queries.AccountFromQuery;
using AspireBudgetTelegramBot.Queries.AccountToQuery;
using AspireBudgetTelegramBot.Queries.CategoryQuery;
using AspireBudgetTelegramBot.Queries.CompleteQuery;
using AspireBudgetTelegramBot.Queries.CompleteTransactionQuery;
using AspireBudgetTelegramBot.Queries.DashboardQuery;
using AspireBudgetTelegramBot.Queries.DateQuery;
using AspireBudgetTelegramBot.Queries.TypeQuery;
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
            return await _mediator.Send(new DashboardQuery(msg));
        }
        
        public async Task<TelegramReplyMessage> ReloadCacheAsync(TelegramMessage msg)
        {
            await _mediator.Publish(new ReloadCacheCommand(msg));
            return await _mediator.Send(new CompleteQuery(msg));
        } 
        
        public async Task<TelegramReplyMessage> ProcessTransactionStepAsync(TelegramMessage msg)
        {
            TryInitNewTransaction();

            try
            {
                switch (CurrentTransaction.GetCurrentStep())
                {
                    case TransactionStep.Sum:
                        await _mediator.Send(new SumCommand(msg, CurrentTransaction));
                        return await _mediator.Send(new TypeQuery(msg));
                    case TransactionStep.Type:
                        await _mediator.Send(new TypeCommand(msg, CurrentTransaction));
                        return await _mediator.Send(new DateQuery(msg));
                    case TransactionStep.Date:
                        await _mediator.Send(new DateCommand(msg, CurrentTransaction));
                        return await _mediator.Send(new AccountFromQuery(msg));
                    case TransactionStep.AccountFrom:
                        await _mediator.Send(new AccountFromCommand(msg, CurrentTransaction));
                        if (CurrentTransaction.Type == Transaction.TypeTransfer)
                        {
                            return await _mediator.Send(new AccountToQuery(msg));
                        }

                        return await _mediator.Send(new CategoryQuery(msg));
                    case TransactionStep.AccountToOrCategory:
                        await _mediator.Send(new AccountToOrCategoryCommand(msg, CurrentTransaction));
                        var reply = await _mediator.Send(new CompleteTransactionQuery(msg, CurrentTransaction));
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