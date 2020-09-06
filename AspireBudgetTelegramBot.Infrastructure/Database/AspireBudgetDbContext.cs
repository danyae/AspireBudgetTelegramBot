using AspireBudgetTelegramBot.Infrastructure.Configuration;
using AspireBudgetTelegramBot.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireBudgetTelegramBot.Infrastructure.Database
{
    public class AspireBudgetDbContext : DbContext
    {
        public DbSet<MemoItem> MemoItems { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=/db/aspire.sqlite");
        }
        
        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MemoItemConfiguration());
        }
    }
}