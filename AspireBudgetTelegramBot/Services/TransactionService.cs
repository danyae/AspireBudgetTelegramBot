using System;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Commands.AccountFromCommand;
using AspireBudgetTelegramBot.Commands.AccountToOrCategoryCommand;
using AspireBudgetTelegramBot.Commands.AddOrUpdateMemoHint;
using AspireBudgetTelegramBot.Commands.DateCommand;
using AspireBudgetTelegramBot.Commands.ReloadCacheCommand;
using AspireBudgetTelegramBot.Commands.SaveTransaction;
using AspireBudgetTelegramBot.Commands.SumCommand;
using AspireBudgetTelegramBot.Commands.TypeCommand;
using AspireBudgetTelegramBot.Commands.UpdateTransactionFromMemoHint;
using AspireBudgetTelegramBot.Infrastructure.Models;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Queries.AccountFromQuery;
using AspireBudgetTelegramBot.Queries.AccountToQuery;
using AspireBudgetTelegramBot.Queries.CategoryQuery;
using AspireBudgetTelegramBot.Queries.CompleteTransactionQuery;
using AspireBudgetTelegramBot.Queries.DashboardQuery;
using AspireBudgetTelegramBot.Queries.DateQuery;
using AspireBudgetTelegramBot.Queries.GetMemoHint;
using AspireBudgetTelegramBot.Queries.RequestMemoHint;
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
            await _mediator.Send(new ReloadCacheCommand(msg));
            return TelegramReplyMessage.OperationCompletedMessage(msg);
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
                        if (!string.IsNullOrWhiteSpace(CurrentTransaction.Memo))
                        {
                            CurrentTransaction.MemoHint = await _mediator.Send(new GetMemoHintQuery(CurrentTransaction.Memo));
                            if (!CurrentTransaction.MemoHint.IsEmpty())
                            {
                                return await _mediator.Send(new RequestMemoHintQuery(msg, CurrentTransaction.MemoHint));
                            }
                        }
                        return await _mediator.Send(new TypeQuery(msg));
                    case TransactionStep.MemoHint:
                        if (msg.Text == TelegramReplyMessage.YesAnswer)
                        {
                            CurrentTransaction.UseMemo = true;
                            await _mediator.Send(new UpdateTransactionFromMemoHintCommand(CurrentTransaction));
                            return await _mediator.Send(new DateQuery(msg));
                        }
                        if (msg.Text == TelegramReplyMessage.NoAnswer)
                        {
                            CurrentTransaction.MemoHint = MemoItem.EmptyMemoItem;
                            return await _mediator.Send(new TypeQuery(msg));
                        }
                        throw new TransactionAbortException();
                    case TransactionStep.Type:
                        await _mediator.Send(new TypeCommand(msg, CurrentTransaction));
                        return await _mediator.Send(new DateQuery(msg));
                    case TransactionStep.Date:
                        await _mediator.Send(new DateCommand(msg, CurrentTransaction));
                        if (CurrentTransaction.UseMemo)
                        {
                            await _mediator.Send(new SaveTransactionCommand(CurrentTransaction));
                            var reply2 = await _mediator.Send(new CompleteTransactionQuery(msg, CurrentTransaction));
                            RemoveTransaction();
                            return reply2;
                        }
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
                        await _mediator.Send(new SaveTransactionCommand(CurrentTransaction));
                        if (!string.IsNullOrWhiteSpace(CurrentTransaction.Memo) && CurrentTransaction.Category != Transaction.TypeTransfer)
                        {
                            await _mediator.Send(new AddOrUpdateMemoHintCommand(CurrentTransaction.Memo,
                                CurrentTransaction.Type,
                                CurrentTransaction.Category, CurrentTransaction.AccountFrom,
                                CurrentTransaction.AccountTo));
                        }
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