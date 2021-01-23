using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspireBudgetApi.Models;
using AspireBudgetTelegramBot.Extensions;
using AspireBudgetTelegramBot.Infrastructure.Database;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Queries.RequestMemoHint;
using AspireBudgetTelegramBot.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;
using ILogger = Google.Apis.Logging.ILogger;
using Transaction = AspireBudgetTelegramBot.Models.Transaction;

namespace AspireBudgetTelegramBot.Queries.CompleteTransactionQuery
{
    public class CompleteTransactionQueryHandler : IRequestHandler<CompleteTransactionQuery, TelegramReplyMessage>
    {
        private readonly AspireApiService _apiService;
        private readonly ILogger<CompleteTransactionQueryHandler> _logger;

        public CompleteTransactionQueryHandler(AspireApiService apiService, ILogger<CompleteTransactionQueryHandler> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }
        
        public async Task<TelegramReplyMessage> Handle(CompleteTransactionQuery request, CancellationToken cancellationToken)
        {
            if (request.Transaction.Type == Transaction.TypeTransfer || request.Transaction.Category == "Available to budget")
            {
                return TelegramReplyMessage.OperationCompletedMessage(request.Message);
            }
            
            var dashboard = await _apiService.GetDashboardAsync();
            var row = dashboard.FirstOrDefault(x => x.Name == request.Transaction.Category && 
                                                    x.Type == DashboardRowType.Category);
            if (row == null)
            {
                _logger.LogError($"No dashboard category found to send for {request.Transaction.Category}: " +
                                 $"{string.Join(", ", dashboard.Select(x => x.Name).ToArray())}");
                row = new DashboardRow(
                    "Error", DashboardRowType.Category, 0, 0, 0, 0, 0, false
                );
            }
            var sb = new StringBuilder();
            sb.AppendLine("👌 Available | 💸 Spent | 💰 Budgeted");
            sb.AppendLine(row.ToHtmlSummary());
            
            var reply = new TelegramReplyMessage
            {
                ChatId = request.Message.ChatId,
                ReplyMarkup = new ReplyKeyboardRemove(),
                Text = sb.ToString()
            };
            return reply;
        }
    }
}