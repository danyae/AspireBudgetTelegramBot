using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Extensions;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Services;
using MediatR;
using Telegram.Bot.Types.ReplyMarkups;

namespace AspireBudgetTelegramBot.Queries.CompleteTransactionQuery
{
    public class CompleteTransactionQueryHandler : IRequestHandler<CompleteTransactionQuery, TelegramReplyMessage>
    {
        private readonly AspireApiService _apiService;
        
        public CompleteTransactionQueryHandler(AspireApiService apiService)
        {
            _apiService = apiService;
        }
        
        public async Task<TelegramReplyMessage> Handle(CompleteTransactionQuery request, CancellationToken cancellationToken)
        {
            if (request.Transaction.Type == Transaction.TypeTransfer)
            {
                return TelegramReplyMessage.OperationCompletedMessage(request.Message);
            }
            
            var dashboard = await _apiService.GetDashboardAsync();
            var row = dashboard.First(x => x.Name == request.Transaction.Category);
            var sb = new StringBuilder();
            sb.AppendLine("👌 Available | Spent | Budgeted");
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