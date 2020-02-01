using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AspireBudgetTelegramBot.Services
{
    public class BackgroundQueue<T> : IBackgroundQueue<T>
    {
        private readonly Channel<T> _channel;

        public BackgroundQueue()
        {
            _channel = Channel.CreateUnbounded<T>();
        }

        public async Task Write(T value)
        {
           await _channel.Writer.WriteAsync(value);
        }

        public ValueTask<T> ReadAsync(CancellationToken cancellationToken)
        {
            return _channel.Reader.ReadAsync(cancellationToken);
        }
    }
}
