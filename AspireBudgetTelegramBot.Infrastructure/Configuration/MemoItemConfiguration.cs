using AspireBudgetTelegramBot.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspireBudgetTelegramBot.Infrastructure.Configuration
{
    public class MemoItemConfiguration: IEntityTypeConfiguration<MemoItem>
    {

        public void Configure(EntityTypeBuilder<MemoItem> builder)
        {
            builder.ToTable("memo_item");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(a => a.Memo).HasColumnName("memo").IsRequired();
            
            builder.Property(a => a.Type).HasColumnName("type").IsRequired();

            builder.Property(a => a.Category).HasColumnName("category");

            builder.Property(a => a.AccountFrom).HasColumnName("account_from").IsRequired();
            
            builder.Property(a => a.AccountTo).HasColumnName("account_to");

            builder.HasIndex(x => x.Memo);
        }
    }
}