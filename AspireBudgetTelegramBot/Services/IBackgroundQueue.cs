using System.Threading;
using System.Threading.Tasks;

namespace AspireBudgetTelegramBot.Services
{
    public interface IBackgroundQueue<T>
    {
        public Task Write(T value);
        public ValueTask<T> ReadAsync(CancellationToken cancellationToken);
    }
}
