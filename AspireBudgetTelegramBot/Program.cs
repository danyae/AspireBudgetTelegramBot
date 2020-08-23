using AspireBudgetTelegramBot.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AspireBudgetTelegramBot.Models;
using AspireBudgetTelegramBot.Options;
using AspireBudgetTelegramBot.Services;
using AspireBudgetTelegramBot.Services.Authentication;
using AspireBudgetTelegramBot.Services.BackgroundQueue;
using AspireBudgetTelegramBot.Workers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AspireBudgetTelegramBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostBuilder = CreateHostBuilder(args).Build();
            
            UpdateDatabase(hostBuilder);
            
            hostBuilder.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<TelegramOptions>(hostContext.Configuration.GetSection(nameof(TelegramOptions)))
                        .Configure<AspireOptions>(hostContext.Configuration.GetSection(nameof(AspireOptions)))
                        .AddDbContext<AspireBudgetDbContext>()
                        .AddSingleton(typeof(IBackgroundQueue<TelegramMessage>),
                            typeof(BackgroundQueue<TelegramMessage>))
                        .AddSingleton(typeof(IBackgroundQueue<TelegramReplyMessage>),
                            typeof(BackgroundQueue<TelegramReplyMessage>))
                        .AddSingleton<TelegramBotService>()
                        .AddSingleton<AspireApiService>()
                        .AddSingleton(typeof(IAuthenticationService), typeof(InMemoryAuthenticationService))
                        .AddScoped<TransactionService>()
                        .AddHostedService<TelegramWorker>()
                        .AddHostedService<IncomingMessageWorker>()
                        .AddMediatR(typeof(Program));
                });
        
        private static void UpdateDatabase(IHost app)
        {
            using (var serviceScope = app.Services.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<AspireBudgetDbContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
