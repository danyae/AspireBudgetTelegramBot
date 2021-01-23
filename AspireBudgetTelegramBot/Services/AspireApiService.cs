using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AspireBudgetApi;
using AspireBudgetApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AspireBudgetTelegramBot.Options;

namespace AspireBudgetTelegramBot.Services
{
    public class AspireApiService : IDisposable
    {
        private readonly IAspireApi _api;

        private List<string> _categories;
        private List<string> _accounts;

        public AspireApiService(ILogger<AspireApiService> logger,
            IOptions<AspireOptions> options
            )
        {
            string json = Encoding.UTF8.GetString(Convert.FromBase64String(options.Value.ApiCredentialsBase64));
            _api = new AspireApi(json, options.Value.SheetId, logger);
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            _categories ??= await _api.GetCategoriesAsync();
            return _categories;
        }
        
        public async Task<List<string>> GetAccountsAsync()
        {
            _accounts ??= await _api.GetAccountsAsync();
            return _accounts;
        }

        public async Task ReloadCacheAsync()
        {
            _accounts = null;
            _categories = null;
            await GetCategoriesAsync();
            await GetAccountsAsync();
        }

        public async Task<List<DashboardRow>> GetDashboardAsync()
        {
            return await _api.GetDashboardAsync();
        }

        public async Task SaveTransactionAsync(Transaction transaction)
        {
            await _api.SaveTransactionAsync(transaction);
        }

        public void Dispose()
        {
            _api?.Dispose();
        }
    }
}
