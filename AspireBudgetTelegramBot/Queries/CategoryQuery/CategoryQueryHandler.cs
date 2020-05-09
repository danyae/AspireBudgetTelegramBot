using System.Threading;
using System.Threading.Tasks;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Services;
using MediatR;

namespace AspireBudgetTelegramBot.Queries.CategoryQuery
{
    public class CategoryQueryHandler : IRequestHandler<CategoryQuery, TelegramReplyMessage>
    {
        private readonly AspireApiService _apiService;

        public CategoryQueryHandler(AspireApiService apiService)
        {
            _apiService = apiService;
        }
        
        public async Task<TelegramReplyMessage> Handle(CategoryQuery request, CancellationToken cancellationToken)
        {
            var categories = await _apiService.GetCategoriesAsync();
            var reply = TelegramReplyMessage.RequestCategoryMessage(request.Message, categories);
            return reply;
        }
    }
}